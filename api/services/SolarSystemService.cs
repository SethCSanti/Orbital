using Orbital.Api.Data;
using Orbital.Api.Models.DTOs;
using Orbital.Api.Results;

namespace Orbital.Api.Services;

public interface ISolarSystemService
{
    /// <summary>Calculates heliocentric positions for the eight planets at a UTC instant.</summary>
    Task<Result<IEnumerable<PlanetPositionDto>>> GetPositions(DateTimeOffset? at = null);
}

public class SolarSystemService(OrbitalDbContext context) : BaseService(context), ISolarSystemService
{
    private static readonly DateTimeOffset J2000 =
        new(2000, 1, 1, 12, 0, 0, TimeSpan.Zero);

    // Approximate J2000 orbital elements. The low-precision model is intended for visualization,
    // not navigation; all distances are AU and angles are degrees.
    private static readonly OrbitalElements[] Planets =
    [
        new("Mercury", 0.38709927, 0.20563593, 7.00497902, 252.250324, 77.457796, 48.330766, 87.9691),
        new("Venus", 0.72333566, 0.00677672, 3.39467605, 181.9790995, 131.602467, 76.679843, 224.701),
        new("Earth", 1.00000261, 0.01671123, -0.00001531, 100.4645717, 102.9376819, 0.0, 365.256),
        new("Mars", 1.52371034, 0.09339410, 1.84969142, -4.55343205, -23.94362959, 49.55953891, 686.98),
        new("Jupiter", 5.20288700, 0.04838624, 1.30439695, 34.39644051, 14.72847983, 100.4739091, 4332.589),
        new("Saturn", 9.53667594, 0.05386179, 2.48599187, 49.95424423, 92.59887831, 113.6624245, 10759.22),
        new("Uranus", 19.18916464, 0.04725744, 0.77263783, 313.2381045, 170.9542763, 74.01692503, 30688.5),
        new("Neptune", 30.06992276, 0.00859048, 1.77004347, -55.12002969, 44.96476227, 131.7842257, 60182)
    ];

    public Task<Result<IEnumerable<PlanetPositionDto>>> GetPositions(DateTimeOffset? at = null)
    {
        var timestamp = (at ?? DateTimeOffset.UtcNow).ToUniversalTime();
        var daysSinceJ2000 = (timestamp - J2000).TotalDays;
        var positions = Planets
            .Select(planet => CalculatePosition(planet, daysSinceJ2000))
            .ToList();

        return Task.FromResult(Result<IEnumerable<PlanetPositionDto>>.Success(positions));
    }

    // Solves Kepler's equation and rotates the orbital-plane coordinates into the ecliptic frame.
    private static PlanetPositionDto CalculatePosition(OrbitalElements planet, double daysSinceJ2000)
    {
        var meanAnomaly = DegreesToRadians(NormalizeDegrees(
            planet.MeanLongitude - planet.LongitudeOfPerihelion
            + daysSinceJ2000 / planet.OrbitalPeriodDays * 360.0));
        var eccentricAnomaly = meanAnomaly;

        for (var iteration = 0; iteration < 8; iteration++)
        {
            eccentricAnomaly -= (eccentricAnomaly - planet.Eccentricity * Math.Sin(eccentricAnomaly) - meanAnomaly)
                / (1 - planet.Eccentricity * Math.Cos(eccentricAnomaly));
        }

        var trueAnomaly = 2 * Math.Atan2(
            Math.Sqrt(1 + planet.Eccentricity) * Math.Sin(eccentricAnomaly / 2),
            Math.Sqrt(1 - planet.Eccentricity) * Math.Cos(eccentricAnomaly / 2));
        var radius = planet.SemiMajorAxis * (1 - planet.Eccentricity * Math.Cos(eccentricAnomaly));
        var inclination = DegreesToRadians(planet.Inclination);
        var longitudeOfAscendingNode = DegreesToRadians(planet.LongitudeOfAscendingNode);
        var argumentOfPerihelion = DegreesToRadians(
            planet.LongitudeOfPerihelion - planet.LongitudeOfAscendingNode);
        var argumentOfLatitude = trueAnomaly + argumentOfPerihelion;

        var x = radius * (
            Math.Cos(longitudeOfAscendingNode) * Math.Cos(argumentOfLatitude)
            - Math.Sin(longitudeOfAscendingNode) * Math.Sin(argumentOfLatitude) * Math.Cos(inclination));
        var y = radius * (
            Math.Sin(longitudeOfAscendingNode) * Math.Cos(argumentOfLatitude)
            + Math.Cos(longitudeOfAscendingNode) * Math.Sin(argumentOfLatitude) * Math.Cos(inclination));
        var z = radius * Math.Sin(argumentOfLatitude) * Math.Sin(inclination);

        return new PlanetPositionDto(planet.Name, x, y, z, planet.OrbitalPeriodDays);
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

    private static double NormalizeDegrees(double degrees)
    {
        var normalized = degrees % 360.0;
        return normalized < 0 ? normalized + 360.0 : normalized;
    }

    private sealed record OrbitalElements(
        string Name,
        double SemiMajorAxis,
        double Eccentricity,
        double Inclination,
        double MeanLongitude,
        double LongitudeOfPerihelion,
        double LongitudeOfAscendingNode,
        double OrbitalPeriodDays);
}
