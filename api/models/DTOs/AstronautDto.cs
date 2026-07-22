namespace Orbital.Api.Models.DTOs;

public class AstronautDto
{
    public string Name { get; set; } = string.Empty;
    /* the astronaut's nationality */
    public string? Nationality { get; set; }
    /* the astronaut's date of birth in YYYY-MM-DD format */
    public DateOnly? DateOfBirth { get; set; }
    /* the astronaut's date of death in YYYY-MM-DD format — 
    conditionally present, only if the astronaut is deceased. */
    public DateOnly? DateOfDeath { get; set; }
    /* the astronaut's biography */  
    public string? Biography { get; set; }
    /* the astronaut's profile image URL */
    public string? ProfileImageUrl { get; set; }
    /* the astronaut's Wikipedia page URL */
    public string? WikipediaUrl { get; set; }
    /* integer, always present */
    public int FlightsCount { get; set; }
}