using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Orbital.Api.Data;
using Orbital.Api.Hubs;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;

namespace Orbital.Api.Jobs;

public interface ILaunchSyncJob
{
    Task ExecuteAsync();
}

/// <summary>
/// Refreshes the live launch window and advances the historical archive by one
/// upstream page per run. The checkpoint makes the import restartable and keeps
/// a slow/rate-limited upstream from monopolising a Hangfire worker.
/// </summary>
[DisableConcurrentExecution(900)]
public class LaunchSyncJob : ILaunchSyncJob
{
    private const string HistoryCatalog = "launch-history";
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
        var state = await GetOrCreateHistoryStateAsync();
        var historyWasComplete = state.Status == "complete";
        state.Status = "running";
        state.LastStartedAt = DateTimeOffset.UtcNow;
        state.LastError = null;
        state.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();

        var upcoming = await client.GetSpaceDevsJsonAsync<LaunchListResponse>(
            "launch/upcoming/?mode=detailed&limit=50", _logger, "fetching upcoming launches");
        if (upcoming is null)
        {
            await MarkHistoryFailureAsync(state, "The upstream source did not return upcoming launches.");
            return;
        }

        LaunchListResponse? historical = null;
        if (!historyWasComplete)
        {
            var offset = state.CurrentPage * state.PageSize;
            historical = await client.GetSpaceDevsJsonAsync<LaunchListResponse>(
                $"launch/previous/?mode=detailed&limit={state.PageSize}&offset={offset}",
                _logger,
                $"fetching historical launch page {state.CurrentPage}");
            if (historical is null)
            {
                await MarkHistoryFailureAsync(state, "The upstream source did not return the historical launch page.");
                return;
            }
            state.TotalAvailable = historical.Count;
        }

        var allLaunches = upcoming.Results.Concat(historical?.Results ?? []).ToList();
        var rocketCache = new Dictionary<string, Rocket>();
        var missionCache = new Dictionary<string, Mission>();
        var mappedLaunches = new List<Launch>();

        foreach (var launch in allLaunches)
        {
            var rocket = await GetOrCreateRocketAsync(launch.Rocket.Configuration, rocketCache);
            var mission = await GetOrCreateMissionAsync(launch.Mission, missionCache);
            var existing = await _db.Launches
                .Include(item => item.Rocket)
                .Include(item => item.Mission)
                .FirstOrDefaultAsync(item => item.ExternalId == launch.ExternalId);

            if (existing is null)
            {
                existing = new Launch { ExternalId = launch.ExternalId };
                _db.Launches.Add(existing);
            }

            existing.SourceUrl = launch.SourceUrl;
            existing.Name = launch.Name;
            existing.StatusName = launch.Status?.Name ?? string.Empty;
            existing.Net = launch.Net;
            existing.WindowStart = launch.WindowStart;
            existing.WindowEnd = launch.WindowEnd;
            existing.Probability = launch.Probability;
            existing.HoldReason = string.IsNullOrEmpty(launch.HoldReason) ? null : launch.HoldReason;
            existing.FailReason = string.IsNullOrEmpty(launch.FailReason) ? null : launch.FailReason;
            existing.Hashtag = launch.Hashtag;
            existing.Rocket = rocket;
            existing.Mission = mission;
            rocket.LastLaunchDate = rocket.LastLaunchDate is null || launch.Net > rocket.LastLaunchDate ? launch.Net : rocket.LastLaunchDate;
            mission.LastLaunchDate = mission.LastLaunchDate is null || launch.Net > mission.LastLaunchDate ? launch.Net : mission.LastLaunchDate;
            mappedLaunches.Add(existing);
        }

