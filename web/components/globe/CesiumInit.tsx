"use client";

import { Ion } from "cesium";

let ionConfigured = false;

// Configure Cesium's client singleton before any ISS Viewer is constructed.
export default function CesiumInit() {
  if (typeof window !== "undefined" && !ionConfigured) {
    const token = process.env.NEXT_PUBLIC_CESIUM_ION_TOKEN;

    if (!token) {
      console.error(
        "[Cesium] NEXT_PUBLIC_CESIUM_ION_TOKEN is missing. Add it to web/.env.local before opening the ISS tracker.",
      );
    } else {
      Ion.defaultAccessToken = token;
    }

    ionConfigured = true;
  }

  return null;
}
