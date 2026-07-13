# 🚀 Orbital — Space Exploration Portfolio

> A real-time, interactive space exploration dashboard built with .NET 10, Next.js, and TypeScript. Live data from NASA, The Space Devs, and more — zero manual maintenance.

![Status](https://img.shields.io/badge/status-in_development-yellow)
![Backend](https://img.shields.io/badge/backend-.NET_10-512BD4)
![Frontend](https://img.shields.io/badge/frontend-Next.js_+_TypeScript-000000)
![License](https://img.shields.io/badge/license-MIT-blue)

---

## 📑 Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Features](#features)
- [Data Sources](#data-sources)
- [File Structure](#file-structure)
- [Getting Started](#getting-started)
- [Environment Variables](#environment-variables)
- [API Reference](#api-reference)
- [Todo / Roadmap](#todo--roadmap)

---

## Overview

CosmosView is a full-stack portfolio project that visualizes the history and present of space exploration through interactive maps, 3D simulations, real-time tracking, and live data feeds. All data is pulled automatically from public APIs and synced in the background — no manual updates required.

Built to demonstrate:
- Full-stack architecture with a .NET 10 Web API backend and a Next.js 15 frontend
- Real-time data delivery via SignalR WebSockets
- Background job scheduling with Hangfire
- 3D rendering and WebGL globe visualization
- Redis caching and API proxy patterns
- Progressive Web App (PWA) support with offline capability

---

## Tech Stack

### Backend — `CosmosView.Api`
| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| Language | C# |
| Framework | ASP.NET Core Web API |
| Real-time | SignalR |
| Background Jobs | Hangfire |
| Caching | Redis (StackExchange.Redis) |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL |
| API Docs | Swagger / OpenAPI (auto-generated) |
| Auth (future) | ASP.NET Core Identity + JWT |

### Frontend — `CosmosView.Web`
| Layer | Technology |
|---|---|
| Framework | Next.js 15 (App Router) |
| Language | TypeScript |
| Runtime | Bun |
| 3D / WebGL | Three.js + @react-three/fiber |
| Globe | CesiumJS or deck.gl |
| State | Zustand |
| Data Fetching | TanStack Query (React Query) |
| Real-time | SignalR JS client (`@microsoft/signalr`) |
| Styling | Tailwind CSS |
| Charts / Viz | D3.js + Recharts |
| PWA | next-pwa |

### Infrastructure
| Concern | Tool |
|---|---|
| Container | Docker + Docker Compose |
| Reverse Proxy | Nginx |
| Cache | Redis |
| Database | PostgreSQL |
| CI/CD | GitHub Actions |

---

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        CLIENT (Browser)                         │
│                                                                 │
│   Next.js 15 + TypeScript + Three.js + CesiumJS + D3.js        │
│   ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────────┐  │
│   │ 3D Solar │  │  Globe   │  │ Timeline │  │  Dashboard   │  │
│   │  System  │  │  Tracker │  │  /Maps   │  │   Panels     │  │
│   └────┬─────┘  └────┬─────┘  └────┬─────┘  └──────┬───────┘  │
│        └─────────────┴──────────────┴────────────────┘          │
│                         TanStack Query + Zustand                │
│                    HTTP REST  /  SignalR WebSocket               │
└──────────────────────────┬───────────────────────┬─────────────┘
                           │ REST                  │ WS
┌──────────────────────────▼───────────────────────▼─────────────┐
│                    ASP.NET Core Web API (.NET 10)               │
│                                                                 │
│   ┌────────────┐  ┌──────────────┐  ┌────────────────────────┐ │
│   │ Controllers│  │  SignalR     │  │   Hangfire Background  │ │
│   │  /Endpoints│  │  Hubs        │  │   Jobs (scheduled sync)│ │
│   └─────┬──────┘  └──────┬───────┘  └──────────┬─────────────┘ │
│         │                │                      │               │
│   ┌─────▼────────────────▼──────────────────────▼────────────┐ │
│   │               Service Layer                               │ │
│   │  IssService │ LaunchService │ AsteroidService │ ApodService│ │
│   └─────┬──────────────────────────────────────────┬─────────┘ │
│         │                                          │            │
│   ┌─────▼──────────┐                    ┌──────────▼─────────┐ │
│   │  Redis Cache   │                    │  PostgreSQL (EF)   │ │
│   │  (TTL per feed)│                    │  (persisted data)  │ │
│   └────────────────┘                    └────────────────────┘ │
└────────────────────────────┬────────────────────────────────────┘
                             │ HTTP (proxied, keys stay server-side)
┌────────────────────────────▼────────────────────────────────────┐
│                    External Data Sources                        │
│                                                                 │
│  NASA Open API  │  The Space Devs LL2  │  Open Notify          │
│  Celestrak TLE  │  SpaceX API          │  NASA NeoWs            │
│  NASA APOD      │  NASA Exoplanet      │  (more below)          │
└─────────────────────────────────────────────────────────────────┘
```

### Key Architectural Decisions

**API Proxy Pattern** — The .NET backend proxies all third-party API calls. No API keys are ever exposed to the browser. External rate limits are absorbed by the backend cache layer.

**Redis TTL Strategy** — Each data feed has its own cache TTL tuned to how often that data actually changes:
- ISS position → 5 seconds
- Upcoming launches → 15 minutes
- Astronauts in space → 1 hour
- Mission history → 24 hours
- APOD → 24 hours
- Asteroid feed → 1 hour

**SignalR for Live Feeds** — The ISS position hub pushes updates every 5 seconds to all connected clients. Clients subscribe to named groups (e.g. `iss-tracker`, `launch-countdown`) so they only receive relevant pushes.

**Hangfire Background Sync** — Scheduled jobs refresh the PostgreSQL cache from external APIs on a per-feed schedule. The frontend always reads from your own backend, never directly from a third-party.

**URL State for 3D Scene** — The solar system camera position, zoom, and selected body are encoded into URL query params. Sharing a link or navigating back restores the exact view.

**PWA Offline Mode** — Static assets and the most recent API snapshots are cached via a service worker. The solar system, timeline, and mission map remain functional without a connection.

---

## Features

### 🌍 Space Station & Mission Map
- World map of all space stations (active and historical) with click-to-expand detail panels
- Mission browser filterable by: rocket, satellite, crewed, robotic, agency, decade
- Launch site markers with launch count stats
- Data source: The Space Devs Launch Library 2

### ☀️ Interactive 3D Solar System
- Full solar system rendered in Three.js with accurate relative orbital positions (calculated via ephemeris math)
- Click any body to get stats: mass, diameter, moons, distance from Sun, orbital period
- Camera position and selected body persisted in URL query params
- Toggleable: orbits, labels, asteroid belt, scale modes (realistic vs. readable)

### 🛰️ Live ISS Tracker
- Real-time ISS position on a WebGL globe (CesiumJS), updated every 5 seconds via SignalR
- Trailing orbital path showing the last 90 minutes of ground track
- Upcoming ISS visibility windows (calculated from TLE data via Celestrak)
- Data sources: Open Notify + Celestrak TLE

### 🚀 Upcoming Launch Countdown
- Live countdown to the next confirmed rocket launch
- Mission details: vehicle, agency, payload, launch site, webcast link
- Countdown auto-advances to the next launch after each one fires
- Data source: The Space Devs LL2

### 👨‍🚀 Who's In Space Right Now
- Real-time panel of every human currently in orbit
- Per-person: photo, name, nationality flag, mission, days elapsed, spacecraft
- Data source: Open Notify People in Space API

### ☄️ Near-Earth Object (Asteroid) Feed
- Daily feed of asteroids making close approaches to Earth
- Scatter plot: distance from Earth (x-axis) vs. diameter (y-axis), color-coded by hazard classification
- Click any asteroid for orbital data, composition, and JPL close-approach table
- Data source: NASA NeoWs API

### 🌌 JWST / Hubble Astronomy Picture of the Day
- Auto-updating daily image with title, description, and credit
- Historical APOD archive browser with search
- Data source: NASA APOD API

### 📅 Mission Timeline
- Horizontal interactive timeline from Sputnik (1957) to present
- Events: first satellite, first human, moon landing, Mars rovers, deep space probes, commercial era
- Filterable by agency (NASA, ESA, SpaceX, Roscosmos, ISRO, CNSA, JAXA)
- Click any mission for a detail panel with imagery and key stats
- Data source: The Space Devs LL2 + NASA NSSDCA

### 🚀 Rocket Size Comparison Tool
- Select 2–4 rockets from a searchable list and render them side-by-side to scale
- Stats shown per rocket: height, mass, payload to LEO, payload to GTO, first flight, reusability
- Data source: The Space Devs LL2

### 🪐 Exoplanet Explorer
- Visualize confirmed exoplanets plotted by distance, size, and habitable zone status
- Filter by star type, discovery method, and year
- Data source: NASA Exoplanet Archive API

---

## Data Sources

| Source | Feeds | Update Cadence | Rate Limit |
|---|---|---|---|
| [NASA Open APIs](https://api.nasa.gov/) | APOD, NeoWs, imagery | Daily / Daily | 1,000 req/hr (free key) |
| [The Space Devs LL2](https://ll.thespacedevs.com/2.2.0/swagger/) | Launches, rockets, stations, agencies, missions | 15 min | 15 req/hr anon, 60 req/hr with key |
| [Open Notify](http://open-notify.org/) | ISS position, people in space | Live / hourly | Unlimited |
| [Celestrak](https://celestrak.org/) | TLE orbital elements for ISS + satellites | 24 hr | Unlimited |
| [SpaceX API](https://github.com/r-spacex/SpaceX-API) | SpaceX launches, rockets, capsules, cores | On launch | Unlimited |
| [NASA NeoWs](https://api.nasa.gov/) | Near-Earth asteroid close approaches | Daily | Shared with NASA key |
| [NASA APOD](https://apod.nasa.gov/apod/astropix.html) | Astronomy picture of the day | Daily | Shared with NASA key |
| [NASA Exoplanet Archive](https://exoplanetarchive.ipac.caltech.edu/docs/TAP/usingTAP.html) | Confirmed exoplanets (TAP/ADQL queries) | Weekly | Unlimited |
| [NASA NSSDCA](https://nssdc.gsfc.nasa.gov/) | Historical mission metadata | Static | N/A |

All external calls are made server-side by Hangfire sync jobs. The frontend only ever calls your own `/api/*` endpoints.

---

## File Structure

```
CosmosView/
│
├── api/                     # .NET 10 Web API
│   ├── controllers/
│   │   ├── IssController.cs
│   │   ├── LaunchController.cs
│   │   ├── AsteroidsController.cs
│   │   ├── AstronautsController.cs
│   │   ├── ApodController.cs
│   │   ├── SolarSystemController.cs
│   │   ├── RocketsController.cs
│   │   ├── MissionsController.cs
│   │   └── ExoplanetsController.cs
│   │
│   ├── hubs/
│   │   ├── IssHub.cs                   # SignalR: live ISS position
│   │   └── LaunchHub.cs                # SignalR: countdown sync
│   │
│   ├── services/
│   │   ├── IssService.cs
│   │   ├── LaunchService.cs
│   │   ├── AsteroidService.cs
│   │   ├── AstronautService.cs
│   │   ├── ApodService.cs
│   │   ├── RocketService.cs
│   │   ├── MissionService.cs
│   │   ├── ExoplanetService.cs
│   │   └── TleService.cs               # Celestrak TLE parsing
│   │
│   ├── jobs/                           # Hangfire background sync jobs
│   │   ├── IssSyncJob.cs
│   │   ├── LaunchSyncJob.cs
│   │   ├── AsteroidSyncJob.cs
│   │   ├── ApodSyncJob.cs
│   │   ├── MissionSyncJob.cs
│   │   └── ExoplanetSyncJob.cs
│   │
│   ├── models/
│   │   ├── entities/                   # EF Core database entities
│   │   │   ├── Launch.cs
│   │   │   ├── Rocket.cs
│   │   │   ├── Mission.cs
│   │   │   ├── Astronaut.cs
│   │   │   ├── SpaceStation.cs
│   │   │   ├── Asteroid.cs
│   │   │   ├── ApodEntry.cs
│   │   │   └── Exoplanet.cs
│   │   │
│   │   └── DTOs/                       # API response shapes
│   │       ├── IssPositionDto.cs
│   │       ├── LaunchDto.cs
│   │       ├── RocketDto.cs
│   │       └── ...
│   │
│   ├── data/
│   │   ├── OrbitalDbContext.cs          # EF Core DbContext
│   │   └── Migrations/
│   │
│   ├── infrastructure/
│   │   ├── CacheKeys.cs                # Centralized Redis key constants
│   │   ├── RedisService.cs             # Wrapper around StackExchange.Redis
│   │   └── HttpClientFactory.cs        # Named HTTP clients per data source
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   └── Orbital.Api.csproj
│
├── web/                     # Next.js 15 + TypeScript frontend
│   ├── app/                            # App Router
│   │   ├── layout.tsx
│   │   ├── page.tsx                    # Home / dashboard
│   │   ├── solar-system/
│   │   │   └── page.tsx
│   │   ├── iss-tracker/
│   │   │   └── page.tsx
│   │   ├── launches/
│   │   │   └── page.tsx
│   │   ├── missions/
│   │   │   └── page.tsx
│   │   ├── asteroids/
│   │   │   └── page.tsx
│   │   ├── astronauts/
│   │   │   └── page.tsx
│   │   ├── apod/
│   │   │   └── page.tsx
│   │   ├── rockets/
│   │   │   └── page.tsx
│   │   ├── timeline/
│   │   │   └── page.tsx
│   │   └── exoplanets/
│   │       └── page.tsx
│   │
│   ├── components/
│   │   ├── globe/
│   │   │   ├── IssGlobe.tsx            # CesiumJS WebGL globe
│   │   │   └── MissionMap.tsx
│   │   ├── solar-system/
│   │   │   ├── SolarSystemCanvas.tsx   # Three.js / R3F scene
│   │   │   ├── Planet.tsx
│   │   │   ├── OrbitPath.tsx
│   │   │   └── PlanetInfoPanel.tsx
│   │   ├── timeline/
│   │   │   ├── MissionTimeline.tsx
│   │   │   └── TimelineEvent.tsx
│   │   ├── charts/
│   │   │   ├── AsteroidScatterPlot.tsx
│   │   │   ├── ExoplanetChart.tsx
│   │   │   └── RocketComparison.tsx
│   │   ├── ui/
│   │   │   ├── CountdownTimer.tsx
│   │   │   ├── AstronautCard.tsx
│   │   │   ├── ApodViewer.tsx
│   │   │   ├── LaunchCard.tsx
│   │   │   └── StatPanel.tsx
│   │   └── layout/
│   │       ├── Navbar.tsx
│   │       └── Sidebar.tsx
│   │
│   ├── hooks/
│   │   ├── useSignalR.ts               # SignalR connection hook
│   │   ├── useIssPosition.ts
│   │   ├── useUrlState.ts              # URL query param state sync
│   │   └── useLaunchCountdown.ts
│   │
│   ├── lib/
│   │   ├── api.ts                      # Typed API client (fetch wrappers)
│   │   ├── signalr.ts                  # SignalR hub connection factory
│   │   └── ephemeris.ts                # Orbital position math utilities
│   │
│   ├── store/
│   │   ├── solarSystemStore.ts         # Zustand: camera, selected body
│   │   └── filtersStore.ts             # Zustand: global filter state
│   │
│   ├── types/
│   │   ├── launch.ts
│   │   ├── rocket.ts
│   │   ├── iss.ts
│   │   ├── asteroid.ts
│   │   └── ...
│   │
│   ├── public/
│   │   ├── textures/                   # Planet texture maps (Three.js)
│   │   │   ├── earth.jpg
│   │   │   ├── mars.jpg
│   │   │   └── ...
│   │   └── manifest.json               # PWA manifest
│   │
│   ├── next.config.ts
│   ├── tailwind.config.ts
│   ├── tsconfig.json
│   ├── bunfig.toml
│   └── package.json
│
├── tests/                   # xUnit test project
│   ├── services/
│   │   ├── IssServiceTests.cs
│   │   └── LaunchServiceTests.cs
│   └── controllers/
│       └── IssControllerTests.cs
│
├── docker-compose.yml                  # Full local stack
├── docker-compose.override.yml         # Dev overrides
├── nginx.conf                          # Reverse proxy config
├── .env.example
├── .gitignore
└── README.md
```

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Bun](https://bun.sh/) (`curl -fsSL https://bun.sh/install | bash`)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [NASA API Key](https://api.nasa.gov/) (free, instant)
- [The Space Devs API Key](https://ll.thespacedevs.com/) (free tier available)

### 1. Clone & setup
```bash
git clone https://github.com/yourusername/CosmosView.git
cd CosmosView
cp .env.example .env
# Fill in your API keys in .env
```

### 2. Start infrastructure (Redis + PostgreSQL)
```bash
docker-compose up -d redis postgres
```

### 3. Run the backend
```bash
cd CosmosView.Api
dotnet restore
dotnet ef database update
dotnet run
# API: https://localhost:7000
# Swagger: https://localhost:7000/swagger
# Hangfire dashboard: https://localhost:7000/hangfire
```

### 4. Run the frontend
```bash
cd CosmosView.Web
bun install
bun dev
# App: http://localhost:3000
```

### 5. Full stack via Docker Compose
```bash
docker-compose up --build
# App: http://localhost:80
```

---

## Environment Variables

### Backend (`CosmosView.Api/appsettings.json` / `.env`)
```
NASA_API_KEY=your_key_here
SPACE_DEVS_API_KEY=your_key_here
REDIS_CONNECTION_STRING=localhost:6379
POSTGRES_CONNECTION_STRING=Host=localhost;Database=cosmosview;Username=postgres;Password=postgres
HANGFIRE_DASHBOARD_ENABLED=true
```

### Frontend (`CosmosView.Web/.env.local`)
```
NEXT_PUBLIC_API_BASE_URL=https://localhost:7000
NEXT_PUBLIC_SIGNALR_HUB_URL=https://localhost:7000/hubs
```

> ⚠️ Never put NASA or Space Devs API keys in the frontend. All external API calls go through the .NET backend.

---

## API Reference

Auto-generated Swagger docs are available at `/swagger` when running the backend locally. Key endpoints:

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/iss/position` | Current ISS lat/lng/altitude |
| GET | `/api/iss/track` | ISS ground track for next N orbits |
| GET | `/api/launches/upcoming` | Upcoming launches (paginated) |
| GET | `/api/launches/history` | Past launches with filters |
| GET | `/api/rockets` | All rockets with specs |
| GET | `/api/rockets/{id}` | Single rocket detail |
| GET | `/api/missions` | Mission list with filters |
| GET | `/api/astronauts/in-space` | Currently orbiting humans |
| GET | `/api/asteroids/today` | Today's NEO close approaches |
| GET | `/api/apod` | Astronomy picture of the day |
| GET | `/api/apod/archive` | APOD history with search |
| GET | `/api/exoplanets` | Confirmed exoplanets |
| GET | `/api/solar-system/bodies` | Planet positions (calculated) |
| WS | `/hubs/iss` | SignalR: live ISS position stream |
| WS | `/hubs/launch` | SignalR: launch countdown sync |

---

## Todo / Roadmap

---

### 🏗️ Phase 0 — Project Setup
- [x] Initialize .NET 10 Web API project (`api/`)
- [x] Initialize Next.js 15 app with Bun + TypeScript (`web/`)
- [x] Set up Docker Compose with Redis and PostgreSQL services
- [x] Configure Nginx reverse proxy
- [x] Add `.env.example` and `.gitignore`
- [x] Create full folder + file skeleton (backend + frontend)
- [x] Add planet texture maps (`web/public/textures/`)
- [x] Add PWA manifest (`web/public/manifest.json`)
- [ ] Set up GitHub Actions CI pipeline

---

### ⚙️ Phase 1 — Backend Infrastructure
> Foundation for everything. Nothing else runs until this phase is complete.

- [ ] `Program.cs` — wire up EF Core, Redis, Hangfire, SignalR, Swagger, CORS, middleware
- [ ] `OrbitalDbContext.cs` — define all `DbSet<T>` entities and run first migration
- [ ] `RedisService.cs` — typed cache wrapper with `GetAsync<T>` / `SetAsync` / TTL helpers
- [ ] `HttpClientFactory.cs` — register named `HttpClient` per data source (NASA, SpaceDevs, etc.)
- [ ] `CacheKeys.cs` — centralize all Redis key constants
- [ ] Generate OpenAPI/Swagger docs (auto from controllers once registered)
- [ ] Enable Hangfire dashboard (dev only, guarded by env flag)
- [ ] Add global exception handling middleware
- [ ] Add request logging middleware

---

### 🗄️ Phase 2 — Data Models & Migrations
> Define what gets stored and what gets returned to the frontend.

**Entities** (database tables via EF Core):
- [ ] `Launch.cs`
- [ ] `Rocket.cs`
- [ ] `Mission.cs`
- [ ] `Astronaut.cs`
- [ ] `SpaceStation.cs`
- [ ] `Asteroid.cs`
- [ ] `ApodEntry.cs`
- [ ] `Exoplanet.cs`

**DTOs** (API response shapes — slimmed down from entities):
- [ ] `LaunchDto.cs`
- [ ] `RocketDto.cs`
- [ ] `MissionDto.cs`
- [ ] `AstronautDto.cs`
- [ ] `AsteroidDto.cs`
- [ ] `ApodDto.cs`
- [ ] `ExoplanetDto.cs`
- [ ] `IssPositionDto.cs`

**Migrations:**
- [ ] Run `dotnet ef migrations add InitialCreate`
- [ ] Run `dotnet ef database update` (PostgreSQL schema created)

---

### 🔄 Phase 3 — Hangfire Background Sync Jobs
> What makes the "zero maintenance" goal real. Start simple, build up.

- [ ] `ApodSyncJob.cs` — fetch NASA APOD daily at midnight UTC *(start here — simplest)*
- [ ] `AsteroidSyncJob.cs` — fetch NeoWs close-approach feed each morning
- [ ] `ExoplanetSyncJob.cs` — sync NASA Exoplanet Archive weekly
- [ ] `TleSyncJob.cs` — fetch ISS TLE from Celestrak every 6 hours
- [ ] `LaunchSyncJob.cs` — sync upcoming + past launches from LL2 every 15 min
- [ ] `MissionSyncJob.cs` — sync mission history from LL2 nightly
- [ ] `IssSyncJob.cs` — poll Open Notify every 5s, push directly via SignalR *(no DB write)*

---

### 🔌 Phase 4 — Controllers & Services
> Pattern for each: Redis check → PostgreSQL query → cache result → return DTO.

- [ ] `IssService.cs` + `IssController.cs` — position, ground track *(start here)*
- [ ] `AstronautService.cs` + `AstronautsController.cs` — currently in-orbit crew
- [ ] `ApodService.cs` + `ApodController.cs` — daily image + archive
- [ ] `AsteroidService.cs` + `AsteroidsController.cs` — daily NEO feed
- [ ] `LaunchService.cs` + `LaunchController.cs` — upcoming + history + filters
- [ ] `RocketService.cs` + `RocketsController.cs` — list + detail + comparison data
- [ ] `MissionService.cs` + `MissionsController.cs` — mission list + filters
- [ ] `ExoplanetService.cs` + `ExoplanetsController.cs` — confirmed exoplanets + filters
- [ ] `SolarSystemController.cs` — planet positions via ephemeris calculations

---

### 📡 Phase 5 — SignalR Real-time Hubs
> Small amount of code, very high visual impact.

- [ ] `IssHub.cs` — broadcast ISS position to all connected clients every 5s
- [ ] `LaunchHub.cs` — broadcast countdown T-minus to keep all clients in sync
- [ ] CORS configuration for Next.js origin
- [ ] SignalR group management (per-feature subscriptions, e.g. `iss-tracker`, `launch-countdown`)

---

### 🎨 Phase 6 — Frontend Infrastructure
> Get the plumbing right before building any feature pages.

- [ ] `app/layout.tsx` — TanStack Query provider, Navbar, Sidebar shell
- [ ] `components/layout/Navbar.tsx` + `Sidebar.tsx`
- [ ] `lib/api.ts` — typed fetch wrapper for all backend endpoints
- [ ] `lib/signalr.ts` — SignalR hub connection factory
- [ ] `hooks/useSignalR.ts` — connection hook with auto-reconnect
- [ ] `hooks/useIssPosition.ts` — subscribes to ISS hub, exposes live position
- [ ] `hooks/useUrlState.ts` — syncs state to/from URL query params
- [ ] `hooks/useLaunchCountdown.ts` — countdown from SignalR or local timer
- [ ] `store/solarSystemStore.ts` — Zustand: camera state, selected body
- [ ] `store/filtersStore.ts` — Zustand: global filter selections
- [ ] `tailwind.config.ts` — space-themed design tokens (colors, fonts)
- [ ] Configure next-pwa with service worker for offline support

**TypeScript types** (fill in parallel with features as needed):
- [ ] `types/launch.ts`
- [ ] `types/rocket.ts`
- [ ] `types/iss.ts`
- [ ] `types/asteroid.ts`
- [ ] `types/astronaut.ts`
- [ ] `types/apod.ts`
- [ ] `types/mission.ts`
- [ ] `types/exoplanet.ts`

---

### 🌌 Phase 7 — Feature Pages
> Build in this order: simplest first, most complex last.

#### 🌌 APOD — Astronomy Picture of the Day
- [ ] `app/apod/page.tsx` — daily image display
- [ ] `components/ui/ApodViewer.tsx` — image/video renderer + description panel
- [ ] Archive browser with search + date picker
- [ ] Video support (some APODs are YouTube embeds)

#### 👨‍🚀 Who's In Space Right Now
- [ ] `app/astronauts/page.tsx`
- [ ] `components/ui/AstronautCard.tsx` — photo, name, flag, days elapsed, spacecraft
- [ ] Days-in-orbit live counter
- [ ] Mission / spacecraft badge

#### ☄️ Asteroid Feed
- [ ] `app/asteroids/page.tsx`
- [ ] `components/charts/AsteroidScatterPlot.tsx` — D3 scatter: distance vs. size, hazard color
- [ ] Click-to-detail panel with orbital data
- [ ] Hazard classification legend

#### 🚀 Launch Countdown
- [ ] `app/launches/page.tsx`
- [ ] `components/ui/CountdownTimer.tsx`
- [ ] `components/ui/LaunchCard.tsx` — mission detail panel
- [ ] Upcoming launch list
- [ ] Auto-advance to next launch on T+0
- [ ] Webcast link integration

#### 🌍 Space Station & Mission Map
- [ ] `app/missions/page.tsx`
- [ ] `components/globe/MissionMap.tsx` — deck.gl or Leaflet world map
- [ ] Space station markers with popup panels
- [ ] Launch site markers with stats
- [ ] Mission filter bar (rocket / type / agency / decade)
- [ ] Mobile-responsive layout

#### 🛰️ Live ISS Tracker
- [ ] `app/iss-tracker/page.tsx`
- [ ] `components/globe/IssGlobe.tsx` — CesiumJS WebGL globe
- [ ] Real-time ISS position marker via SignalR
- [ ] 90-minute orbital ground track trail
- [ ] `lib/ephemeris.ts` — visibility window calculations from TLE
- [ ] ISS stats panel (altitude, speed, orbital period)

#### 📅 Mission Timeline
- [ ] `app/timeline/page.tsx`
- [ ] `components/timeline/MissionTimeline.tsx` — horizontal scrollable D3 timeline
- [ ] `components/timeline/TimelineEvent.tsx` — event card with imagery
- [ ] Agency filter bar
- [ ] Click-to-expand detail panel

#### 🚀 Rocket Comparison
- [ ] `app/rockets/page.tsx`
- [ ] `components/charts/RocketComparison.tsx` — side-by-side to-scale SVG render
- [ ] Searchable rocket selector (up to 4)
- [ ] Stats table (height, mass, payload to LEO/GTO, reusability)

#### 🪐 Exoplanet Explorer
- [ ] `app/exoplanets/page.tsx`
- [ ] `components/charts/ExoplanetChart.tsx` — scatter plot by distance + size
- [ ] Habitable zone overlay
- [ ] Filter by star type, discovery method, year
- [ ] Click-to-detail panel

#### ☀️ 3D Solar System *(most complex — save for last)*
- [ ] `app/solar-system/page.tsx`
- [ ] `components/solar-system/SolarSystemCanvas.tsx` — Three.js / R3F scene setup
- [ ] `components/solar-system/Planet.tsx` — mesh + texture map per body
- [ ] `components/solar-system/OrbitPath.tsx` — orbital path rings
- [ ] `components/solar-system/PlanetInfoPanel.tsx` — click-to-expand stats
- [ ] Camera controls (orbit, zoom, pan)
- [ ] URL state sync via `useUrlState` (camera position + selected body)
- [ ] Toggle panel: orbits / labels / asteroid belt / scale mode

---

### 🧪 Phase 8 — Testing
- [ ] Unit tests for all services (`tests/services/`) — xUnit + Moq
- [ ] Integration tests for key controllers (`tests/controllers/`)
- [ ] Frontend component tests — Vitest + Testing Library

---

### 🚢 Phase 9 — Deployment
- [ ] Dockerfiles for `api/` and `web/`
- [ ] Docker Compose production config
- [ ] GitHub Actions: build + test on PR
- [ ] GitHub Actions: deploy on merge to `main`
- [ ] Choose hosting (Railway / Render / Azure / Fly.io)
- [ ] Set up production environment variables
- [ ] Configure production Redis and PostgreSQL
- [ ] Enable HTTPS (Let's Encrypt via Nginx)
- [ ] Add uptime monitoring

---
