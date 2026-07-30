using System.Text.Json.Serialization;
namespace Orbital.Api.Models.External;

public record TleObject
{
    public string Name { get; init; } = string.Empty;
    public string Line1 { get; init; } = string.Empty;
    public string Line2 { get; init; } = string.Empty;
}