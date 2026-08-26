import withPWAInit from "@ducanh2912/next-pwa";
import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  reactStrictMode: true,
  images: {
    remotePatterns: [
      { protocol: "https", hostname: "apod.nasa.gov" },
      { protocol: "https", hostname: "*.nasa.gov" },
      { protocol: "https", hostname: "*.thespacedevs.com" },
    ],
  },
};

const withPWA = withPWAInit({
  dest: "public",
  register: true,
  disable: process.env.NODE_ENV !== "production",
  workboxOptions: { skipWaiting: true },
  fallbacks: { document: "/~offline" },
});

// Keep the development config free of webpack-only PWA hooks so Turbopack can run without warnings.
export default process.env.NODE_ENV === "development" ? nextConfig : withPWA(nextConfig);
