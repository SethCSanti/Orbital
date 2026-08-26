import type { ApodEntry } from "@/types/apod";
import type { Asteroid } from "@/types/asteroid";
import type { IssPositionUpdate } from "@/types/iss";
import type { Launch } from "@/types/launch";
import type { Mission } from "@/types/mission";
import type { MissionFilters } from "@/types/missionFilters";
import type { Rocket } from "@/types/rocket";
import type { SpaceStation } from "@/types/spaceStation";
import type { PlanetPosition } from "@/types/solarSystem";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5110";

interface ApiResult<T> { isSuccess: boolean; value: T | null; error: string | null }

export interface PageResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  filterMetadata?: Record<string, string[]>;
}

export interface RelatedLaunch {
  id: number;
  externalId: string;
  name: string;
  net: string;
  statusName: string;
  rocketName: string;
  missionName: string;
}

export interface CatalogStatus {
  catalog: string;
  status: "pending" | "running" | "partial" | "complete" | string;
  currentPage: number;
  pageSize: number;
  totalAvailable: number | null;
  recordsImported: number;
  lastStartedAt: string | null;
  lastCompletedAt: string | null;
  updatedAt: string;
  lastError: string | null;
}

export class ApiError extends Error {
  status?: number;
  constructor(message: string, status?: number) { super(message); this.name = "ApiError"; this.status = status; }
}

function buildQuery(params: object): string {
  const search = new URLSearchParams();
  for (const [key, value] of Object.entries(params) as [string, string | number | null | undefined][]) {
    if (value !== undefined && value !== null && value !== "") search.set(key, String(value));
  }
  const query = search.toString();
  return query ? `?${query}` : "";
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers: { "Content-Type": "application/json", ...init?.headers },
    });
  } catch (error) {
    if (error instanceof DOMException && error.name === "AbortError") throw error;
    throw new ApiError(`Could not reach the API at ${API_BASE_URL}. Is it running?`);
  }
  if (!response.ok) throw new ApiError(`Request to ${path} failed: ${response.status} ${response.statusText}`, response.status);
  const body = (await response.json()) as ApiResult<T>;
  if (!body.isSuccess || body.value === null || body.value === undefined) throw new ApiError(body.error ?? `Request to ${path} did not return a value`, response.status);
  return body.value;
}

type Signal = AbortSignal | undefined;
const withSignal = (signal: Signal): RequestInit | undefined => signal ? { signal } : undefined;

export const api = {
  apod: {
    latest: (signal?: Signal) => request<ApodEntry>("/api/apod/latest", withSignal(signal)),
    byDate: (date: string, signal?: Signal) => request<ApodEntry>(`/api/apod/${date}`, withSignal(signal)),
  },
  asteroids: { feed: (signal?: Signal) => request<Asteroid[]>("/api/asteroids", withSignal(signal)) },
  iss: { position: (signal?: Signal) => request<IssPositionUpdate>("/api/iss/position", withSignal(signal)) },
  launches: {
    upcoming: (rocketName?: string, signal?: Signal) => request<Launch[]>(`/api/launch/upcoming${buildQuery({ rocketName })}`, withSignal(signal)),
    past: (rocketName?: string, signal?: Signal) => request<Launch[]>(`/api/launch/past${buildQuery({ rocketName })}`, withSignal(signal)),
  },
  missions: {
    all: (filters: MissionFilters = {}, signal?: Signal) => request<PageResult<Mission>>(`/api/missions${buildQuery(filters)}`, withSignal(signal)),
    byId: (id: number, signal?: Signal) => request<{ mission: Mission; launches: RelatedLaunch[] }>(`/api/missions/${id}`, withSignal(signal)),
  },
  rockets: {
    all: (params: { page?: number; pageSize?: number; search?: string } = {}, signal?: Signal) => request<PageResult<Rocket>>(`/api/rockets${buildQuery(params)}`, withSignal(signal)),
    byId: (id: number, signal?: Signal) => request<{ rocket: Rocket; launches: RelatedLaunch[] }>(`/api/rockets/id/${id}`, withSignal(signal)),
    byName: (name: string, signal?: Signal) => request<Rocket>(`/api/rockets/${encodeURIComponent(name)}`, withSignal(signal)),
    compare: (ids: number[], signal?: Signal) => request<Rocket[]>("/api/rockets/compare", { method: "POST", body: JSON.stringify(ids), signal }),
  },
  catalog: { status: (signal?: Signal) => request<CatalogStatus[]>("/api/catalog/status", withSignal(signal)) },
  spaceStations: {
    all: (signal?: Signal) => request<SpaceStation[]>("/api/spacestation", withSignal(signal)),
    byId: (id: number, signal?: Signal) => request<SpaceStation>(`/api/spacestation/${id}`, withSignal(signal)),
  },
  solarSystem: { positions: (at?: string, signal?: Signal) => request<PlanetPosition[]>(`/api/solarSystem/bodies${buildQuery({ at })}`, withSignal(signal)) },
};
