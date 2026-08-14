using Orbital.Api.Models.Entities;

namespace Orbital.Api.Models.DTOs;

public record ApodEntryDto(
    DateOnly Date,
    string Title,
    string Explanation,
    string Url,
    string MediaType,
    string? HdUrl,
    string? Copyright)
{
    public ApodEntryDto() : this(default, string.Empty, string.Empty, string.Empty, string.Empty, null, null) { }

    public ApodEntryDto(ApodEntry entity) : this(
        entity.Date,
        entity.Title,
        entity.Explanation,
        entity.Url,
        entity.MediaType,
        entity.HdUrl,
        entity.Copyright)
    { }
}
