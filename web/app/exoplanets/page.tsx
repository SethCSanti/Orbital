"use client";

import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import ExoplanetChart from "@/components/charts/ExoplanetChart";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { api } from "@/lib/api";

export default function ExoplanetsPage() {
  const [discoveryMethod, setDiscoveryMethod] = useState("");
  const [minYear, setMinYear] = useState("");
  const [maxYear, setMaxYear] = useState("");
  const filters = { discoveryMethod: discoveryMethod || undefined, minYear: minYear ? Number(minYear) : undefined, maxYear: maxYear ? Number(maxYear) : undefined };
  const query = useQuery({ queryKey: ["exoplanets", filters], queryFn: () => api.exoplanets.all(filters), staleTime: 86_400_000 });
  const methods = useMemo(() => [...new Set((query.data ?? []).map((planet) => planet.discoveryMethod).filter(Boolean))].sort(), [query.data]);

  return <div className="mx-auto max-w-[1500px] space-y-8 px-5 py-8 lg:px-10"><header className="max-w-3xl"><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-cyan">Confirmed worlds</p><h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink md:text-5xl">Exoplanet explorer</h1><p className="mt-4 text-base leading-7 text-muted">Compare discovered worlds by orbital distance and radius, with incomplete records kept visible rather than silently discarded.</p></header><div className="flex flex-wrap items-end gap-3 border-y border-line py-4"><label className="text-xs uppercase tracking-[0.12em] text-muted">Discovery method<select value={discoveryMethod} onChange={(event) => setDiscoveryMethod(event.target.value)} className="mt-2 block border border-line bg-panel px-3 py-2 text-sm normal-case tracking-normal text-ink"><option value="">All methods</option>{methods.map((method) => <option key={method}>{method}</option>)}</select></label><label className="text-xs uppercase tracking-[0.12em] text-muted">From year<input type="number" value={minYear} onChange={(event) => setMinYear(event.target.value)} className="mt-2 block w-28 border border-line bg-panel px-3 py-2 text-sm normal-case tracking-normal text-ink" /></label><label className="text-xs uppercase tracking-[0.12em] text-muted">To year<input type="number" value={maxYear} onChange={(event) => setMaxYear(event.target.value)} className="mt-2 block w-28 border border-line bg-panel px-3 py-2 text-sm normal-case tracking-normal text-ink" /></label></div>{query.isLoading && <LoadingState label="Loading exoplanet catalogue" />}{query.error && <ErrorState message={query.error instanceof Error ? query.error.message : "Exoplanet catalogue unavailable."} onRetry={() => void query.refetch()} />}{query.data && <ExoplanetChart planets={query.data} />}</div>;
}
