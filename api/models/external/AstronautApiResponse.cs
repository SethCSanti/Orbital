using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record AstronautApiResponse
{
    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("url")]
    public string? SourceUrl { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("nationality")]
    public string? Nationality { get; init; }

    [JsonPropertyName("date_of_birth")]
    public DateOnly? DateOfBirth { get; init; }

    [JsonPropertyName("date_of_death")]
    public DateOnly? DateOfDeath { get; init; }

    [JsonPropertyName("bio")]
    public string? Biography { get; init; }

    [JsonPropertyName("profile_image")]
    public string? ProfileImageUrl { get; init; }

    [JsonPropertyName("wiki")]
    public string? WikipediaUrl { get; init; }

    [JsonPropertyName("flights_count")]
    public int? FlightsCount { get; init; }
}
