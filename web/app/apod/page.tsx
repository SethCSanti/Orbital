"use client";

import { useState, type FormEvent } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import ApodViewer from "@/components/ui/ApodViewer";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";

export default function ApodPage() {
  const [dateInput, setDateInput] = useState("");
  const [selectedDate, setSelectedDate] = useState("");
  const query = useQuery({
    queryKey: ["apod", selectedDate || "latest"],
    queryFn: () => selectedDate ? api.apod.byDate(selectedDate) : api.apod.latest(),
    staleTime: 86_400_000,
  });

  const submitDate = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setSelectedDate(dateInput);
  };

  return (
    <div className="mx-auto max-w-6xl">
      <header className="mb-8 flex flex-wrap items-end justify-between gap-6">
        <div>
          <p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-signal">Daily sky log</p>
          <h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">Astronomy picture of the day</h1>
          <p className="mt-3 max-w-2xl text-base leading-7 text-muted">One image, video, or observation from NASA’s archive, with the story behind it.</p>
        </div>
        <form onSubmit={submitDate} className="flex items-end gap-2 border border-line bg-panel p-3">
          <label className="text-xs font-semibold uppercase tracking-[0.14em] text-dim">
            Browse date
            <input type="date" value={dateInput} onChange={(event) => setDateInput(event.target.value)} className="mt-2 block border border-line bg-orbit-900 px-3 py-2 text-sm text-ink" />
          </label>
          <button type="submit" disabled={!dateInput} className="border border-signal px-3 py-2 text-xs font-semibold uppercase tracking-[0.12em] text-signal-strong disabled:cursor-not-allowed disabled:opacity-40">View</button>
        </form>
      </header>
      {query.isPending && <LoadingState label="Loading NASA image" />}
      {query.isError && <ErrorState message={query.error.message} onRetry={() => query.refetch()} />}
      {query.data && <ApodViewer entry={query.data} />}
      {selectedDate && query.data && <button onClick={() => { setDateInput(""); setSelectedDate(""); }} className="mt-4 text-sm font-semibold text-signal hover:text-signal-strong">Return to latest APOD</button>}
    </div>
  );
}
