import type { ApodEntry } from "@/types/apod";
import type { Asteroid } from "@/types/asteroid";
import type { Astronaut } from "@/types/astronaut";
import type { Exoplanet } from "@/types/exoplanet";
import type { ExoplanetFilters } from "@/types/exoplanetFilters";
import type { IssPositionUpdate } from "@/types/iss";
import type { Launch } from "@/types/launch";
import type { Mission } from "@/types/mission";
import type { MissionFilters } from "@/types/missionFilters";
import type { Rocket } from "@/types/rocket";
import type { SpaceStation } from "@/types/spaceStation";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5110";

// Mirrors api/result/Result.cs, as serialized by System.Text.Json (camelCase web defaults)
interface ApiResult<T> {
  isSuccess: boolean;
  value: T | null;
  error: string | null;
}

export class ApiError extends Error {
  status?: number;

  constructor(message: string, status?: number) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

function buildQuery(params: object): string {
  const search = new URLSearchParams();

  for (const [key, value] of Object.entries(params) as [string, string | number | null | undefined][]) {
    if (value !== undefined && value !== null && value !== "") {
      search.set(key, String(value));
    }
  }

  const query = search.toString();
  return query ? `?${query}` : "";
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let res: Response;

  try {
    res = await fetch(`${API_BASE_URL}${path}`, {
      ...init,
      headers: {
        "Content-Type": "application/json",
        ...init?.headers,
      },
    });
  } catch {
    throw new ApiError(`Could not reach the API at ${API_BASE_URL}. Is it running?`);
  }

  if (!res.ok) {
    throw new ApiError(`Request to ${path} failed: ${res.status} ${res.statusText}`, res.status);
  }

  const body = (await res.json()) as ApiResult<T>;

  if (!body.isSuccess || body.value === null || body.value === undefined) {
    throw new ApiError(body.error ?? `Request to ${path} did not return a value`, res.status);
  }

  return body.value;
}

export const api = {
  apod: {
    latest: () => request<ApodEntry>("/api/apod/latest"),
    byDate: (date: string) => request<ApodEntry>(`/api/apod/${date}`),
  },

  asteroids: {
    feed: () => request<Asteroid[]>("/api/asteroids"),
  },

  astronauts: {
    all: () => request<Astronaut[]>("/api/astronauts"),
    byId: (id: number) => request<Astronaut>(`/api/astronauts/${id}`),
  },

  exoplanets: {
    all: (filters: ExoplanetFilters = {}) =>
      request<Exoplanet[]>(`/api/exoplanets${buildQuery(filters)}`),
  },

  iss: {
    position: () => request<IssPositionUpdate>("/api/iss/position"),
  },

  launches: {
    upcoming: (rocketName?: string) =>
      request<Launch[]>(`/api/launch/upcoming${buildQuery({ rocketName })}`),
    past: (rocketName?: string) =>
      request<Launch[]>(`/api/launch/past${buildQuery({ rocketName })}`),
  },

  missions: {
    all: (filters: MissionFilters = {}) =>
      request<Mission[]>(`/api/missions${buildQuery(filters)}`),
  },

  rockets: {
    all: () => request<Rocket[]>("/api/rockets"),
    byName: (name: string) => request<Rocket>(`/api/rockets/${encodeURIComponent(name)}`),
    compare: (names: string[]) =>
      request<Rocket[]>("/api/rockets/compare", {
        method: "POST",
        body: JSON.stringify(names),
      }),
  },

  spaceStations: {
    all: () => request<SpaceStation[]>("/api/spacestation"),
    byId: (id: number) => request<SpaceStation>(`/api/spacestation/${id}`),
  },
};