using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record AstronautDto(
    int Id,
    string? SourceId,
    string? SourceUrl,
    string Name,
    string? Nationality,
    DateOnly? DateOfBirth,
    DateOnly? DateOfDeath,
    string? Biography,
    string? ProfileImageUrl,
    string? WikipediaUrl,
    int FlightsCount)
{
    public AstronautDto() : this(0, null, null, string.Empty, null, null, null, null, null, null, 0) { }

    public AstronautDto(Astronaut entity) : this(
        entity.Id,
        entity.SourceId,
        entity.SourceUrl,
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
