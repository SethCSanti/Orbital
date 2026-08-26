import type { Launch } from "@/types/launch";

export default function LaunchCard({ launch, featured = false }: { launch: Launch; featured?: boolean }) {
  return (
    <article className={`border bg-panel p-5 ${featured ? "border-signal/60" : "border-line"}`}>
      <div className="flex flex-wrap items-start justify-between gap-4">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-signal">{launch.rocketName || "Vehicle unlisted"}</p>
          <h2 className="mt-2 font-display text-lg font-semibold text-ink">{launch.name}</h2>
        </div>
        <span className="border border-line bg-orbit-900 px-2 py-1 text-[10px] font-semibold uppercase tracking-[0.14em] text-muted">{launch.statusName}</span>
      </div>
      <dl className="mt-5 grid grid-cols-2 gap-x-5 gap-y-4 text-sm sm:grid-cols-3">
        <div><dt className="text-xs text-dim">Mission</dt><dd className="mt-1 text-muted">{launch.missionName || "—"}</dd></div>
        <div><dt className="text-xs text-dim">Orbit</dt><dd className="mt-1 text-muted">{launch.orbitAbbrev || "—"}</dd></div>
        <div><dt className="text-xs text-dim">Scheduled</dt><dd className="mt-1 text-muted"><time dateTime={launch.net}>{new Date(launch.net).toLocaleString()}</time></dd></div>
      </dl>
    </article>
  );
}
