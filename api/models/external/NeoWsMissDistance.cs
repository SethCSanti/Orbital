using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record NeoWsMissDistance
{
    // kilometers — as a string, not decimal
    [JsonPropertyName("kilometers")]
    public string Kilometers { get; init; } = string.Empty;
}