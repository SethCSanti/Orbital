using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record MissionDto(
    string Name,
    string Description,
    string Type,
    string? LaunchDesignator,
    string OrbitName,
    string OrbitAbbrev)
{
    public MissionDto() : this(string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty) { }

    public MissionDto(Mission entity) : this(
        entity.Name,
        entity.Description,
        entity.Type,
        entity.LaunchDesignator,
        entity.OrbitName,
        entity.OrbitAbbrev)
    { }
}
