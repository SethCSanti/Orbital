using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record OrbitResponse
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("abbrev")]
    public string Abbrev { get; init; } = string.Empty;
}