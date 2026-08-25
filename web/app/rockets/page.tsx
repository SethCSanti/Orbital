"use client";

import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import RocketComparison from "@/components/charts/RocketComparison";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { api } from "@/lib/api";

export default function RocketsPage() {
  const rockets = useQuery({ queryKey: ["rockets"], queryFn: api.rockets.all, staleTime: 86_400_000 });
  const [search, setSearch] = useState("");
  const [selectedNames, setSelectedNames] = useState<string[]>([]);
  const filtered = useMemo(() => (rockets.data ?? []).filter((rocket) => `${rocket.name} ${rocket.fullName}`.toLowerCase().includes(search.toLowerCase())), [rockets.data, search]);
  const selected = selectedNames.slice(0, 4);
  const comparison = useQuery({ queryKey: ["rockets", "compare", selected], queryFn: () => api.rockets.compare(selected), enabled: selected.length > 0, staleTime: 86_400_000 });

  function toggleRocket(name: string) {
    setSelectedNames((current) => current.includes(name) ? current.filter((item) => item !== name) : current.length < 4 ? [...current, name] : current);
  }

  return <div className="mx-auto max-w-[1500px] space-y-8 px-5 py-8 lg:px-10">
    <header className="max-w-3xl"><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-cyan">Vehicle reference</p><h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink md:text-5xl">Rocket comparison</h1><p className="mt-4 text-base leading-7 text-muted">Select two to four vehicles to compare their physical scale, performance, and launch record.</p></header>
    <label className="block max-w-xl text-xs uppercase tracking-[0.14em] text-muted">Search rockets<input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Falcon, Ariane, Soyuz…" className="mt-2 w-full border border-line bg-panel px-4 py-3 text-sm normal-case tracking-normal text-ink placeholder:text-dim" /></label>
    {rockets.isLoading && <LoadingState label="Loading rocket catalogue" />}
    {rockets.error && <ErrorState message={rockets.error instanceof Error ? rockets.error.message : "Rocket catalogue unavailable."} onRetry={() => void rockets.refetch()} />}
    {rockets.data && <RocketComparison rockets={filtered} comparison={comparison.data} selectedNames={selected} onSelect={toggleRocket} />}
    {comparison.isFetching && selected.length > 0 && <p className="text-xs text-muted">Updating comparison…</p>}
    {comparison.error && <ErrorState message={comparison.error instanceof Error ? comparison.error.message : "Comparison unavailable."} onRetry={() => void comparison.refetch()} />}
  </div>;
}
