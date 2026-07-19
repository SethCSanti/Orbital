namespace Orbital.Api.Models.Entities;

public class ApodEntry
{
    /* primary key */
    public int Id { get; set; }
    /* a string in YYYY-MM-DD format indicating the date of the APOD image — always present. */
    public DateOnly Date { get; set; }
    /* always present, not documented as ever missing. */
    public string Title { get; set; } = string.Empty;
    /* the supplied text explanation of the image — always present. */
    public string Explanation { get; set; } = string.Empty;
    /* always present, not documented as ever missing. */
    public string Url { get; set; } = string.Empty;
    /* the type of media returned, either 'image' or 'video' depending on content — always present */
    public string MediaType { get; set; } = string.Empty;
    /* returned regardless of the 'hd' param setting but will be omitted in the response 
    if it does not exist originally at APOD — so this one is conditionally missing, not guaranteed. */
    public string? HdUrl { get; set; }
    /* not in the official README's field list at all — it's an unofficial/informal field that only 
    shows up when NASA has a credited photographer. The guptarohit/apod-api mirror repo's example 
    response even shows it embedded awkwardly inside the title string in older data, which tells you 
    it's inconsistent — treat it as optional. */
    public string? Copyright { get; set; }
}