        var changedLaunches = _db.ChangeTracker.Entries<Launch>()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified)
            .Select(entry => entry.Entity)
            .ToList();
        await _db.SaveChangesAsync();

        if (historical is not null)
        {
            var offset = state.CurrentPage * state.PageSize;
            var reachedEnd = historical.Results.Count == 0 || offset + historical.Results.Count >= historical.Count;
            state.Status = reachedEnd ? "complete" : "partial";
            if (!reachedEnd) state.CurrentPage++;
            state.RecordsImported = await _db.Launches.CountAsync();
            state.LastCompletedAt = reachedEnd ? DateTimeOffset.UtcNow : state.LastCompletedAt;
        }
        else
        {
            state.Status = historyWasComplete ? "complete" : "partial";
        }
        state.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();

        // The launch endpoints use short-lived cache entries; invalidate both
        // after an archive page so readers never see a permanently partial list.
        await _redis.DeleteAsync(CacheKeys.UpcomingLaunches);
        await _redis.DeleteAsync(CacheKeys.PastLaunches);

        var changedDtos = changedLaunches.Select(launch => new LaunchDto(launch)).ToList();
        if (changedDtos.Count > 0)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveLaunchUpdates", changedDtos);
        }
        _logger.LogInformation("Synced {Count} launches; history status is {Status} at page {Page}", mappedLaunches.Count, state.Status, state.CurrentPage);
    }

    private async Task<CatalogSyncState> GetOrCreateHistoryStateAsync()
    {
        var state = await _db.CatalogSyncStates.FirstOrDefaultAsync(item => item.Catalog == HistoryCatalog);
        if (state is not null) return state;
        state = new CatalogSyncState { Catalog = HistoryCatalog, Status = "pending", PageSize = 100 };
        _db.CatalogSyncStates.Add(state);
        await _db.SaveChangesAsync();
        return state;
    }

    private async Task MarkHistoryFailureAsync(CatalogSyncState state, string error)
    {
        state.Status = state.CurrentPage == 0 ? "pending" : "partial";
        state.LastError = error;
        state.UpdatedAt = DateTimeOffset.UtcNow;
        await _db.SaveChangesAsync();
        _logger.LogWarning("Launch history backfill paused: {Error}", error);
    }

    private async Task<Rocket> GetOrCreateRocketAsync(RocketConfigurationResponse source, Dictionary<string, Rocket> cache)
    {
        var key = source.Id?.ToString() ?? $"{source.Name}|{source.Variant}";
        if (cache.TryGetValue(key, out var cached)) return cached;
        var mapped = new Rocket
        {
            SourceId = source.Id?.ToString(), SourceUrl = source.SourceUrl,
            Name = source.Name, FullName = source.FullName, Family = source.Family,
            Active = source.Active, Reusable = source.Reusable, Description = source.Description ?? string.Empty,
            Variant = source.Variant ?? string.Empty, Length = source.Length ?? 0m, Diameter = source.Diameter ?? 0m,
            MaidenFlight = source.MaidenFlight ?? DateOnly.MinValue, LaunchCost = source.LaunchCost,
            LaunchMass = source.LaunchMass ?? 0m, LeoCapacity = source.LeoCapacity ?? 0m, GtoCapacity = source.GtoCapacity,
            ImageUrl = source.ImageUrl ?? string.Empty, WikiUrl = source.WikiUrl ?? string.Empty,
            TotalLaunchCount = source.TotalLaunchCount ?? 0, SuccessfulLaunchCount = source.SuccessfulLaunchCount ?? 0,
            FailedLaunchCount = source.FailedLaunchCount ?? 0
        };
        var existing = source.Id is not null
            ? await _db.Rockets.FirstOrDefaultAsync(item => item.SourceId == mapped.SourceId)
            : await _db.Rockets.FirstOrDefaultAsync(item => item.Name == mapped.Name && item.Variant == mapped.Variant);
        Rocket result;
        if (existing is null) { _db.Rockets.Add(mapped); result = mapped; }
        else
        {
            mapped.LastLaunchDate = existing.LastLaunchDate;
            _db.Entry(existing).CurrentValues.SetValues(mapped);
            result = existing;
        }
        cache[key] = result;
        return result;
    }

    private async Task<Mission> GetOrCreateMissionAsync(MissionResponse source, Dictionary<string, Mission> cache)
    {
        var key = source.Id?.ToString() ?? source.Name;
        if (cache.TryGetValue(key, out var cached)) return cached;
        var mapped = new Mission
        {
            SourceId = source.Id?.ToString(), SourceUrl = source.SourceUrl, Name = source.Name,
            Description = source.Description ?? string.Empty, Type = source.Type ?? string.Empty,
            LaunchDesignator = source.LaunchDesignator, OrbitName = source.Orbit?.Name ?? string.Empty,
            OrbitAbbrev = source.Orbit?.Abbrev ?? string.Empty
        };
        var existing = source.Id is not null
            ? await _db.Missions.FirstOrDefaultAsync(item => item.SourceId == mapped.SourceId)
            : await _db.Missions.FirstOrDefaultAsync(item => item.Name == mapped.Name);
        Mission result;
        if (existing is null) { _db.Missions.Add(mapped); result = mapped; }
        else
        {
            mapped.LastLaunchDate = existing.LastLaunchDate;
            _db.Entry(existing).CurrentValues.SetValues(mapped);
            result = existing;
        }
        cache[key] = result;
        return result;
    }

}
