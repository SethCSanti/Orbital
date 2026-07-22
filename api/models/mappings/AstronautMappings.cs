using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Models.Mappings;

public static class AstronautMappings
{
    public static AstronautDto ToDto(this Astronaut entity)
    {
        return new AstronautDto
        {
            Name = entity.Name,
            Nationality = entity.Nationality,
            DateOfBirth = entity.DateOfBirth,
            DateOfDeath = entity.DateOfDeath,
            Biography = entity.Biography,
            ProfileImageUrl = entity.ProfileImageUrl,
            WikipediaUrl = entity.WikipediaUrl,
            FlightsCount = entity.FlightsCount
        };
    }
}