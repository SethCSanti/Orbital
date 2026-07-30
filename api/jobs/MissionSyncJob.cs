using Orbital.Api.Infrastructure;
using Orbital.Api.Data;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Orbital.Api.Jobs;

public interface IMissionSyncJob
{
    Task ExecuteAsync();
}

public class MissionSyncJob : IMissionSyncJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OrbitalDbContext _db;
    private readonly IRedisService _redis;
    private readonly ILogger<MissionSyncJob> _logger;

    public MissionSyncJob(
        IHttpClientFactory httpClientFactory,
        OrbitalDbContext db,
        IRedisService redis,
        ILogger<MissionSyncJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _redis = redis;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var response = await _httpClientFactory.CreateClient("SpaceDevs")
            .GetAsync("launch/previous/?mode=detailed&limit=100");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch mission history data. Status code: {StatusCode}", response.StatusCode);
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        var launchData = JsonSerializer.Deserialize<LaunchListResponse>(content);

        if (launchData == null)
        {
            _logger.LogError("Failed to deserialize mission history data.");
            return;
        }

        var mappedMissions = new List<Mission>();

        foreach (var launch in launchData.Results)
        {
            var missionEntity = new Mission
            {
                Name = launch.Mission.Name,
                Description = launch.Mission.Description ?? string.Empty,
                Type = launch.Mission.Type ?? string.Empty,
                LaunchDesignator = launch.Mission.LaunchDesignator,
                OrbitName = launch.Mission.Orbit?.Name ?? string.Empty,
                OrbitAbbrev = launch.Mission.Orbit?.Abbrev ?? string.Empty
            };

            var existing = await _db.Missions.FirstOrDefaultAsync(m => m.Name == missionEntity.Name);

            if (existing == null)
            {
                _db.Missions.Add(missionEntity);
                mappedMissions.Add(missionEntity);
            }
            else
            {
                _db.Entry(existing).CurrentValues.SetValues(missionEntity);
                mappedMissions.Add(existing);
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Synced {Count} historical missions", mappedMissions.Count);
        await _redis.SetAsync(CacheKeys.MissionHistory, mappedMissions, TimeSpan.FromHours(24));
    }
}