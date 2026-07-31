using Orbital.Api.Infrastructure;
using Orbital.Api.Data;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;
using Microsoft.EntityFrameworkCore;
using Hangfire;
namespace Orbital.Api.Jobs;

[AutomaticRetry(Attempts = 0)]
public interface ISpaceStationSyncJob
{
    Task ExecuteAsync();
}

public class SpaceStationSyncJob : ISpaceStationSyncJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OrbitalDbContext _db;
    private readonly IRedisService _redis;
    private readonly ILogger<SpaceStationSyncJob> _logger;

    public SpaceStationSyncJob(
        IHttpClientFactory httpClientFactory,
        OrbitalDbContext db,
        IRedisService redis,
        ILogger<SpaceStationSyncJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _redis = redis;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var stationData = await _httpClientFactory.CreateClient("SpaceDevs")
            .GetSpaceDevsJsonAsync<SpaceStationListResponse>(
                "space_station/?mode=detailed&limit=20",
                _logger,
                "fetching space station data");
        if (stationData is null)
        {
            return;
        }

        var mappedStations = new List<SpaceStation>();

        foreach (var station in stationData.Results)
        {
            var stationEntity = new SpaceStation
            {
                Name = station.Name,
                Status = station.Status?.Name ?? string.Empty,
                Type = station.Type?.Name ?? string.Empty,
                Founded = station.Founded ?? DateOnly.MinValue,
                Deorbited = station.Deorbited,
                Description = station.Description ?? string.Empty,
                Orbit = station.Orbit ?? string.Empty,
                ImageUrl = station.ImageUrl ?? string.Empty
            };

            var existing = await _db.SpaceStations.FirstOrDefaultAsync(s => s.Name == stationEntity.Name);

            if (existing == null)
            {
                _db.SpaceStations.Add(stationEntity);
                mappedStations.Add(stationEntity);
            }
            else
            {
                _db.Entry(existing).CurrentValues.SetValues(stationEntity);
                mappedStations.Add(existing);
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Synced {Count} space stations", mappedStations.Count);
        await _redis.SetAsync(CacheKeys.SpaceStationData, mappedStations, TimeSpan.FromDays(7));
    }
}
