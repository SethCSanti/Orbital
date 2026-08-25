using Orbital.Api.Infrastructure;
using Orbital.Api.Data;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
namespace Orbital.Api.Jobs;

public interface IAsteroidSyncJob
{
    Task ExecuteAsync();
}

public class AsteroidSyncJob : IAsteroidSyncJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OrbitalDbContext _db;
    private readonly IRedisService _redis;
    private readonly ILogger<AsteroidSyncJob> _logger;
    private readonly IConfiguration _configuration;

    public AsteroidSyncJob(
        IHttpClientFactory httpClientFactory,
        OrbitalDbContext db,
        IRedisService redis,
        ILogger<AsteroidSyncJob> logger,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _redis = redis;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task ExecuteAsync()
    {
        var startDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var endDate = startDate.AddDays(7);
        var apiKey = _configuration["Nasa:ApiKey"] ?? throw new InvalidOperationException("NASA API key is not configured.");
        var response = await _httpClientFactory.CreateClient("Nasa").GetAsync(
            $"neo/rest/v1/feed?start_date={startDate:yyyy-MM-dd}&end_date={endDate:yyyy-MM-dd}&api_key={apiKey}");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch asteroid data. Status code: {StatusCode}", response.StatusCode);
            return;
        }
        var content = await response.Content.ReadAsStringAsync();
        var feedData = System.Text.Json.JsonSerializer.Deserialize<NeoWsFeedResponse>(content);
        if (feedData == null)
        {
            _logger.LogError("Failed to deserialize asteroid feed data.");
            return;
        }
        var allAsteroids = feedData.NearEarthObjects.Values.SelectMany(list => list).ToList();
        var mappedAsteroids = new List<Asteroid>();

        foreach (var asteroid in allAsteroids)
        {
            var asteroidEntity = new Asteroid
            {
                NeoReferenceId = asteroid.NeoReferenceId,
                Name = asteroid.Name,
                NasaJplUrl = asteroid.NasaJplUrl,
                AbsoluteMagnitudeH = asteroid.AbsoluteMagnitudeH,
                EstimatedDiameterMinKm = asteroid.EstimatedDiameter.Kilometers.EstimatedDiameterMin,
                EstimatedDiameterMaxKm = asteroid.EstimatedDiameter.Kilometers.EstimatedDiameterMax,
                IsPotentiallyHazardous = asteroid.IsPotentiallyHazardous,
                IsSentryObject = asteroid.IsSentryObject,
                CloseApproachDate = DateOnly.Parse(asteroid.CloseApproachData.First().CloseApproachDate),
                RelativeVelocityKph = decimal.Parse(
                    asteroid.CloseApproachData.First().RelativeVelocity.KilometersPerHour,
                    CultureInfo.InvariantCulture),
                MissDistanceKm = decimal.Parse(
                    asteroid.CloseApproachData.First().MissDistance.Kilometers,
                    CultureInfo.InvariantCulture)
            };

            mappedAsteroids.Add(asteroidEntity);

            var existingAsteroid = await _db.Asteroids
                .FirstOrDefaultAsync(a => a.NeoReferenceId == asteroidEntity.NeoReferenceId);

            if (existingAsteroid == null)
            {
                _db.Asteroids.Add(asteroidEntity);
            }
            else
            {
                _db.Entry(existingAsteroid).CurrentValues.SetValues(asteroidEntity);
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Synced {Count} asteroids for window {Start} to {End}", mappedAsteroids.Count, startDate, endDate);
        await _redis.SetAsync(CacheKeys.AsteroidFeed, mappedAsteroids, TimeSpan.FromHours(24));
    }
}
