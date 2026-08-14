using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record ExoplanetDto(
    string PlanetName,
    string HostName,
    int DiscoveryYear,
    string DiscoveryMethod,
    string DiscoveryFacility,
    decimal? OrbitalPeriodDays,
    decimal? RadiusEarthRadii,
    decimal? MassEarthMasses,
    decimal? SemiMajorAxisAu)
{
    public ExoplanetDto() : this(string.Empty, string.Empty, 0, string.Empty, string.Empty, null, null, null, null) { }

    public ExoplanetDto(Exoplanet entity) : this(
        entity.PlanetName,
        entity.HostName,
        entity.DiscoveryYear,
        entity.DiscoveryMethod,
        entity.DiscoveryFacility,
        entity.OrbitalPeriodDays,
        entity.RadiusEarthRadii,
        entity.MassEarthMasses,
        entity.SemiMajorAxisAu)
    { }
}
