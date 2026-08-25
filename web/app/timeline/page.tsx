"use client";

import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import MissionTimeline from "@/components/timeline/MissionTimeline";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { api } from "@/lib/api";

export default function TimelinePage() {
  const missions = useQuery({ queryKey: ["missions", "timeline"], queryFn: () => api.missions.all(), staleTime: 86_400_000 });
  const launches = useQuery({ queryKey: ["launches", "past"], queryFn: () => api.launches.past(), staleTime: 86_400_000 });
  const [typeFilter, setTypeFilter] = useState("");
  const [orbitFilter, setOrbitFilter] = useState("");
  const types = useMemo(() => [...new Set((missions.data ?? []).map((item) => item.type).filter(Boolean))].sort(), [missions.data]);
  const orbits = useMemo(() => [...new Set([...(missions.data ?? []), ...(launches.data ?? [])].map((item) => item.orbitAbbrev).filter(Boolean))].sort(), [missions.data, launches.data]);
  const error = missions.error ?? launches.error;

  return <div className="mx-auto max-w-[1500px] space-y-8 px-5 py-8 lg:px-10">
    <header className="max-w-3xl"><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-cyan">History / operations</p><h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink md:text-5xl">Mission timeline</h1><p className="mt-4 text-base leading-7 text-muted">A chronological view assembled from mission records and completed launch events.</p></header>
    <div className="flex flex-wrap gap-3 border-y border-line py-4"><label className="text-xs uppercase tracking-[0.12em] text-muted">Mission type<select value={typeFilter} onChange={(event) => setTypeFilter(event.target.value)} className="ml-2 border border-line bg-panel px-3 py-2 text-sm normal-case tracking-normal text-ink"><option value="">All types</option>{types.map((type) => <option key={type}>{type}</option>)}</select></label><label className="text-xs uppercase tracking-[0.12em] text-muted">Orbit<select value={orbitFilter} onChange={(event) => setOrbitFilter(event.target.value)} className="ml-2 border border-line bg-panel px-3 py-2 text-sm normal-case tracking-normal text-ink"><option value="">All orbits</option>{orbits.map((orbit) => <option key={orbit}>{orbit}</option>)}</select></label></div>
    {(missions.isLoading || launches.isLoading) && <LoadingState label="Loading mission history" />}
    {error && <ErrorState message={error instanceof Error ? error.message : "Mission history unavailable."} onRetry={() => { void missions.refetch(); void launches.refetch(); }} />}
    {missions.data && launches.data && <MissionTimeline missions={missions.data} launches={launches.data} typeFilter={typeFilter} orbitFilter={orbitFilter} />}
  </div>;
}
