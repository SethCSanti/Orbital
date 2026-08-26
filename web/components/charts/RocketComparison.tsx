"use client";

import type { Rocket } from "@/types/rocket";
import Link from "next/link";

interface RocketComparisonProps {
  rockets: Rocket[];
  comparison?: Rocket[];
  selectedIds: number[];
  onSelect: (id: number) => void;
}

const MAX_HEIGHT = 220;

function formatNumber(value: number | null, suffix = "") {
  return value === null ? "—" : `${value.toLocaleString()}${suffix}`;
}

export default function RocketComparison({ rockets, comparison, selectedIds, onSelect }: RocketComparisonProps) {
  const selected = (comparison ?? rockets).filter((rocket) => selectedIds.includes(rocket.id ?? -1));
  const maxLength = Math.max(...selected.map((rocket) => rocket.length), 1);
  const stats: Array<[string, (rocket: Rocket) => string]> = [
    ["Full name", (rocket) => rocket.fullName],
    ["Active", (rocket) => rocket.active ? "Yes" : "No"],
    ["Reusable", (rocket) => rocket.reusable ? "Yes" : "No"],
    ["Length", (rocket) => formatNumber(rocket.length, " m")],
    ["Launch cost", (rocket) => formatNumber(rocket.launchCost, " USD")],
    ["Launch mass", (rocket) => formatNumber(rocket.launchMass, " kg")],
    ["LEO capacity", (rocket) => formatNumber(rocket.leoCapacity, " kg")],
    ["GTO capacity", (rocket) => formatNumber(rocket.gtoCapacity, " kg")],
    ["Total launches", (rocket) => formatNumber(rocket.totalLaunchCount)],
    ["Successful launches", (rocket) => formatNumber(rocket.successfulLaunchCount)],
    ["Failed launches", (rocket) => formatNumber(rocket.failedLaunchCount)],
  ];

  return <div className="space-y-6">
    <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4" aria-label="Rocket selector">
      {rockets.map((rocket) => { const id = rocket.id ?? -1; return <label key={id} className={`flex cursor-pointer items-start gap-3 border p-4 transition-colors ${selectedIds.includes(id) ? "border-signal bg-panel-raised" : "border-line bg-panel hover:border-signal/60"}`}><input type="checkbox" checked={selectedIds.includes(id)} onChange={() => onSelect(id)} className="mt-1 accent-signal" /><span><Link href={`/rockets/${id}`} prefetch={false} className="block font-display text-sm font-semibold text-ink hover:text-signal">{rocket.name}</Link><span className="mt-1 block text-xs text-muted">{rocket.family} · {rocket.lastLaunchDate ? new Date(rocket.lastLaunchDate).toLocaleDateString() : rocket.maidenFlight || "Date unknown"}</span></span></label>; })}
    </div>
    {selected.length > 0 && <div className="overflow-x-auto border border-line bg-panel p-5"><div className="flex min-w-[620px] items-end justify-around gap-5 border-b border-line pb-5" aria-label="Rocket silhouettes to scale">{selected.map((rocket) => <div key={rocket.id ?? rocket.name} className="flex min-w-[120px] flex-1 flex-col items-center justify-end"><span className="mb-3 text-center text-xs text-muted">{rocket.length} m</span><button type="button" onClick={() => onSelect(rocket.id ?? -1)} className="relative w-14 bg-signal/75 transition-colors hover:bg-signal" style={{ height: `${Math.max(28, rocket.length / maxLength * MAX_HEIGHT)}px` }} aria-label={`Deselect ${rocket.name}`}><span className="absolute -top-3 left-1/2 h-4 w-4 -translate-x-1/2 rounded-full bg-signal-strong" /><span className="absolute inset-x-[-6px] bottom-0 h-3 bg-signal-strong/45" /></button><span className="mt-3 text-center font-display text-sm font-semibold text-ink">{rocket.name}</span></div>)}</div><div className="mt-5 min-w-[620px] overflow-hidden"><table className="w-full border-collapse text-left text-sm"><thead><tr className="border-b border-line text-xs uppercase tracking-[0.12em] text-muted"><th className="p-3">Measure</th>{selected.map((rocket) => <th className="p-3" key={rocket.id ?? rocket.name}>{rocket.name}</th>)}</tr></thead><tbody>{stats.map(([label, getter]) => <tr key={label} className="border-b border-line/70"><th className="p-3 font-normal text-muted">{label}</th>{selected.map((rocket) => <td className="p-3 text-ink" key={`${label}-${rocket.id ?? rocket.name}`}>{getter(rocket)}</td>)}</tr>)}</tbody></table></div></div>}
  </div>;
}
