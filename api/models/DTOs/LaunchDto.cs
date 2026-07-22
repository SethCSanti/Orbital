namespace Orbital.Api.Models.DTOs;

public class LaunchDto
{
    public string Name { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public DateTimeOffset Net { get; set; }
    public DateTimeOffset WindowStart { get; set; }
    public DateTimeOffset WindowEnd { get; set; }
    public int? Probability { get; set; }
    public string? HoldReason { get; set; }
    public string? FailReason { get; set; }
    public string? Hashtag { get; set; }
    public string RocketName { get; set; } = string.Empty;
    public string MissionName { get; set; } = string.Empty;
    public string OrbitAbbrev { get; set; } = string.Empty;
    public List<string> CrewNames { get; set; } = new List<string>();
}