using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Orbital.Api.Infrastructure;

public static class SpaceDevsApiClientExtensions
{
    private static readonly TimeSpan DefaultRateLimitDelay = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan MaximumRateLimitDelay = TimeSpan.FromSeconds(30);

    public static async Task<T?> GetSpaceDevsJsonAsync<T>(
        this HttpClient client,
        string requestUri,
        ILogger logger,
        string operation,
        CancellationToken cancellationToken = default)
        where T : class
    {
        try
        {
            using var response = await client.GetAsync(requestUri, cancellationToken);

            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                var delay = GetRateLimitDelay(response.Headers.RetryAfter);

                logger.LogWarning(
                    "SpaceDevs rate limit reached while {Operation}. Skipping this run after a {DelaySeconds}-second backoff.",
                    operation,
                    delay.TotalSeconds);

                await Task.Delay(delay, cancellationToken);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                logger.LogError(
                    "SpaceDevs request failed while {Operation}. Status code: {StatusCode} ({ReasonPhrase}).",
                    operation,
                    (int)response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<T>(content);

            if (result is null)
            {
                logger.LogError(
                    "SpaceDevs returned an empty JSON payload while {Operation}.",
                    operation);
            }

            return result;
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(
                exception,
                "Transient HTTP failure while {Operation}. Skipping this run.",
                operation);
            return null;
        }
        catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(
                exception,
                "SpaceDevs request timed out while {Operation}. Skipping this run.",
                operation);
            return null;
        }
        catch (JsonException exception)
        {
            logger.LogError(
                exception,
                "SpaceDevs returned invalid JSON while {Operation}. Skipping this run.",
                operation);
            return null;
        }
    }

    private static TimeSpan GetRateLimitDelay(RetryConditionHeaderValue? retryAfter)
    {
        var delay = retryAfter?.Delta;

        if (delay is null && retryAfter?.Date is { } retryDate)
        {
            delay = retryDate - DateTimeOffset.UtcNow;
        }

        if (delay is null || delay <= TimeSpan.Zero)
        {
            return DefaultRateLimitDelay;
        }

        return delay > MaximumRateLimitDelay
            ? MaximumRateLimitDelay
            : delay.Value;
    }
}
