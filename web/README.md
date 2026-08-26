# Orbital web

The frontend is a Next.js 15 application using the App Router, TypeScript, and Bun.

## Local development

```sh
bun install
bun run dev
```

The dev and production-build hooks copy Cesium's runtime assets into `public/cesium` before Next.js starts.
The catalogue pages require the API and its PostgreSQL/Redis dependencies; run the full stack with `docker compose up` or start the API separately on `http://localhost:5110`.

Rockets and missions are historical catalogues backed by paginated API responses. The launch sync advances the archive one upstream page at a time and exposes progress at `/api/catalog/status`; partial records remain visible while the backfill runs. Catalog detail pages include related launch history without downloading the full archive.

## Environment variables

Copy `.env.example` to `.env.local`. The ISS tracker requires `NEXT_PUBLIC_CESIUM_ION_TOKEN`; create one at [https://ion.cesium.com/tokens](https://ion.cesium.com/tokens). Never commit the real token; `.env.local` is ignored by git.
