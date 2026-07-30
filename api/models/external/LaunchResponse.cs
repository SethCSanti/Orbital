using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record LaunchResponse
{
    [JsonPropertyName("id")]
    public string ExternalId { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public NamedReference? Status { get; init; }

    [JsonPropertyName("net")]
    public DateTimeOffset Net { get; init; }

    [JsonPropertyName("window_start")]
    public DateTimeOffset WindowStart { get; init; }

    [JsonPropertyName("window_end")]
    public DateTimeOffset WindowEnd { get; init; }

    [JsonPropertyName("probability")]
    public int? Probability { get; init; }

    [JsonPropertyName("holdreason")]
    public string? HoldReason { get; init; }

    [JsonPropertyName("failreason")]
    public string? FailReason { get; init; }

    [JsonPropertyName("hashtag")]
    public string? Hashtag { get; init; }

    [JsonPropertyName("rocket")]
    public RocketWrapperResponse Rocket { get; init; } = new();

    [JsonPropertyName("mission")]
    public MissionResponse Mission { get; init; } = new();

    [JsonPropertyName("crew")]
    public List<CrewMemberResponse> Crew { get; init; } = new();
}