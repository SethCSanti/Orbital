"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import AstronautCard from "@/components/ui/AstronautCard";
import CatalogStatus from "@/components/catalog/CatalogStatus";
import Pagination from "@/components/catalog/Pagination";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";

export default function AstronautsPage() {
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const query = useQuery({
    queryKey: ["astronauts", { page, search }],
    queryFn: ({ signal }) => api.astronauts.all({ page, pageSize: 24, search }, signal),
    placeholderData: (previous) => previous,
    staleTime: 5 * 60_000,
  });
  useEffect(() => setPage(1), [search]);

  return <div className="mx-auto max-w-6xl">
    <header className="mb-8"><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-signal">Crew manifest</p><div className="mt-3 flex flex-wrap items-end justify-between gap-4"><div><h1 className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">Astronaut archive</h1><p className="mt-3 max-w-2xl text-base leading-7 text-muted">Search the historical crew manifest and open a record for its launch history.</p></div><CatalogStatus /></div></header>
    <label className="mb-6 block max-w-xl text-xs uppercase tracking-[0.14em] text-muted">Search astronauts<input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Name or nationality" className="mt-2 w-full border border-line bg-panel px-4 py-3 text-sm normal-case tracking-normal text-ink placeholder:text-dim" /></label>
    {query.isPending && <LoadingState label="Loading astronaut archive" />}
    {query.isError && <ErrorState message={query.error.message} onRetry={() => void query.refetch()} />}
    {query.data?.items.length === 0 && <p className="border border-line bg-panel p-8 text-muted">No astronauts match this search.</p>}
    {query.data && query.data.items.length > 0 && <><div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">{query.data.items.map((astronaut) => <Link key={astronaut.id} href={`/astronauts/${astronaut.id}`} prefetch={false}><AstronautCard astronaut={astronaut} /></Link>)}</div><div className="mt-6"><Pagination page={query.data.page} pageSize={query.data.pageSize} total={query.data.total} onChange={setPage} /></div></>}
  </div>;
}
