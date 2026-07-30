using Orbital.Api.Infrastructure;
using Orbital.Api.Data;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Orbital.Api.Jobs;

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
        var response = await _httpClientFactory.CreateClient("SpaceDevs")
            .GetAsync("space_station/?mode=detailed&limit=20");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch space station data. Status code: {StatusCode}", response.StatusCode);
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        var stationData = JsonSerializer.Deserialize<SpaceStationListResponse>(content);

        if (stationData == null)
        {
            _logger.LogError("Failed to deserialize space station data.");
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