using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Models.Mappings;

public static class AsteroidMappings
{
    public static AsteroidDto ToDto(this Asteroid entity)
    {
        // your mapping code goes here
        return new AsteroidDto
        {
            NeoReferenceId = entity.NeoReferenceId,
            Name = entity.Name,
            NasaJplUrl = entity.NasaJplUrl,
            AbsoluteMagnitudeH = entity.AbsoluteMagnitudeH,
            EstimatedDiameterMinKm = entity.EstimatedDiameterMinKm,
            EstimatedDiameterMaxKm = entity.EstimatedDiameterMaxKm,
            IsPotentiallyHazardous = entity.IsPotentiallyHazardous,
            IsSentryObject = entity.IsSentryObject
        };
    }
}