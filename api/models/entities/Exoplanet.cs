namespace Orbital.Api.Models.Entities;

public class Exoplanet
{
    public int Id { get; set; }
    public string PlanetName { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public int DiscoveryYear { get; set; }
    public string DiscoveryMethod { get; set; } = string.Empty;
    public string DiscoveryFacility { get; set; } = string.Empty;
    public decimal? OrbitalPeriodDays { get; set; }
    public decimal? RadiusEarthRadii { get; set; }
    public decimal? MassEarthMasses { get; set; }
    public decimal? SemiMajorAxisAu { get; set; }
}