namespace Orbital.Api.Models.DTOs;

public class MissionDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? LaunchDesignator { get; set; }
    public string OrbitName { get; set; } = string.Empty;
    public string OrbitAbbrev { get; set; } = string.Empty;
}