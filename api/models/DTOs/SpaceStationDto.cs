using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record SpaceStationDto(
    string Name,
    string Status,
    string Type,
    DateOnly Founded,
    DateOnly? Deorbited,
    string Description,
    string Orbit,
    string ImageUrl)
{
    public SpaceStationDto() : this(
        string.Empty, string.Empty, string.Empty, default, null, string.Empty, string.Empty, string.Empty)
    { }

    public SpaceStationDto(SpaceStation entity) : this(
        entity.Name,
        entity.Status,
        entity.Type,
        entity.Founded,
        entity.Deorbited,
        entity.Description,
        entity.Orbit,
        entity.ImageUrl)
    { }
}
