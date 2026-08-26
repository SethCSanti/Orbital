"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import CountdownTimer from "@/components/ui/CountdownTimer";
import LaunchCard from "@/components/ui/LaunchCard";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";

export default function LaunchesPage() {
  const query = useQuery({ queryKey: ["launches", "upcoming"], queryFn: ({ signal }) => api.launches.upcoming(undefined, signal), staleTime: 900_000 });
  const launches = query.data ? [...query.data].sort((a, b) => new Date(a.net).getTime() - new Date(b.net).getTime()) : [];
  const featured = launches[0];

  return (
    <div className="mx-auto max-w-6xl">
      <header className="mb-8"><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-signal">Launch operations</p><h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">Upcoming launches</h1><p className="mt-3 max-w-2xl text-base leading-7 text-muted">The next confirmed liftoffs, ordered by scheduled NET. Countdown times use the launch timestamp returned by the backend.</p></header>
      {query.isPending && <LoadingState label="Loading launch manifest" />}
      {query.isError && <ErrorState message={query.error.message} onRetry={() => query.refetch()} />}
      {query.data && !featured && <p className="border border-line bg-panel p-8 text-muted">No upcoming launches are currently available.</p>}
      {featured && <>
        <section className="border border-signal/60 bg-panel p-6 sm:p-8">
          <div className="grid gap-8 lg:grid-cols-[minmax(0,1fr)_minmax(340px,0.8fr)] lg:items-end"><div><p className="text-xs font-semibold uppercase tracking-[0.18em] text-signal">Next on the pad</p><h2 className="mt-3 max-w-xl font-display text-2xl font-semibold text-ink sm:text-3xl">{featured.name}</h2><p className="mt-3 text-muted">{featured.rocketName} · {featured.missionName || "Mission details pending"}</p></div><CountdownTimer target={featured.net} /></div>
        </section>
        <section className="mt-10"><div className="mb-4 flex items-baseline justify-between gap-4"><h2 className="font-display text-xl font-semibold text-ink">Launch queue</h2><span className="text-xs text-dim">{launches.length} scheduled</span></div><div className="space-y-3">{launches.slice(1).map((launch, index) => <LaunchCard key={`${launch.externalId ?? `${launch.name}-${launch.net}`}-${index}`} launch={launch} />)}</div></section>
      </>}
    </div>
  );
}
