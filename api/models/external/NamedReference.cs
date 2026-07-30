using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record NamedReference
{
    [JsonPropertyName("id")]
    public int? Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;
}