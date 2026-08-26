using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record RocketDto(
    int Id,
    string? SourceId,
    string? SourceUrl,
    string Name,
    string FullName,
    string Family,
    bool Active,
    bool Reusable,
    string Description,
    string Variant,
    decimal Length,
    decimal Diameter,
    DateOnly MaidenFlight,
    decimal? LaunchCost,
    decimal LaunchMass,
    decimal LeoCapacity,
    decimal? GtoCapacity,
    string ImageUrl,
    string WikiUrl,
    int TotalLaunchCount,
    int SuccessfulLaunchCount,
    int FailedLaunchCount)
{
    public RocketDto() : this(
        0, null, null, string.Empty, string.Empty, string.Empty, false, false, string.Empty, string.Empty,
        0, 0, default, null, 0, 0, null, string.Empty, string.Empty, 0, 0, 0)
    { }

    public RocketDto(Rocket entity) : this(
        entity.Id,
        entity.SourceId,
        entity.SourceUrl,
        entity.Name,
        entity.FullName,
        entity.Family,
        entity.Active,
        entity.Reusable,
        entity.Description,
        entity.Variant,
        entity.Length,
        entity.Diameter,
        entity.MaidenFlight,
        entity.LaunchCost,
        entity.LaunchMass,
        entity.LeoCapacity,
        entity.GtoCapacity,
        entity.ImageUrl,
        entity.WikiUrl,
        entity.TotalLaunchCount,
        entity.SuccessfulLaunchCount,
        entity.FailedLaunchCount)
    { }
}
