"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import RelatedLaunches from "@/components/catalog/RelatedLaunches";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { api } from "@/lib/api";

export default function RocketDetailPage() {
  const id = Number(useParams<{ id: string }>().id);
  const query = useQuery({ queryKey: ["rocket", id], queryFn: ({ signal }) => api.rockets.byId(id, signal), enabled: Number.isInteger(id) });
  if (query.isPending) return <LoadingState label="Loading rocket record" />;
  if (query.isError) return <ErrorState message={query.error.message} onRetry={() => void query.refetch()} />;
  if (!query.data) return null;
  const { rocket, launches } = query.data;
  return <div className="mx-auto max-w-5xl"><Link href="/rockets" prefetch={false} className="text-xs uppercase tracking-[0.14em] text-signal">← Rocket archive</Link><header className="mt-6 border border-line bg-panel p-6"><p className="font-display text-xs uppercase tracking-[0.18em] text-cyan">Vehicle record</p><h1 className="mt-2 font-display text-3xl font-semibold text-ink">{rocket.fullName || rocket.name}</h1><p className="mt-2 text-muted">{rocket.family} · {rocket.variant || "Variant unlisted"}</p><div className="mt-6 grid gap-4 sm:grid-cols-3 text-sm text-muted"><span>Length <strong className="block text-ink">{rocket.length} m</strong></span><span>LEO capacity <strong className="block text-ink">{rocket.leoCapacity.toLocaleString()} kg</strong></span><span>Successful launches <strong className="block text-ink">{rocket.successfulLaunchCount}</strong></span></div>{rocket.description && <p className="mt-5 max-w-3xl text-sm leading-7 text-muted">{rocket.description}</p>}{rocket.wikiUrl && <a href={rocket.wikiUrl} target="_blank" rel="noreferrer" className="mt-4 inline-block text-sm text-signal">Source profile ↗</a>}</header><RelatedLaunches launches={launches} /></div>;
}
