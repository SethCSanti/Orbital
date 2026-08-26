"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import MissionMap from "@/components/globe/MissionMap";
import CatalogStatus from "@/components/catalog/CatalogStatus";
import Pagination from "@/components/catalog/Pagination";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { useUrlState } from "@/hooks/useUrlState";
import { api } from "@/lib/api";

export default function MissionsView() {
  const [type, setType] = useUrlState("type");
  const [orbitAbbrev, setOrbitAbbrev] = useUrlState("orbitAbbrev");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const missionsQuery = useQuery({
    queryKey: ["missions", { page, type, orbitAbbrev, search }],
    queryFn: ({ signal }) => api.missions.all({ page, pageSize: 24, search, type: type || undefined, orbitAbbrev: orbitAbbrev || undefined }, signal),
    placeholderData: (previous) => previous,
  });
  const stationsQuery = useQuery({ queryKey: ["space-stations"], queryFn: ({ signal }) => api.spaceStations.all(signal), staleTime: 604_800_000 });
  useEffect(() => setPage(1), [type, orbitAbbrev, search]);

  return <div className="mx-auto max-w-7xl">
    <header className="mb-8 flex flex-wrap items-end justify-between gap-4"><div><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-signal">Stations / mission history</p><h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">Mission archive</h1><p className="mt-3 max-w-2xl text-base leading-7 text-muted">Search historical missions and open a record for its related launches.</p></div><CatalogStatus /></header>
    <section className="mb-6 border border-line bg-panel p-4 sm:p-5"><div className="flex flex-wrap items-end gap-4"><label className="text-xs font-semibold uppercase tracking-[0.14em] text-dim">Search missions<input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Mission name or description" className="mt-2 block w-56 border border-line bg-orbit-900 px-3 py-2 text-sm font-normal normal-case tracking-normal text-ink placeholder:text-dim" /></label><label className="text-xs font-semibold uppercase tracking-[0.14em] text-dim">Mission type<select value={type} onChange={(event) => setType(event.target.value)} className="mt-2 block w-44 border border-line bg-orbit-900 px-3 py-2 text-sm font-normal normal-case tracking-normal text-ink"><option value="">All types</option>{(missionsQuery.data?.filterMetadata?.types ?? []).map((value) => <option key={value}>{value}</option>)}</select></label><label className="text-xs font-semibold uppercase tracking-[0.14em] text-dim">Orbit<select value={orbitAbbrev} onChange={(event) => setOrbitAbbrev(event.target.value)} className="mt-2 block w-44 border border-line bg-orbit-900 px-3 py-2 text-sm font-normal normal-case tracking-normal text-ink"><option value="">All orbits</option>{(missionsQuery.data?.filterMetadata?.orbits ?? []).map((value) => <option key={value}>{value}</option>)}</select></label></div></section>
    {(missionsQuery.isPending || stationsQuery.isPending) && <LoadingState label="Loading mission archive" />}
    {missionsQuery.isError && <ErrorState message={missionsQuery.error.message} onRetry={() => void missionsQuery.refetch()} />}
    {stationsQuery.isError && <ErrorState message={stationsQuery.error.message} onRetry={() => void stationsQuery.refetch()} />}
    {missionsQuery.data && <><MissionMap missions={missionsQuery.data.items} stations={stationsQuery.data ?? []} /><div className="mt-5 flex flex-wrap items-center justify-between gap-4"><Pagination page={missionsQuery.data.page} pageSize={missionsQuery.data.pageSize} total={missionsQuery.data.total} onChange={setPage}/><div className="flex flex-wrap gap-2">{missionsQuery.data.items.slice(0, 6).map((mission) => <Link key={mission.id} href={`/missions/${mission.id}`} prefetch={false} className="text-xs text-signal hover:text-signal-strong">{mission.name} →</Link>)}</div></div></>}
  </div>;
}
