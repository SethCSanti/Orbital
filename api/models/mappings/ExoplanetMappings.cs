using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Models.Mappings;

public static class ExoplanetMappings
{
    public static ExoplanetDto ToDto(this Exoplanet entity)
    {
        // your mapping code goes here
        return new ExoplanetDto
        {
            PlanetName = entity.PlanetName,
            HostName = entity.HostName,
            DiscoveryYear = entity.DiscoveryYear,
            DiscoveryMethod = entity.DiscoveryMethod,
            DiscoveryFacility = entity.DiscoveryFacility,
            OrbitalPeriodDays = entity.OrbitalPeriodDays,
            RadiusEarthRadii = entity.RadiusEarthRadii,
            MassEarthMasses = entity.MassEarthMasses,
            SemiMajorAxisAu = entity.SemiMajorAxisAu
        };
    }
}