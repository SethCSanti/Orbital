using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record NeoWsAsteroidResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    [JsonPropertyName("neo_reference_id")]
    public string NeoReferenceId { get; init; } = string.Empty;
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
    [JsonPropertyName("nasa_jpl_url")]
    public string NasaJplUrl { get; init; } = string.Empty;
    [JsonPropertyName("absolute_magnitude_h")]
    public decimal AbsoluteMagnitudeH { get; init; }
    [JsonPropertyName("is_potentially_hazardous_asteroid")]
    public bool IsPotentiallyHazardous { get; init; }
    [JsonPropertyName("is_sentry_object")]
    public bool IsSentryObject { get; init; }
}