export interface Asteroid {
    neoReferenceId: string,
    name: string,
    nasaJplUrl: string,
    absoluteMagnitudeH: number,
    estimatedDiameterMinKm: number,
    estimatedDiameterMaxKm: number,
    isPotentiallyHazardous: boolean,
    isSentryObject: boolean,
    closeApproachDate: string,
    relativeVelocityKph: number,
    missDistanceKm: number
}