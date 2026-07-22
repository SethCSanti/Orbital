using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Models.Mappings;

public static class SpaceStationMappings
{
    public static SpaceStationDto ToDto(this SpaceStation entity)
    {
        // your mapping code goes here
        return new SpaceStationDto
        {
            Name = entity.Name,
            Status = entity.Status,
            Type = entity.Type,
            Founded = entity.Founded,
            Deorbited = entity.Deorbited,
            Description = entity.Description,
            Orbit = entity.Orbit,
            ImageUrl = entity.ImageUrl
        };
    }
}