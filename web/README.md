# Orbital web

The frontend is a Next.js 15 application using the App Router, TypeScript, and Bun.

## Local development

```sh
bun install
bun run dev
```

The dev and production-build hooks copy Cesium's runtime assets into `public/cesium` before Next.js starts.

## Environment variables

Copy `.env.example` to `.env.local`. The ISS tracker requires `NEXT_PUBLIC_CESIUM_ION_TOKEN`; get a token from [Cesium ion](https://ion.cesium.com/tokens).
