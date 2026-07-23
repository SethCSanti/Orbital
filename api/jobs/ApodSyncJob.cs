using Orbital.Api.Infrastructure;
using Orbital.Api.Data;
using Orbital.Api.Models.Entities;
using Orbital.Api.Models.External;
using Microsoft.EntityFrameworkCore;
namespace Orbital.Api.Jobs;
public class ApodSyncJob : IApodSyncJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly OrbitalDbContext _db;
    private readonly IRedisService _redis;
    private readonly ILogger<ApodSyncJob> _logger;
    private readonly IConfiguration _configuration;

    public ApodSyncJob(
        IHttpClientFactory httpClientFactory,
        OrbitalDbContext db,
        IRedisService redis,
        ILogger<ApodSyncJob> logger,
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
        var client = _httpClientFactory.CreateClient("Nasa");
        var apiKey = _configuration["Nasa:ApiKey"] ?? throw new InvalidOperationException("NASA API key is not configured.");
        var response = await client.GetAsync($"planetary/apod?api_key={apiKey}");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch APOD data. Status code: {StatusCode}", response.StatusCode);
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        var apodData = System.Text.Json.JsonSerializer.Deserialize<ApodApiResponse>(content);

        if (apodData == null)
        {
            _logger.LogError("Failed to deserialize APOD data.");
            return;
        }

        // Save to database
        var apodEntity = new ApodEntry
        {
            Date = DateOnly.Parse(apodData.Date),
            Title = apodData.Title,
            Explanation = apodData.Explanation,
            HdUrl = apodData.HdUrl,
            MediaType = apodData.MediaType,
            Url = apodData.Url,
            Copyright = apodData.Copyright
        };

        var existing = await _db.ApodEntries
            .FirstOrDefaultAsync(a => a.Date == apodEntity.Date);

        if (existing == null)
        {
            _db.ApodEntries.Add(apodEntity);
        }
        else
        {
            _db.Entry(existing).CurrentValues.SetValues(apodEntity);
        }

        await _db.SaveChangesAsync();
        await _redis.SetAsync(CacheKeys.Apod, apodEntity, TimeSpan.FromHours(24));
    }
}