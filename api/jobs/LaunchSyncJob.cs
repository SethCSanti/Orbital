using Orbital.Api.Infrastructure;
using Orbital.Api.Data;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Orbital.Api.Hubs;
namespace Orbital.Api.Jobs;

public interface ILaunchSyncJob
{
    Task ExecuteAsync();
}

public class LaunchSyncJob : ILaunchSyncJob
{
    private readonly IHttpClientFactory _httpClientFactory;
private readonly OrbitalDbContext _db;
private readonly IHubContext<LaunchHub> _hubContext;
private readonly IRedisService _redis;
private readonly ILogger<LaunchSyncJob> _logger;

    public LaunchSyncJob(
        IHttpClientFactory httpClientFactory,
        OrbitalDbContext db,
        IHubContext<LaunchHub> hubContext,
        IRedisService redis,
        ILogger<LaunchSyncJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _hubContext = hubContext;
        _redis = redis;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var client = _httpClientFactory.CreateClient("SpaceDevs");

        using var upcomingResponse = await client.GetAsync("launch/upcoming/?mode=detailed&limit=50");

        if (upcomingResponse.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var retryDelay = upcomingResponse.Headers.RetryAfter?.Delta
                ?? (upcomingResponse.Headers.RetryAfter?.Date - DateTimeOffset.UtcNow)
                ?? TimeSpan.FromMinutes(2);

            if (retryDelay < TimeSpan.Zero)
            {
                retryDelay = TimeSpan.Zero;
            }

            _logger.LogWarning(
                "SpaceDevs rate limit reached while fetching upcoming launches. Waiting {RetryDelay} before ending this run.",
                retryDelay);
            await Task.Delay(retryDelay);
            return;
        }

        if (!upcomingResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to fetch upcoming launch data. Status code: {StatusCode}",
                upcomingResponse.StatusCode);
            return;
        }

        using var previousResponse = await client.GetAsync("launch/previous/?mode=detailed&limit=50");

        if (previousResponse.StatusCode == HttpStatusCode.TooManyRequests)
        {
            var retryDelay = previousResponse.Headers.RetryAfter?.Delta
                ?? (previousResponse.Headers.RetryAfter?.Date - DateTimeOffset.UtcNow)
                ?? TimeSpan.FromMinutes(2);

            if (retryDelay < TimeSpan.Zero)
            {
                retryDelay = TimeSpan.Zero;
            }

            _logger.LogWarning(
                "SpaceDevs rate limit reached while fetching previous launches. Waiting {RetryDelay} before ending this run.",
                retryDelay);
            await Task.Delay(retryDelay);
            return;
        }

        if (!previousResponse.IsSuccessStatusCode)
        {
            _logger.LogError(
                "Failed to fetch previous launch data. Status code: {StatusCode}",
                previousResponse.StatusCode);
            return;
        }

        var upcomingContent = await upcomingResponse.Content.ReadAsStringAsync();
        var previousContent = await previousResponse.Content.ReadAsStringAsync();

        LaunchListResponse? upcomingData;
        LaunchListResponse? previousData;

