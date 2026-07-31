using Orbital.Api.Infrastructure;
using Orbital.Api.Data;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;
using Microsoft.EntityFrameworkCore;
using Hangfire;
namespace Orbital.Api.Jobs;

[AutomaticRetry(Attempts = 0)]
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
        var launchData = await _httpClientFactory.CreateClient("SpaceDevs")
            .GetSpaceDevsJsonAsync<LaunchListResponse>(
                "launch/previous/?mode=detailed&limit=100",
                _logger,
                "fetching mission history");
        if (launchData is null)
        {
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
