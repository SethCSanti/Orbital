using Orbital.Api.Infrastructure;
using Orbital.Api.Models.External;
namespace Orbital.Api.Jobs;

public interface ITleSyncJob
{
    Task ExecuteAsync();
}

public class TleSyncJob : ITleSyncJob
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IRedisService _redis;
    private readonly ILogger<TleSyncJob> _logger;

    public TleSyncJob(
        IHttpClientFactory httpClientFactory,
        IRedisService redis,
        ILogger<TleSyncJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _redis = redis;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var response = await _httpClientFactory.CreateClient("Celestrak")
            .GetAsync("NORAD/elements/gp.php?CATNR=25544&FORMAT=TLE");
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to fetch TLE data. Status code: {StatusCode}", response.StatusCode);
            return;
        }
        var content = await response.Content.ReadAsStringAsync();
        var contentLines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (contentLines.Length < 3)
        {
            _logger.LogError("Unexpected TLE data format.");
            return;
        }

        var tleObject = new TleObject
        {
            Name = contentLines[0],
            Line1 = contentLines[1],
            Line2 = contentLines[2]
        };

        _logger.LogInformation("Synced TLE data for ISS (NORAD {CatalogNumber})", 25544);
        await _redis.SetAsync(CacheKeys.IssTle, tleObject, TimeSpan.FromHours(6));
    }
}