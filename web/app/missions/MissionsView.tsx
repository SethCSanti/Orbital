"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import MissionMap from "@/components/globe/MissionMap";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { useUrlState } from "@/hooks/useUrlState";

export default function MissionsView() {
  const [type, setType] = useUrlState("type");
  const [orbitAbbrev, setOrbitAbbrev] = useUrlState("orbitAbbrev");
  const missionsQuery = useQuery({
    queryKey: ["missions", type, orbitAbbrev],
    queryFn: () => api.missions.all({ type: type || undefined, orbitAbbrev: orbitAbbrev || undefined }),
    staleTime: 86_400_000,
  });
  const stationsQuery = useQuery({ queryKey: ["space-stations"], queryFn: api.spaceStations.all, staleTime: 604_800_000 });

  return (
    <div className="mx-auto max-w-7xl">
      <header className="mb-8"><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-signal">Stations / mission history</p><h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">Space station & mission atlas</h1><p className="mt-3 max-w-2xl text-base leading-7 text-muted">A compact reference for orbital outposts and the missions that put them there.</p></header>
      <section className="mb-6 border border-line bg-panel p-4 sm:p-5"><div className="flex flex-wrap items-end gap-4"><label className="text-xs font-semibold uppercase tracking-[0.14em] text-dim">Mission type<input value={type} onChange={(event) => setType(event.target.value)} placeholder="e.g. Human" className="mt-2 block w-44 border border-line bg-orbit-900 px-3 py-2 text-sm font-normal normal-case tracking-normal text-ink placeholder:text-dim" /></label><label className="text-xs font-semibold uppercase tracking-[0.14em] text-dim">Orbit abbreviation<input value={orbitAbbrev} onChange={(event) => setOrbitAbbrev(event.target.value)} placeholder="e.g. LEO" className="mt-2 block w-44 border border-line bg-orbit-900 px-3 py-2 text-sm font-normal normal-case tracking-normal text-ink placeholder:text-dim" /></label>{(type || orbitAbbrev) && <button type="button" onClick={() => { setType(""); setOrbitAbbrev(""); }} className="px-2 py-2 text-xs font-semibold uppercase tracking-[0.14em] text-signal hover:text-signal-strong">Clear filters</button>}</div></section>
      {(missionsQuery.isPending || stationsQuery.isPending) && <LoadingState label="Loading mission atlas" />}
      {missionsQuery.isError && <ErrorState message={missionsQuery.error.message} onRetry={() => missionsQuery.refetch()} />}
      {stationsQuery.isError && <ErrorState message={stationsQuery.error.message} onRetry={() => stationsQuery.refetch()} />}
      {(missionsQuery.data || stationsQuery.data) && <MissionMap missions={missionsQuery.data ?? []} stations={stationsQuery.data ?? []} />}
    </div>
  );
}
