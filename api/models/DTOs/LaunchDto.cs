using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record LaunchDto(
    int Id,
    string ExternalId,
    string? SourceUrl,
    string Name,
    string StatusName,
    DateTimeOffset Net,
    DateTimeOffset WindowStart,
    DateTimeOffset WindowEnd,
    int? Probability,
    string? HoldReason,
    string? FailReason,
    string? Hashtag,
    string RocketName,
    string MissionName,
    string OrbitAbbrev,
    List<string> CrewNames)
{
    public LaunchDto() : this(
        0, string.Empty, null, string.Empty, string.Empty, default, default, default, null, null, null, null,
        string.Empty, string.Empty, string.Empty, new List<string>())
    { }

    public LaunchDto(Launch entity) : this(
        entity.Id,
        entity.ExternalId,
        entity.SourceUrl,
        entity.Name,
        entity.StatusName,
        entity.Net,
        entity.WindowStart,
        entity.WindowEnd,
        entity.Probability,
        entity.HoldReason,
        entity.FailReason,
        entity.Hashtag,
        entity.Rocket.Name,
        entity.Mission.Name,
        entity.Mission.OrbitAbbrev,
        entity.Crew.Select(astronaut => astronaut.Name).ToList())
    { }
}
