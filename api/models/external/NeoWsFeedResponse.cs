using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record NeoWsFeedResponse
{
    [JsonPropertyName("element_count")]
    public int ElementCount { get; init; }

    [JsonPropertyName("near_earth_objects")]
    public Dictionary<string, List<NeoWsAsteroidResponse>> NearEarthObjects { get; init; } = new();
}