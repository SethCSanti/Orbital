using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record NeoWsRelativeVelocity
{
    // kilometers_per_hour — as a string, not decimal
    [JsonPropertyName("kilometers_per_hour")]
    public string KilometersPerHour { get; init; } = string.Empty;
}