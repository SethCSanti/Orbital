namespace Orbital.Api.Models.Entities;

public class Mission
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? LaunchDesignator { get; set; }
    public string OrbitName { get; set; } = string.Empty;
    public string OrbitAbbrev { get; set; } = string.Empty;
}