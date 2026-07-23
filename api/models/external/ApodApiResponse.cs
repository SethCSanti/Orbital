using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record ApodApiResponse
{
    [JsonPropertyName("date")]
    public string Date { get; init; } = string.Empty;
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;
    [JsonPropertyName("explanation")]
    public string Explanation { get; init; } = string.Empty;
    [JsonPropertyName("hdurl")]
    public string? HdUrl { get; init; }
    [JsonPropertyName("media_type")]
    public string MediaType { get; init; } = string.Empty;
    [JsonPropertyName("service_version")]
    public string ServiceVersion { get; init; } = string.Empty;
    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;
    [JsonPropertyName("copyright")]
    public string? Copyright { get; init; }
}