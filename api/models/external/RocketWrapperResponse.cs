using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record RocketWrapperResponse
{
    [JsonPropertyName("configuration")]
    public RocketConfigurationResponse Configuration { get; init; } = new();
}