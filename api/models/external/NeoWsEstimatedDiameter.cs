using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record NeoWsEstimatedDiameter
{
    [JsonPropertyName("kilometers")]
    public NeoWsDiameterRange Kilometers { get; init; } = new();
}