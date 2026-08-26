"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import RocketComparison from "@/components/charts/RocketComparison";
import CatalogStatus from "@/components/catalog/CatalogStatus";
import Pagination from "@/components/catalog/Pagination";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { api } from "@/lib/api";
import { toggleRocketId } from "@/lib/rocketSelection";

export default function RocketsPage() {
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [selectedIds, setSelectedIds] = useState<number[]>([]);
  const rockets = useQuery({
    queryKey: ["rockets", { page, search }],
    queryFn: ({ signal }) => api.rockets.all({ page, pageSize: 24, search }, signal),
    placeholderData: (previous) => previous,
  });
  useEffect(() => setPage(1), [search]);
  const selected = selectedIds.slice(0, 4);
  const comparison = useQuery({ queryKey: ["rockets", "compare", selected], queryFn: ({ signal }) => api.rockets.compare(selected, signal), enabled: selected.length > 0, staleTime: 86_400_000 });

  function toggleRocket(id: number) {
    if (id < 0) return;
    setSelectedIds((current) => toggleRocketId(current, id));
  }

  return <div className="mx-auto max-w-[1500px] space-y-8 px-5 py-8 lg:px-10">
    <header className="flex flex-wrap items-end justify-between gap-4"><div className="max-w-3xl"><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-cyan">Vehicle reference</p><h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink md:text-5xl">Rocket archive</h1><p className="mt-4 text-base leading-7 text-muted">Search historical launch vehicles, compare up to four, and open a vehicle’s launch record.</p></div><CatalogStatus /></header>
    <label className="block max-w-xl text-xs uppercase tracking-[0.14em] text-muted">Search rockets<input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Falcon, Ariane, Soyuz…" className="mt-2 w-full border border-line bg-panel px-4 py-3 text-sm normal-case tracking-normal text-ink placeholder:text-dim" /></label>
    {rockets.isPending && <LoadingState label="Loading rocket archive" />}
    {rockets.error && <ErrorState message={rockets.error instanceof Error ? rockets.error.message : "Rocket archive unavailable."} onRetry={() => void rockets.refetch()} />}
    {rockets.data && <><RocketComparison rockets={rockets.data.items} comparison={comparison.data} selectedIds={selected} onSelect={toggleRocket} /><Pagination page={rockets.data.page} pageSize={rockets.data.pageSize} total={rockets.data.total} onChange={setPage} /></>}
    {comparison.isFetching && selected.length > 0 && <p className="text-xs text-muted">Updating comparison…</p>}
    {comparison.error && <ErrorState message={comparison.error instanceof Error ? comparison.error.message : "Comparison unavailable."} onRetry={() => void comparison.refetch()} />}
  </div>;
}
