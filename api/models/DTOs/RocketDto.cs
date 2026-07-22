namespace Orbital.Api.Models.DTOs;

public class RocketDto
{
    public string Name { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Family { get; set; } = string.Empty;
    public bool Active { get; set; }
    public bool Reusable { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Variant { get; set; } = string.Empty;
    public decimal Length { get; set; }
    public decimal Diameter { get; set; }
    public DateOnly MaidenFlight { get; set; }
    public decimal? LaunchCost { get; set; }
    public decimal LaunchMass { get; set; }
    public decimal LeoCapacity { get; set; }
    public decimal? GtoCapacity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string WikiUrl { get; set; } = string.Empty;
    public int TotalLaunchCount { get; set; }
    public int SuccessfulLaunchCount { get; set; }
    public int FailedLaunchCount { get; set; }
}