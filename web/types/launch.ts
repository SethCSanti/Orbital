export interface Launch {
    id?: number,
    externalId?: string,
    sourceUrl?: string | null,
    name: string,
    statusName: string,
    net: string,
    windowStart: string,
    windowEnd: string,
    probability: number | null,
    holdReason: string | null,
    failReason: string | null,
    hashtag: string | null,
    rocketName: string,
    missionName: string,
    orbitAbbrev: string,
    crewNames: string[]
}
