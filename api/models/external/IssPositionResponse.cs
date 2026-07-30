using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record IssPositionResponse
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; init; }
    [JsonPropertyName("iss_position")]
    public IssPosition Position { get; init; } = new IssPosition();
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}

public record IssPosition
{
    [JsonPropertyName("latitude")]
    public string Latitude { get; init; } = string.Empty;
    [JsonPropertyName("longitude")]
    public string Longitude { get; init; } = string.Empty;
}

public record IssPositionUpdate
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}