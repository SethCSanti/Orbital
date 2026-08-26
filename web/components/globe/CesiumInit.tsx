"use client";

import { useEffect } from "react";

declare global {
  interface Window {
    CESIUM_BASE_URL?: string;
  }
}

let initialization: Promise<void> | null = null;

// Set the asset base URL before importing Cesium. Cesium evaluates its worker
// and asset URL configuration during module initialization.
export function initializeCesium(): Promise<void> {
  if (typeof window === "undefined") return Promise.resolve();
  if (initialization) return initialization;

  window.CESIUM_BASE_URL = "/cesium";
  const token = process.env.NEXT_PUBLIC_CESIUM_ION_TOKEN;
  if (!token) {
    const error = new Error(
      "[Cesium] NEXT_PUBLIC_CESIUM_ION_TOKEN is missing. Add it to web/.env.local before opening the ISS tracker.",
    );
    console.error(error.message);
    return Promise.reject(error);
  }

  initialization = import("cesium").then(({ Ion }) => {
    Ion.defaultAccessToken = token;
  });
  return initialization;
}

export default function CesiumInit() {
  useEffect(() => {
    void initializeCesium().catch(() => undefined);
  }, []);

  return null;
}
