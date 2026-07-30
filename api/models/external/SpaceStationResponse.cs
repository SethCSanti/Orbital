using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record SpaceStationResponse
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public NamedReference? Status { get; init; }

    [JsonPropertyName("type")]
    public NamedReference? Type { get; init; }

    [JsonPropertyName("founded")]
    public DateOnly? Founded { get; init; }

    [JsonPropertyName("deorbited")]
    public DateOnly? Deorbited { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("orbit")]
    public string? Orbit { get; init; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; init; }
}