"use client";

export default function Error({ reset }: { error: Error & { digest?: string }; reset: () => void }) { return <div className="border border-danger/50 bg-danger-soft/40 p-8" role="alert"><p className="text-sm text-ink">The ISS visualization failed to load.</p><button type="button" onClick={() => reset()} className="mt-4 border border-danger px-3 py-2 text-xs uppercase tracking-[0.14em] text-danger">Retry globe</button></div>; }
