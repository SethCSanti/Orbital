namespace Orbital.Api.Models.DTOs;

// Coordinates are heliocentric ecliptic positions measured in astronomical units (AU).
public record PlanetPositionDto(
    string Name,
    double X,
    double Y,
    double Z,
    double OrbitalPeriodDays);
