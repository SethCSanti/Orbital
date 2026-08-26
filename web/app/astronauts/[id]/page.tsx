"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import RelatedLaunches from "@/components/catalog/RelatedLaunches";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { api } from "@/lib/api";

export default function AstronautDetailPage() {
  const id = Number(useParams<{ id: string }>().id);
  const query = useQuery({ queryKey: ["astronaut", id], queryFn: ({ signal }) => api.astronauts.byId(id, signal), enabled: Number.isInteger(id) });
  if (query.isPending) return <LoadingState label="Loading astronaut record" />;
  if (query.isError) return <ErrorState message={query.error.message} onRetry={() => void query.refetch()} />;
  if (!query.data) return null;
  const { astronaut, launches } = query.data;
  return <div className="mx-auto max-w-5xl"><Link href="/astronauts" prefetch={false} className="text-xs uppercase tracking-[0.14em] text-signal">← Astronaut archive</Link><header className="mt-6 border border-line bg-panel p-6"><p className="font-display text-xs uppercase tracking-[0.18em] text-cyan">Crew record</p><h1 className="mt-2 font-display text-3xl font-semibold text-ink">{astronaut.name}</h1><p className="mt-2 text-muted">{astronaut.nationality ?? "Nationality not listed"} · {astronaut.flightsCount} flights</p>{astronaut.biography && <p className="mt-5 max-w-3xl text-sm leading-7 text-muted">{astronaut.biography}</p>}{astronaut.wikipediaUrl && <a href={astronaut.wikipediaUrl} target="_blank" rel="noreferrer" className="mt-4 inline-block text-sm text-signal">Wikipedia ↗</a>}</header><RelatedLaunches launches={launches} /></div>;
}