        try
        {
            upcomingData = JsonSerializer.Deserialize<LaunchListResponse>(upcomingContent);
            previousData = JsonSerializer.Deserialize<LaunchListResponse>(previousContent);
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "Failed to deserialize SpaceDevs launch data.");
            return;
        }

        if (upcomingData is null || previousData is null)
        {
            _logger.LogError("Failed to deserialize SpaceDevs launch data.");
            return;
        }

        var allLaunches = upcomingData.Results.Concat(previousData.Results).ToList();

        var rocketCache = new Dictionary<string, Rocket>();
        var missionCache = new Dictionary<string, Mission>();
        var astronautCache = new Dictionary<string, Astronaut>();
        var mappedLaunches = new List<Launch>();

        foreach (var launch in allLaunches)
        {
            var rocket = await GetOrCreateRocketAsync(launch.Rocket.Configuration, rocketCache);
            var mission = await GetOrCreateMissionAsync(launch.Mission, missionCache);

            var crew = new List<Astronaut>();
            foreach (var crewMember in launch.Crew)
            {
                var astronaut = await GetOrCreateAstronautAsync(crewMember.Astronaut, astronautCache);
                crew.Add(astronaut);
            }

            // Include Rocket/Mission/Crew so EF can correctly diff the
            // many-to-many Crew collection against what's already saved.
            var existingLaunch = await _db.Launches
                .Include(l => l.Rocket)
                .Include(l => l.Mission)
                .Include(l => l.Crew)
                .FirstOrDefaultAsync(l => l.ExternalId == launch.ExternalId);

            if (existingLaunch == null)
            {
                var newLaunch = new Launch
                {
                    ExternalId = launch.ExternalId,
                    Name = launch.Name,
                    StatusName = launch.Status?.Name ?? string.Empty,
                    Net = launch.Net,
                    WindowStart = launch.WindowStart,
                    WindowEnd = launch.WindowEnd,
                    Probability = launch.Probability,
                    HoldReason = string.IsNullOrEmpty(launch.HoldReason) ? null : launch.HoldReason,
                    FailReason = string.IsNullOrEmpty(launch.FailReason) ? null : launch.FailReason,
                    Hashtag = launch.Hashtag,
                    Rocket = rocket,
                    Mission = mission,
                    Crew = crew
                };
                _db.Launches.Add(newLaunch);
                mappedLaunches.Add(newLaunch);
            }
            else
            {
                existingLaunch.Name = launch.Name;
                existingLaunch.StatusName = launch.Status?.Name ?? string.Empty;
                existingLaunch.Net = launch.Net;
                existingLaunch.WindowStart = launch.WindowStart;
                existingLaunch.WindowEnd = launch.WindowEnd;
                existingLaunch.Probability = launch.Probability;
                existingLaunch.HoldReason = string.IsNullOrEmpty(launch.HoldReason) ? null : launch.HoldReason;
                existingLaunch.FailReason = string.IsNullOrEmpty(launch.FailReason) ? null : launch.FailReason;
                existingLaunch.Hashtag = launch.Hashtag;
                existingLaunch.Rocket = rocket;
                existingLaunch.Mission = mission;
                existingLaunch.Crew = crew;
                mappedLaunches.Add(existingLaunch);
            }
        }

        var changedLaunches = _db.ChangeTracker.Entries<Launch>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .ToList();

        await _db.SaveChangesAsync();

        // Cache DTOs, not raw entities: Launch -> Crew -> Astronaut -> Astronaut.Launches -> Launch
        // is a circular reference, which System.Text.Json throws on by default.
        var now = DateTimeOffset.UtcNow;
        var upcomingDtos = mappedLaunches.Where(l => l.Net >= now).Select(l => new LaunchDto(l)).ToList();
        var pastDtos = mappedLaunches.Where(l => l.Net < now).Select(l => new LaunchDto(l)).ToList();

        _logger.LogInformation(
            "Synced {Count} launches ({Upcoming} upcoming, {Past} past)",
            mappedLaunches.Count, upcomingDtos.Count, pastDtos.Count);

        await _redis.SetAsync(CacheKeys.UpcomingLaunches, upcomingDtos, TimeSpan.FromMinutes(20));
        await _redis.SetAsync(CacheKeys.PastLaunches, pastDtos, TimeSpan.FromMinutes(20));
        if (changedLaunches.Count > 0)
        {
            var changedDtos = changedLaunches.Select(l => new LaunchDto(l)).ToList();
            await _hubContext.Clients.All.SendAsync("ReceiveLaunchUpdates", changedDtos);
            _logger.LogInformation("Broadcast {Count} changed launches over SignalR", changedDtos.Count);
        }
    }

    private async Task<Rocket> GetOrCreateRocketAsync(RocketConfigurationResponse src, Dictionary<string, Rocket> cache)
    {
        var key = $"{src.Name}|{src.Variant}";
        if (cache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var mapped = new Rocket
        {
            Name = src.Name,
            FullName = src.FullName,
            Family = src.Family,
            Active = src.Active,
            Reusable = src.Reusable,
            Description = src.Description ?? string.Empty,
            Variant = src.Variant ?? string.Empty,
            Length = src.Length ?? 0m,
            Diameter = src.Diameter ?? 0m,
            MaidenFlight = src.MaidenFlight ?? DateOnly.MinValue,
            LaunchCost = src.LaunchCost,
            LaunchMass = src.LaunchMass ?? 0m,
            LeoCapacity = src.LeoCapacity ?? 0m,
            GtoCapacity = src.GtoCapacity,
            ImageUrl = src.ImageUrl ?? string.Empty,
            WikiUrl = src.WikiUrl ?? string.Empty,
            TotalLaunchCount = src.TotalLaunchCount ?? 0,
            SuccessfulLaunchCount = src.SuccessfulLaunchCount ?? 0,
            FailedLaunchCount = src.FailedLaunchCount ?? 0
        };

        var existing = await _db.Rockets
            .FirstOrDefaultAsync(r => r.Name == src.Name && r.Variant == mapped.Variant);

        Rocket result;
        if (existing == null)
        {
            _db.Rockets.Add(mapped);
            result = mapped;
        }
        else
        {
            _db.Entry(existing).CurrentValues.SetValues(mapped);
            result = existing;
        }

        cache[key] = result;
        return result;
    }

    private async Task<Mission> GetOrCreateMissionAsync(MissionResponse src, Dictionary<string, Mission> cache)
    {
        if (cache.TryGetValue(src.Name, out var cached))
        {
            return cached;
        }

        var mapped = new Mission
        {
            Name = src.Name,
            Description = src.Description ?? string.Empty,
            Type = src.Type ?? string.Empty,
            LaunchDesignator = src.LaunchDesignator,
            OrbitName = src.Orbit?.Name ?? string.Empty,
            OrbitAbbrev = src.Orbit?.Abbrev ?? string.Empty
        };

        var existing = await _db.Missions.FirstOrDefaultAsync(m => m.Name == src.Name);

        Mission result;
        if (existing == null)
        {
            _db.Missions.Add(mapped);
            result = mapped;
        }
        else
        {
            _db.Entry(existing).CurrentValues.SetValues(mapped);
            result = existing;
        }

        cache[src.Name] = result;
        return result;
    }

    private async Task<Astronaut> GetOrCreateAstronautAsync(AstronautApiResponse src, Dictionary<string, Astronaut> cache)
    {
        if (cache.TryGetValue(src.Name, out var cached))
        {
            return cached;
        }

        var mapped = new Astronaut
        {
            Name = src.Name,
            Nationality = src.Nationality,
            DateOfBirth = src.DateOfBirth,
            DateOfDeath = src.DateOfDeath,
            Biography = src.Biography,
            ProfileImageUrl = src.ProfileImageUrl,
            WikipediaUrl = src.WikipediaUrl,
            FlightsCount = src.FlightsCount ?? 0
        };

        var existing = await _db.Astronauts.FirstOrDefaultAsync(a => a.Name == src.Name);

        Astronaut result;
        if (existing == null)
        {
            _db.Astronauts.Add(mapped);
            result = mapped;
        }
        else
        {
            _db.Entry(existing).CurrentValues.SetValues(mapped);
            result = existing;
        }

        cache[src.Name] = result;
        return result;
    }
}
