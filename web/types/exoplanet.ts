export interface Exoplanet {
    planetName: string,
    hostName: string,
    discoveryYear: number,
    discoveryMethod: string,
    discoveryFacility: string,
    orbitalPeriodDays: number | null,
    radiusEarthRadii: number | null,
    massEarthMasses: number | null,
    semiMajorAxisAu: number | null 
}