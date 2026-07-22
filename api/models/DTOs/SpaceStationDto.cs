namespace Orbital.Api.Models.DTOs;

public class SpaceStationDto
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateOnly Founded { get; set; }
    public DateOnly? Deorbited { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Orbit { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}