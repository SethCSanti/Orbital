using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record CrewMemberResponse
{
    [JsonPropertyName("astronaut")]
    public AstronautApiResponse Astronaut { get; init; } = new();
}