using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record MissionResponse
{
    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("url")]
    public string? SourceUrl { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("launch_designator")]
    public string? LaunchDesignator { get; init; }

    [JsonPropertyName("orbit")]
    public OrbitResponse? Orbit { get; init; }
}
