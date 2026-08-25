"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import AstronautCard from "@/components/ui/AstronautCard";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";

export default function AstronautsPage() {
  const query = useQuery({ queryKey: ["astronauts"], queryFn: api.astronauts.all, staleTime: 3_600_000 });

  return (
    <div className="mx-auto max-w-6xl">
      <header className="mb-8">
        <p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-signal">Crew manifest</p>
        <div className="mt-3 flex flex-wrap items-end justify-between gap-4">
          <div>
            <h1 className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">Who’s in space</h1>
            <p className="mt-3 max-w-2xl text-base leading-7 text-muted">The current astronaut catalogue, refreshed from the backend’s people-in-space feed.</p>
          </div>
          {query.data && <p className="font-display text-3xl text-signal-strong">{query.data.length}<span className="ml-2 text-sm text-muted">people</span></p>}
        </div>
      </header>
      {query.isPending && <LoadingState label="Loading astronaut manifest" />}
      {query.isError && <ErrorState message={query.error.message} onRetry={() => query.refetch()} />}
      {query.data && query.data.length === 0 && <p className="border border-line bg-panel p-8 text-muted">No astronauts are currently listed.</p>}
      {query.data && query.data.length > 0 && <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">{query.data.map((astronaut) => <AstronautCard key={astronaut.name} astronaut={astronaut} />)}</div>}
    </div>
  );
}
