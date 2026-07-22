using Orbital.Api.Models.DTOs;
using Orbital.Api.Models.Entities;
namespace Orbital.Api.Models.Mappings;

public static class RocketMappings
{
    public static RocketDto ToDto(this Rocket entity)
    {
        // your mapping code goes here
        return new RocketDto
        {
            Name = entity.Name,
            FullName = entity.FullName,
            Family = entity.Family,
            Active = entity.Active,
            Reusable = entity.Reusable,
            Description = entity.Description,
            Variant = entity.Variant,
            Length = entity.Length,
            Diameter = entity.Diameter,
            MaidenFlight = entity.MaidenFlight,
            LaunchCost = entity.LaunchCost,
            LaunchMass = entity.LaunchMass,
            LeoCapacity = entity.LeoCapacity,
            GtoCapacity = entity.GtoCapacity,
            ImageUrl = entity.ImageUrl,
            WikiUrl = entity.WikiUrl,
            TotalLaunchCount = entity.TotalLaunchCount,
            SuccessfulLaunchCount = entity.SuccessfulLaunchCount,
            FailedLaunchCount = entity.FailedLaunchCount
        };
    }
}