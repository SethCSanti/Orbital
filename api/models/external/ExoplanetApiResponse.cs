using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record ExoplanetApiResponse
{
    [JsonPropertyName("pl_name")]
    public string PlanetName { get; init; } = string.Empty;

    [JsonPropertyName("hostname")]
    public string HostName { get; init; } = string.Empty;

    [JsonPropertyName("disc_year")]
    public int DiscoveryYear { get; init; }

    [JsonPropertyName("discoverymethod")]
    public string DiscoveryMethod { get; init; } = string.Empty;

    [JsonPropertyName("disc_facility")]
    public string DiscoveryFacility { get; init; } = string.Empty;

    [JsonPropertyName("pl_orbper")]
    public decimal? OrbitalPeriodDays { get; init; }

    [JsonPropertyName("pl_rade")]
    public decimal? RadiusEarthRadii { get; init; }

    [JsonPropertyName("pl_bmasse")]
    public decimal? MassEarthMasses { get; init; }

    [JsonPropertyName("pl_orbsmax")]
    public decimal? SemiMajorAxisAu { get; init; }
}