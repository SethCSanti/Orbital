namespace Orbital.Api.Models.DTOs;

public class ApodEntryDto
{
    public DateOnly Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public string? HdUrl { get; set; }
    public string? Copyright { get; set; }
}