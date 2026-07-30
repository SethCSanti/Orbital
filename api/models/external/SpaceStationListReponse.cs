using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record SpaceStationListResponse
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("results")]
    public List<SpaceStationResponse> Results { get; init; } = new();
}