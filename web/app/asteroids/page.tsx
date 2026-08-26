"use client";

import { useCallback, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import type { Asteroid } from "@/types/asteroid";
import AsteroidScatterPlot from "@/components/charts/AsteroidScatterPlot";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";

export default function AsteroidsPage() {
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const query = useQuery({ queryKey: ["asteroids", "feed"], queryFn: ({ signal }) => api.asteroids.feed(signal), staleTime: 3_600_000 });
  const selectAsteroid = useCallback((asteroid: Asteroid) => setSelectedId(asteroid.neoReferenceId), []);
  const selected = query.data?.find((asteroid) => asteroid.neoReferenceId === selectedId) ?? null;

  return (
    <div className="mx-auto max-w-7xl">
      <header className="mb-8">
        <p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-signal">NeoWs / close approaches</p>
        <h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">Near-Earth objects</h1>
        <p className="mt-3 max-w-2xl text-base leading-7 text-muted">Distance and estimated diameter for the current asteroid feed. Select a point to inspect its approach.</p>
      </header>
      {query.isPending && <LoadingState label="Loading asteroid feed" />}
      {query.isError && <ErrorState message={query.error.message} onRetry={() => query.refetch()} />}
      {query.data && <div className="grid gap-6 xl:grid-cols-[minmax(0,1fr)_300px]">
        <section className="border border-line bg-panel p-4 sm:p-6">
          <div className="mb-4 flex items-baseline justify-between gap-4"><h2 className="font-display text-xl font-semibold text-ink">Approach plot</h2><span className="text-xs text-dim">{query.data.length} objects</span></div>
          <AsteroidScatterPlot asteroids={query.data} selectedId={selectedId ?? undefined} onSelect={selectAsteroid} />
        </section>
        <aside className="border border-line bg-panel p-5" aria-live="polite">
          {selected ? <div>
            <p className="text-xs font-semibold uppercase tracking-[0.16em] text-danger">Selected object</p>
            <h2 className="mt-3 font-display text-xl font-semibold text-ink">{selected.name}</h2>
            <dl className="mt-6 space-y-4 text-sm"><div><dt className="text-xs text-dim">Close approach</dt><dd className="mt-1 text-muted">{selected.closeApproachDate}</dd></div><div><dt className="text-xs text-dim">Miss distance</dt><dd className="mt-1 text-muted">{selected.missDistanceKm.toLocaleString()} km</dd></div><div><dt className="text-xs text-dim">Relative velocity</dt><dd className="mt-1 text-muted">{selected.relativeVelocityKph.toLocaleString()} km/h</dd></div><div><dt className="text-xs text-dim">Estimated diameter</dt><dd className="mt-1 text-muted">{selected.estimatedDiameterMinKm.toFixed(2)}–{selected.estimatedDiameterMaxKm.toFixed(2)} km</dd></div></dl>
            <a href={selected.nasaJplUrl} target="_blank" rel="noreferrer" className="mt-7 inline-block text-sm font-semibold text-signal hover:text-signal-strong">Open NASA JPL record ↗</a>
          </div> : <div className="flex min-h-56 flex-col justify-center"><p className="text-xs font-semibold uppercase tracking-[0.16em] text-dim">No object selected</p><p className="mt-3 text-sm leading-6 text-muted">Select a point in the plot to see the close-approach details.</p></div>}
        </aside>
      </div>}
    </div>
  );
}
