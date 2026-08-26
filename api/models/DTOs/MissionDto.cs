using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record MissionDto(
    int Id,
    string? SourceId,
    string? SourceUrl,
    string Name,
    string Description,
    string Type,
    string? LaunchDesignator,
    string OrbitName,
    string OrbitAbbrev)
{
    public MissionDto() : this(0, null, null, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty) { }

    public MissionDto(Mission entity) : this(
        entity.Id,
        entity.SourceId,
        entity.SourceUrl,
        entity.Name,
        entity.Description,
        entity.Type,
        entity.LaunchDesignator,
        entity.OrbitName,
        entity.OrbitAbbrev)
    { }
}
