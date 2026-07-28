using Orbital.Api.Infrastructure;
using Orbital.Api.Data;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;
using Microsoft.EntityFrameworkCore;
namespace Orbital.Api.Jobs;

public interface IExoplanetSyncJob
{
    Task ExecuteAsync();
}

public class ExoplanetSyncJob : IExoplanetSyncJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OrbitalDbContext _db;
    private readonly IRedisService _redis;
    private readonly ILogger<ExoplanetSyncJob> _logger;

    public ExoplanetSyncJob(
        IHttpClientFactory httpClientFactory,
        OrbitalDbContext db,
        IRedisService redis,
        ILogger<ExoplanetSyncJob> logger
    )
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _redis = redis;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var query = "SELECT pl_name,hostname,disc_year,discoverymethod,disc_facility,pl_orbper,pl_rade,pl_bmasse,pl_orbsmax FROM ps";
        var url = $"TAP/sync?query={Uri.EscapeDataString(query)}&format=json";
        var response = await _httpClientFactory.CreateClient("ExoplanetArchive").GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch exoplanet data. Status code: {StatusCode}", response.StatusCode);
            return;
        }
        var content = await response.Content.ReadAsStringAsync();
        var exoplanets = System.Text.Json.JsonSerializer.Deserialize<List<ExoplanetApiResponse>>(content);
        if (exoplanets == null)
        {
            _logger.LogError("Failed to deserialize exoplanet feed data.");
            return;
        }
        var mappedExoplanets = new List<Exoplanet>();

        foreach (var exoplanet in exoplanets)
        {
            var exoplanetEntity = new Exoplanet
            {
                PlanetName = exoplanet.PlanetName,
                HostName = exoplanet.HostName,
                DiscoveryYear = exoplanet.DiscoveryYear,
                DiscoveryMethod = exoplanet.DiscoveryMethod,
                DiscoveryFacility = exoplanet.DiscoveryFacility,
                OrbitalPeriodDays = exoplanet.OrbitalPeriodDays,
                RadiusEarthRadii = exoplanet.RadiusEarthRadii,
                MassEarthMasses = exoplanet.MassEarthMasses,
                SemiMajorAxisAu = exoplanet.SemiMajorAxisAu
            };

            mappedExoplanets.Add(exoplanetEntity);

            var existingExoplanet = await _db.Exoplanets
                .FirstOrDefaultAsync(e => e.PlanetName == exoplanetEntity.PlanetName && e.HostName == exoplanetEntity.HostName);

            if (existingExoplanet == null)
            {
                _db.Exoplanets.Add(exoplanetEntity);
            }
            else
            {
                _db.Entry(existingExoplanet).CurrentValues.SetValues(exoplanetEntity);
            }
        }
        
        await _db.SaveChangesAsync();
        _logger.LogInformation("Synced {Count} exoplanets", exoplanets.Count);
        await _redis.SetAsync(CacheKeys.ExoplanetData, mappedExoplanets, TimeSpan.FromHours(24));
    }
}