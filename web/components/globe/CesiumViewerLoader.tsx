"use client";

import { useEffect, useState, type ComponentType } from "react";
import type { IssPositionUpdate } from "@/types/iss";
import { initializeCesium } from "@/components/globe/CesiumInit";

export default function CesiumViewerLoader({ position, trail }: { position: IssPositionUpdate | null; trail: IssPositionUpdate[] }) {
  const [Viewer, setViewer] = useState<ComponentType<{ position: IssPositionUpdate | null; trail: IssPositionUpdate[] }> | null>(null);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    let active = true;
    // Base URL setup is synchronous, so both large module graphs can download
    // together; mounting still waits for the token and Viewer code to finish.
    const cesiumReady = initializeCesium();
    const viewerReady = import("@/components/globe/IssGlobe");
    void Promise.all([cesiumReady, viewerReady])
      .then(([, module]) => {
        if (active) setViewer(() => module.default);
      })
      .catch((reason: unknown) => {
        if (active) setError(reason instanceof Error ? reason : new Error("Cesium failed to initialize."));
      });
    return () => {
      active = false;
    };
  }, []);

  if (error) return <div className="flex min-h-[420px] items-center justify-center border border-warning/50 bg-panel p-6 text-sm text-warning" role="alert">{error.message}</div>;
  if (!Viewer) return <div className="flex min-h-[420px] items-center justify-center border border-line bg-panel p-6 text-sm text-muted">Loading Cesium globe…</div>;
  return <Viewer position={position} trail={trail} />;
}
