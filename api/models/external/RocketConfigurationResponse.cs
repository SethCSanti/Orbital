using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record RocketConfigurationResponse
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; init; } = string.Empty;

    [JsonPropertyName("family")]
    public string Family { get; init; } = string.Empty;

    [JsonPropertyName("active")]
    public bool Active { get; init; }

    [JsonPropertyName("reusable")]
    public bool Reusable { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("variant")]
    public string? Variant { get; init; }

    [JsonPropertyName("length")]
    public decimal? Length { get; init; }

    [JsonPropertyName("diameter")]
    public decimal? Diameter { get; init; }

    [JsonPropertyName("maiden_flight")]
    public DateOnly? MaidenFlight { get; init; }

    [JsonPropertyName("launch_cost")]
    public decimal? LaunchCost { get; init; }

    [JsonPropertyName("launch_mass")]
    public decimal? LaunchMass { get; init; }

    [JsonPropertyName("leo_capacity")]
    public decimal? LeoCapacity { get; init; }

    [JsonPropertyName("gto_capacity")]
    public decimal? GtoCapacity { get; init; }

    [JsonPropertyName("image_url")]
    public string? ImageUrl { get; init; }

    [JsonPropertyName("wiki_url")]
    public string? WikiUrl { get; init; }

    [JsonPropertyName("total_launch_count")]
    public int? TotalLaunchCount { get; init; }

    [JsonPropertyName("successful_launches")]
    public int? SuccessfulLaunchCount { get; init; }

    [JsonPropertyName("failed_launches")]
    public int? FailedLaunchCount { get; init; }
}