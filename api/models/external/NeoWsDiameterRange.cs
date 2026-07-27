using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record NeoWsDiameterRange
{
    // two decimal fields, matching estimated_diameter_min / estimated_diameter_max
    [JsonPropertyName("estimated_diameter_min")]
    public decimal EstimatedDiameterMin { get; init; }

    [JsonPropertyName("estimated_diameter_max")]
    public decimal EstimatedDiameterMax { get; init; }
}