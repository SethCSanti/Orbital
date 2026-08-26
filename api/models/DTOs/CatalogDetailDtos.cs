using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record RelatedLaunchDto(
    int Id,
    string ExternalId,
    string Name,
    DateTimeOffset Net,
    string StatusName,
    string RocketName,
    string MissionName)
{
    public RelatedLaunchDto(Launch entity) : this(
        entity.Id,
        entity.ExternalId,
        entity.Name,
        entity.Net,
        entity.StatusName,
        entity.Rocket.Name,
        entity.Mission.Name)
    { }
}

public record RocketDetailDto(RocketDto Rocket, IReadOnlyList<RelatedLaunchDto> Launches);
public record MissionDetailDto(MissionDto Mission, IReadOnlyList<RelatedLaunchDto> Launches);
