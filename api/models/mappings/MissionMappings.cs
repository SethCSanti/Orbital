using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Models.Mappings;

public static class MissionMappings
{
    public static MissionDto ToDto(this Mission entity)
    {
        // your mapping code goes here
        return new MissionDto
        {
            Name = entity.Name,
            Description = entity.Description,
            Type = entity.Type,
            LaunchDesignator = entity.LaunchDesignator,
            OrbitName = entity.OrbitName,
            OrbitAbbrev = entity.OrbitAbbrev
        };
    }
}