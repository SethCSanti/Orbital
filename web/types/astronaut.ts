export interface Astronaut {
    name: string,
    nationality: string | null,
    dateOfBirth: string | null,
    dateOfDeath: string | null,
    biography: string | null,
    profileImageUrl: string | null,
    wikipediaUrl : string | null,
    flightsCount: number
}