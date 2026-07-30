using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record LaunchListResponse
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("results")]
    public List<LaunchResponse> Results { get; init; } = new();
}