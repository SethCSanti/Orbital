using System.Net;
using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using Orbital.Api.Infrastructure;
using Orbital.Api.Models.External;

namespace Orbital.Tests.Infrastructure;

public class SpaceDevsApiClientExtensionsTests
{
    [Fact]
    public async Task GetSpaceDevsJsonAsync_DeserializesSuccessfulResponse()
    {
        using var client = CreateClient(
            HttpStatusCode.OK,
            """{"count":0,"results":[]}""");

        var result = await client.GetSpaceDevsJsonAsync<LaunchListResponse>(
            "launch/",
            NullLogger.Instance,
            "testing a successful response");

        Assert.NotNull(result);
        Assert.Empty(result.Results);
    }

    [Fact]
    public async Task GetSpaceDevsJsonAsync_DoesNotDeserializeErrorResponse()
    {
        using var client = CreateClient(
            HttpStatusCode.InternalServerError,
            """{"detail":"Temporary upstream failure"}""");

        var result = await client.GetSpaceDevsJsonAsync<LaunchListResponse>(
            "launch/",
            NullLogger.Instance,
            "testing an error response");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSpaceDevsJsonAsync_ReturnsNullForRateLimit()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests)
        {
            Content = new StringContent(
                """{"detail":"Request was throttled."}""",
                Encoding.UTF8,
                "application/json")
        };
        response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(
            TimeSpan.FromMilliseconds(1));
        using var client = new HttpClient(new StubHttpMessageHandler(response))
        {
            BaseAddress = new Uri("https://example.test/")
        };

        var result = await client.GetSpaceDevsJsonAsync<LaunchListResponse>(
            "launch/",
            NullLogger.Instance,
            "testing rate limiting");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSpaceDevsJsonAsync_ReturnsNullForInvalidSuccessJson()
    {
        using var client = CreateClient(HttpStatusCode.OK, "not-json");

        var result = await client.GetSpaceDevsJsonAsync<LaunchListResponse>(
            "launch/",
            NullLogger.Instance,
            "testing invalid JSON");

        Assert.Null(result);
    }

    private static HttpClient CreateClient(HttpStatusCode statusCode, string content)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json")
        };

        return new HttpClient(new StubHttpMessageHandler(response))
        {
            BaseAddress = new Uri("https://example.test/")
        };
    }

    private sealed class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(response);
    }
}
