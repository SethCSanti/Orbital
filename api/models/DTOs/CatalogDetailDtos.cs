using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record RelatedLaunchDto(
    int Id,
    string ExternalId,
    string Name,
    DateTimeOffset Net,
    string StatusName,
    string RocketName,
    string MissionName,
    IReadOnlyList<string> CrewNames)
{
    public RelatedLaunchDto(Launch entity) : this(
        entity.Id,
        entity.ExternalId,
        entity.Name,
        entity.Net,
        entity.StatusName,
        entity.Rocket.Name,
        entity.Mission.Name,
        entity.Crew.Select(crew => crew.Name).ToList())
    { }
}

public record AstronautDetailDto(AstronautDto Astronaut, IReadOnlyList<RelatedLaunchDto> Launches);
public record RocketDetailDto(RocketDto Rocket, IReadOnlyList<RelatedLaunchDto> Launches);
public record MissionDetailDto(MissionDto Mission, IReadOnlyList<RelatedLaunchDto> Launches);
