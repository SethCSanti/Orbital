namespace Orbital.Api.Models.Entities;

public class Launch
{
    public int Id { get; set; }
    public int RocketId { get; set; }
    public Rocket Rocket { get; set; } = null!;
    public ICollection<Astronaut> Crew { get; set; } = new List<Astronaut>();
    public int MissionId { get; set; }
    public Mission Mission { get; set; } = null!;
    public string ExternalId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public DateTimeOffset Net { get; set; }
    public DateTimeOffset WindowStart { get; set; }
    public DateTimeOffset WindowEnd { get; set; }
    public int? Probability { get; set; }
    public string? HoldReason { get; set; }
    public string? FailReason { get; set; }
    public string? Hashtag { get; set; }
}