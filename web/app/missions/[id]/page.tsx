"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import RelatedLaunches from "@/components/catalog/RelatedLaunches";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { api } from "@/lib/api";

export default function MissionDetailPage() {
  const id = Number(useParams<{ id: string }>().id);
  const query = useQuery({ queryKey: ["mission", id], queryFn: ({ signal }) => api.missions.byId(id, signal), enabled: Number.isInteger(id) });
  if (query.isPending) return <LoadingState label="Loading mission record" />;
  if (query.isError) return <ErrorState message={query.error.message} onRetry={() => void query.refetch()} />;
  if (!query.data) return null;
  const { mission, launches } = query.data;
  return <div className="mx-auto max-w-5xl"><Link href="/missions" prefetch={false} className="text-xs uppercase tracking-[0.14em] text-signal">← Mission archive</Link><header className="mt-6 border border-line bg-panel p-6"><p className="font-display text-xs uppercase tracking-[0.18em] text-cyan">Mission record</p><h1 className="mt-2 font-display text-3xl font-semibold text-ink">{mission.name}</h1><p className="mt-2 text-muted">{mission.type || "Type unlisted"} · {mission.orbitAbbrev || mission.orbitName || "Orbit unlisted"}</p>{mission.description && <p className="mt-5 max-w-3xl text-sm leading-7 text-muted">{mission.description}</p>}{mission.sourceUrl && <a href={mission.sourceUrl} target="_blank" rel="noreferrer" className="mt-4 inline-block text-sm text-signal">Source record ↗</a>}</header><RelatedLaunches launches={launches} /></div>;
}
