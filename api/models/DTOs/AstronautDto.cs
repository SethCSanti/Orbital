using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record AstronautDto(
    string Name,
    string? Nationality,
    DateOnly? DateOfBirth,
    DateOnly? DateOfDeath,
    string? Biography,
    string? ProfileImageUrl,
    string? WikipediaUrl,
    int FlightsCount)
{
    public AstronautDto() : this(string.Empty, null, null, null, null, null, null, 0) { }

    public AstronautDto(Astronaut entity) : this(
        entity.Name,
        entity.Nationality,
        entity.DateOfBirth,
        entity.DateOfDeath,
        entity.Biography,
        entity.ProfileImageUrl,
        entity.WikipediaUrl,
        entity.FlightsCount)
    { }
}
