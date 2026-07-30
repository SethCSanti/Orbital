using Microsoft.AspNetCore.SignalR;
using Orbital.Api.Hubs;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.External;
using System.Text.Json;
using System.Globalization;
namespace Orbital.Api.Jobs;

public class IssSyncJob : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHubContext<IssHub> _hubContext;
    private readonly IRedisService _redis;
    private readonly ILogger<IssSyncJob> _logger;

    public IssSyncJob(
        IHttpClientFactory httpClientFactory,
        IHubContext<IssHub> hubContext,
        IRedisService redis,
        ILogger<IssSyncJob> logger)
    {
        _httpClientFactory = httpClientFactory;
        _hubContext = hubContext;
        _redis = redis;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                // your turn — fetch, parse, push, cache
                var response = await _httpClientFactory.CreateClient("OpenNotify").GetAsync("iss-now.json", stoppingToken);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var position = JsonSerializer.Deserialize<IssPositionResponse>(content);

                if (position?.Position == null)
                {
                    _logger.LogWarning("Received null position from ISS API.");
                    continue;
                }

                if (!double.TryParse(position.Position.Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
                {
                    _logger.LogWarning("Failed to parse latitude: {LatString}", position.Position.Latitude);
                    continue;
                }

                if (!double.TryParse(position.Position.Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                {
                    _logger.LogWarning("Failed to parse longitude: {LonString}", position.Position.Longitude);
                    continue;
                }

                var update = new IssPositionUpdate
                {
                    Latitude = lat,
                    Longitude = lon,
                    Timestamp = DateTimeOffset.FromUnixTimeSeconds(position.Timestamp)
                };

                await _hubContext.Clients.All.SendAsync("ReceiveIssPosition", update, stoppingToken);
                await _redis.SetAsync(CacheKeys.IssPosition, update, TimeSpan.FromSeconds(15));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to poll ISS position.");
            }
        }
    }
}