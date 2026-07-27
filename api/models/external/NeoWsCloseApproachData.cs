using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record NeoWsCloseApproachData
{
    // close_approach_date (string, matches NASA's date format)
    [JsonPropertyName("close_approach_date")]
    public string CloseApproachDate { get; init; } = string.Empty;
    // relative_velocity (nested — another record)
    [JsonPropertyName("relative_velocity")]
    public NeoWsRelativeVelocity RelativeVelocity { get; init; } = new();
    // miss_distance (nested — another record)
    [JsonPropertyName("miss_distance")]
    public NeoWsMissDistance MissDistance { get; init; } = new();
}