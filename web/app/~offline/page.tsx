import Link from "next/link";

export default function OfflinePage() {
  return (
    <section className="mx-auto max-w-3xl border border-line bg-panel p-8 sm:p-12">
      <p className="font-display text-xs font-semibold uppercase tracking-[0.2em] text-warning">Offline mode</p>
      <h1 className="mt-4 font-display text-3xl font-semibold tracking-tight text-ink">The signal is quiet.</h1>
      <p className="mt-4 max-w-xl text-base leading-7 text-muted">
        Orbital can still open its cached shell, but this view needs a connection for fresh mission data.
      </p>
      <Link href="/" className="mt-8 inline-flex border border-signal bg-signal/10 px-4 py-2 text-sm font-semibold text-signal-strong hover:bg-signal/20">
        Return to dashboard
      </Link>
    </section>
  );
}
