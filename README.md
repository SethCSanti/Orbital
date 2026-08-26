# Orbital

Orbital is a full-stack space dashboard built with .NET 10, Next.js 15, TypeScript, PostgreSQL, Redis, Hangfire, SignalR, Three.js, and CesiumJS.

## Features

- Live ISS tracking with a Cesium globe, ground track, and connection status
- Historical launches, missions, and rockets with paginated catalogs and detail views
- Upcoming launch countdown and launch history
- Interactive solar-system visualization
- Space-station map and station catalog
- Near-Earth asteroid feed and visualization
- NASA Astronomy Picture of the Day archive
- Rocket comparison by exact database record

The frontend keeps Cesium and Three.js route-specific so ordinary catalog pages stay light. Navigation prefetch is disabled for catalog and visualization links; heavy visualizations load only when opened.

## Repository layout

```text
Orbital/
├── api/                 # ASP.NET Core API, EF Core models, jobs, and migrations
├── web/                 # Next.js App Router frontend
├── docker-compose.yml   # PostgreSQL, Redis, and local services
└── README.md
```

## Local development

Start the dependencies and API, then run the frontend:

```sh
docker compose up
cd api && dotnet run
cd ../web && bun install && bun run dev
```

The API reads its database and upstream-service configuration from its normal .NET configuration files. The frontend runs at `http://localhost:3000` and proxies API requests to the local API.

## Environment variables

Copy `web/.env.example` to `web/.env.local` and set `NEXT_PUBLIC_CESIUM_ION_TOKEN` for the ISS globe. A token can be created at [Cesium ion tokens](https://ion.cesium.com/tokens). `.env.local` is ignored by git.

The API also requires its configured PostgreSQL, Redis, and SpaceDevs/NASA settings. See `api/appsettings.json` and the API project configuration for the available server-side values.

## Data and synchronization

PostgreSQL is the source of truth and Redis is the response cache. Hangfire refreshes current launches, missions, rockets, stations, ISS data, asteroids, and APOD. Historical launch-derived catalog backfill is paginated, resumable, and exposes status through `/api/catalog/status`; partial results remain visible during collection.

The `RemoveHeavyFeatures` EF migration intentionally drops the retired Astronaut and Exoplanet tables and the astronaut-to-launch join table. Back up the database before applying migrations in a deployment environment.

## API areas

- `/api/launches` — upcoming and historical launch data
- `/api/missions` — paginated mission catalog and details
- `/api/rockets` — paginated rocket catalog, details, and comparisons
- `/api/spacestations` — station records
- `/api/iss` — ISS snapshots and live hub data
- `/api/asteroids` — near-Earth object data
- `/api/apod` — NASA APOD data
- `/api/catalog/status` — historical catalog backfill status

## Verification

```sh
cd web && bun run test && bun run build
cd ../api && dotnet build
```
