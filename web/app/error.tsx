"use client";

export default function Error({ reset }: { error: Error & { digest?: string }; reset: () => void }) {
  return <div className="mx-auto max-w-3xl border border-danger/50 bg-danger-soft/40 p-8" role="alert"><h1 className="font-display text-xl font-semibold text-ink">This page could not load</h1><p className="mt-3 text-sm leading-6 text-muted">The failed route was isolated so the rest of Orbital can continue running.</p><button type="button" onClick={() => reset()} className="mt-5 border border-danger px-4 py-2 text-xs font-semibold uppercase tracking-[0.14em] text-danger">Try again</button></div>;
}
