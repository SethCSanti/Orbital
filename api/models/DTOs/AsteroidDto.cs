using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record AsteroidDto(
    string NeoReferenceId,
    string Name,
    string NasaJplUrl,
    decimal AbsoluteMagnitudeH,
    decimal EstimatedDiameterMinKm,
    decimal EstimatedDiameterMaxKm,
    bool IsPotentiallyHazardous,
    bool IsSentryObject,
    DateOnly CloseApproachDate,
    decimal RelativeVelocityKph,
    decimal MissDistanceKm)
{
    public AsteroidDto() : this(
        string.Empty, string.Empty, string.Empty, 0, 0, 0, false, false, default, 0, 0)
    { }

    public AsteroidDto(Asteroid entity) : this(
        entity.NeoReferenceId,
        entity.Name,
        entity.NasaJplUrl,
        entity.AbsoluteMagnitudeH,
        entity.EstimatedDiameterMinKm,
        entity.EstimatedDiameterMaxKm,
        entity.IsPotentiallyHazardous,
        entity.IsSentryObject,
        entity.CloseApproachDate,
        entity.RelativeVelocityKph,
        entity.MissDistanceKm)
    { }
}
