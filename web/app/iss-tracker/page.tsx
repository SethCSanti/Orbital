"use client";

import { useEffect, useMemo, useState } from "react";
import { HubConnectionState } from "@microsoft/signalr";
import IssGlobe from "@/components/globe/IssGlobe";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { useIssPosition } from "@/hooks/useIssPosition";
import { estimateIssVisibility } from "@/lib/ephemeris";

const DEFAULT_OBSERVER = { latitude: 40.015, longitude: -105.27 };

function formatCoordinate(value: number, positive: string, negative: string) {
  return `${Math.abs(value).toFixed(2)}° ${value >= 0 ? positive : negative}`;
}

export default function IssTrackerPage() {
  const { data: position, isLoading, isError, error, refetch, connectionState, signalError } = useIssPosition();
  const [trail, setTrail] = useState<typeof position extends infer T ? Exclude<T, null>[] : never[]>([]);
  const [observer, setObserver] = useState(DEFAULT_OBSERVER);

  useEffect(() => {
    if (!position) return;
    setTrail((previous) => {
      if (previous.at(-1)?.timestamp === position.timestamp) return previous;
      return [...previous, position].slice(-18);
    });
  }, [position]);

  const visibility = useMemo(
    () => position ? estimateIssVisibility(position, observer) : null,
    [position, observer],
  );
  const connectionLabel = connectionState === HubConnectionState.Connected ? "Live" : connectionState;

  return (
    <div className="mx-auto max-w-[1500px] space-y-8 px-5 py-8 lg:px-10">
      <header className="max-w-3xl">
        <p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-cyan">Realtime telemetry</p>
        <h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink md:text-5xl">International Space Station</h1>
        <p className="mt-4 text-base leading-7 text-muted">A live orbital position and the ground track collected since this page opened.</p>
      </header>

      {isLoading && <LoadingState label="Acquiring ISS position" />}
      {isError && <ErrorState message={error instanceof Error ? error.message : "ISS position unavailable."} onRetry={() => void refetch()} />}

      {position && (
        <div className="grid gap-5 xl:grid-cols-[minmax(0,1fr)_320px]">
          <IssGlobe position={position} trail={trail} />
          <aside className="space-y-4 border border-line bg-panel p-5">
            <div className="flex items-center justify-between border-b border-line pb-4">
              <span className="font-display text-xs font-semibold uppercase tracking-[0.18em] text-muted">Connection</span>
              <span className={connectionState === HubConnectionState.Connected ? "text-success" : "text-warning"}>{connectionLabel}</span>
            </div>
            <dl className="grid grid-cols-2 gap-4 text-sm">
              <div><dt className="text-muted">Latitude</dt><dd className="mt-1 font-display text-lg text-ink">{formatCoordinate(position.latitude, "N", "S")}</dd></div>
              <div><dt className="text-muted">Longitude</dt><dd className="mt-1 font-display text-lg text-ink">{formatCoordinate(position.longitude, "E", "W")}</dd></div>
              <div><dt className="text-muted">Track points</dt><dd className="mt-1 font-display text-lg text-ink">{trail.length}/18</dd></div>
              <div><dt className="text-muted">Updated</dt><dd className="mt-1 text-ink">{new Date(position.timestamp).toLocaleTimeString()}</dd></div>
            </dl>
            <div className="border-t border-line pt-5">
              <p className="font-display text-xs font-semibold uppercase tracking-[0.18em] text-muted">Next pass estimate</p>
              <div className="mt-4 grid grid-cols-2 gap-3">
                <label className="text-xs text-muted">Observer latitude<input type="number" value={observer.latitude} onChange={(event) => setObserver((value) => ({ ...value, latitude: Number(event.target.value) }))} className="mt-1 w-full border border-line bg-orbit-900 px-2 py-2 text-sm text-ink" min={-90} max={90} step={0.001} /></label>
                <label className="text-xs text-muted">Observer longitude<input type="number" value={observer.longitude} onChange={(event) => setObserver((value) => ({ ...value, longitude: Number(event.target.value) }))} className="mt-1 w-full border border-line bg-orbit-900 px-2 py-2 text-sm text-ink" min={-180} max={180} step={0.001} /></label>
              </div>
              {visibility && <p className="mt-4 text-sm leading-6 text-muted">{visibility.isVisibleNow ? "The ISS is near your location now." : `Approx. rise in ${visibility.minutesUntilRise} minutes.`} Overhead around {visibility.overheadAt.toLocaleTimeString()}.</p>}
              <p className="mt-3 text-xs leading-5 text-dim">Pass timing is an approximation from the live point and a 92.68-minute orbital period; it is not a TLE-derived prediction.</p>
            </div>
            {signalError && <p className="border-t border-line pt-4 text-xs text-warning">Realtime updates unavailable; showing the latest API snapshot.</p>}
          </aside>
        </div>
      )}
    </div>
  );
}
