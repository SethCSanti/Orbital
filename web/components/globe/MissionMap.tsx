import type { Mission } from "@/types/mission";
import type { SpaceStation } from "@/types/spaceStation";
import Link from "next/link";

const stationCoordinates: Record<string, { left: number; top: number }> = {
  iss: { left: 45, top: 43 },
  tiangong: { left: 52, top: 47 },
  mir: { left: 47, top: 42 },
};

function coordinatesFor(name: string) {
  const key = name.toLowerCase();
  return Object.entries(stationCoordinates).find(([station]) => key.includes(station))?.[1];
}

export default function MissionMap({ stations, missions }: { stations: SpaceStation[]; missions: Mission[] }) {
  return (
    <div className="space-y-6">
      <section className="border border-line bg-panel p-4 sm:p-6">
        <div className="flex flex-wrap items-end justify-between gap-3">
          <div>
            <p className="font-display text-xs font-semibold uppercase tracking-[0.18em] text-signal">Station atlas</p>
            <h2 className="mt-2 font-display text-xl font-semibold text-ink">Human outposts</h2>
          </div>
          <p className="max-w-xs text-right text-xs leading-5 text-dim">The station DTO has no coordinates, so known positions are shown as an approximate reference layer.</p>
        </div>
        {stations.length === 0 ? <div className="mt-6 border border-cyan/40 bg-cyan/5 p-6 text-sm leading-6 text-muted" role="status">Station data is still syncing from SpaceDevs. Refresh this page after the initial station sync completes.</div> : <div className="relative mt-6 h-64 overflow-hidden border border-line bg-orbit-900" aria-label="Approximate station position map" role="img">
          <div className="absolute inset-0 opacity-50" style={{ backgroundImage: "linear-gradient(var(--color-line) 1px, transparent 1px), linear-gradient(90deg, var(--color-line) 1px, transparent 1px)", backgroundSize: "10% 25%" }} />
          <div className="absolute left-0 right-0 top-1/2 border-t border-dashed border-line" />
          <div className="absolute bottom-2 left-3 text-[10px] uppercase tracking-[0.16em] text-dim">Approximate longitude / latitude grid</div>
          {stations.map((station) => {
            const position = coordinatesFor(station.name);
            if (!position) return null;
            return (
              <div key={station.name} className="absolute -translate-x-1/2 -translate-y-1/2" style={{ left: `${position.left}%`, top: `${position.top}%` }} title={station.name}>
                <span className="block h-3 w-3 rounded-full border-2 border-orbit-950 bg-cyan shadow-[0_0_0_4px_rgba(104,216,214,0.18)]" />
                <span className="mt-2 block -translate-x-1/4 whitespace-nowrap text-[10px] font-semibold text-cyan">{station.name}</span>
              </div>
            );
          })}
        </div>}
      </section>

      {stations.length > 0 && <div className="grid gap-4 md:grid-cols-2">
        {stations.map((station) => (
          <article key={station.name} className="border border-line bg-panel p-5">
            <div className="flex items-start justify-between gap-4">
              <h3 className="font-display text-lg font-semibold text-ink">{station.name}</h3>
              <span className="text-xs font-semibold uppercase tracking-[0.14em] text-success">{station.status}</span>
            </div>
            <p className="mt-2 text-sm text-muted">{station.type} · {station.orbit}</p>
            <p className="mt-4 line-clamp-3 text-sm leading-6 text-muted">{station.description || "No description available."}</p>
          </article>
        ))}
      </div>}

      <section className="border border-line bg-panel p-5 sm:p-6">
        <div className="flex items-baseline justify-between gap-4 border-b border-line pb-4">
          <h2 className="font-display text-xl font-semibold text-ink">Mission catalogue</h2>
          <span className="text-xs text-dim">{missions.length} matching missions</span>
        </div>
        <div className="divide-y divide-line">
          {missions.map((mission, index) => (
            <article key={`${mission.id ?? mission.sourceId ?? `${mission.name}-${mission.launchDesignator ?? "mission"}`}-${index}`} className="grid gap-2 py-4 sm:grid-cols-[minmax(0,1fr)_140px]">
              <div>
                <h3 className="font-display font-medium text-ink"><Link href={`/missions/${mission.id}`} prefetch={false} className="hover:text-signal">{mission.name}</Link></h3>
                <p className="mt-1 line-clamp-2 text-sm leading-6 text-muted">{mission.description || "No description available."}</p>
              </div>
              <div className="text-left text-xs text-dim sm:text-right">
                <p>{mission.type || "Type unlisted"}</p>
                <p className="mt-1 text-signal">{mission.orbitAbbrev || mission.orbitName || "Orbit unlisted"}</p>
              </div>
            </article>
          ))}
        </div>
      </section>
    </div>
  );
}
