namespace Orbital.Api.Models.DTOs;

public record IssPositionUpdate
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}