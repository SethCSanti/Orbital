using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Models.Mappings;

public static class LaunchMappings
{
    public static LaunchDto ToDto(this Launch entity)
    {
        return new LaunchDto
        {
            Name = entity.Name,
            StatusName = entity.StatusName,
            Net = entity.Net,
            WindowStart = entity.WindowStart,
            WindowEnd = entity.WindowEnd,
            Probability = entity.Probability,
            HoldReason = entity.HoldReason,
            FailReason = entity.FailReason,
            Hashtag = entity.Hashtag,
            RocketName = entity.Rocket.Name,
            MissionName = entity.Mission.Name,
            OrbitAbbrev = entity.Mission.OrbitAbbrev,
            CrewNames = entity.Crew.Select(a => a.Name).ToList(),
        };
    }
}