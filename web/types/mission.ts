export interface Mission {
    id?: number,
    sourceId?: string | null,
    sourceUrl?: string | null,
    name: string,
    description: string,
    type: string,
    launchDesignator: string | null,
    orbitName: string,
    orbitAbbrev : string
}
