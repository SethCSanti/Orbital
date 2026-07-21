namespace Orbital.Api.Models.Entities;

public class Asteroid
{
    public int Id { get; set; }
    public string NeoReferenceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string NasaJplUrl { get; set; } = string.Empty;
    public decimal AbsoluteMagnitudeH { get; set; }
    public decimal EstimatedDiameterMinKm { get; set; }
    public decimal EstimatedDiameterMaxKm { get; set; }
    public bool IsPotentiallyHazardous { get; set; }
    public bool IsSentryObject { get; set; }
}