"use client";

import { useMemo, useState } from "react";
import * as d3 from "d3";
import TimelineEvent from "@/components/timeline/TimelineEvent";
import type { Launch } from "@/types/launch";
import type { Mission } from "@/types/mission";

export interface TimelineItem {
  id: string;
  date: Date;
  title: string;
  description: string;
  kind: "mission" | "launch";
  type: string;
  orbitAbbrev: string;
  rocketName?: string;
  metadata: string;
}

interface MissionTimelineProps {
  missions: Mission[];
  launches: Launch[];
  typeFilter: string;
  orbitFilter: string;
}

function missionDate(value: string | null): Date | null {
  if (!value) return null;
  const isoDate = new Date(value);
  if (!Number.isNaN(isoDate.valueOf())) return isoDate;
  const year = value.match(/\b(19|20)\d{2}\b/)?.[0];
  return year ? new Date(`${year}-01-01T00:00:00Z`) : null;
}

export function buildTimelineItems(missions: Mission[], launches: Launch[]): TimelineItem[] {
  const missionItems = missions.flatMap((mission, index) => {
    const date = missionDate(mission.launchDesignator);
    return date ? [{ id: `mission-${index}-${mission.name}`, date, title: mission.name, description: mission.description, kind: "mission" as const, type: mission.type, orbitAbbrev: mission.orbitAbbrev, metadata: mission.orbitName }] : [];
  });
  const launchItems = launches.flatMap((launch, index) => {
    const date = new Date(launch.net);
    return Number.isNaN(date.valueOf()) ? [] : [{ id: `launch-${index}-${launch.name}`, date, title: launch.name, description: launch.missionName, kind: "launch" as const, type: launch.statusName, orbitAbbrev: launch.orbitAbbrev, rocketName: launch.rocketName, metadata: launch.missionName }];
  });
  return [...missionItems, ...launchItems].sort((a, b) => a.date.valueOf() - b.date.valueOf());
}

export default function MissionTimeline({ missions, launches, typeFilter, orbitFilter }: MissionTimelineProps) {
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const allItems = useMemo(() => buildTimelineItems(missions, launches), [missions, launches]);
  const items = allItems.filter((item) => (!typeFilter || item.type === typeFilter) && (!orbitFilter || item.orbitAbbrev === orbitFilter));
  const selected = items.find((item) => item.id === selectedId) ?? items[0] ?? null;
  const domain = d3.extent(items, (item) => item.date) as [Date, Date] | [undefined, undefined];
  const start = domain[0] ?? new Date(Date.UTC(1957, 0, 1));
  const end = domain[1] ?? new Date();
  const x = d3.scaleTime().domain(start.valueOf() === end.valueOf() ? [start, new Date(end.valueOf() + 86_400_000)] : [start, end]).range([28, Math.max(420, items.length * 190)]);
  const width = Math.max(520, items.length * 190);

  return (
    <div className="space-y-5">
      <div className="overflow-x-auto border border-line bg-panel p-5">
        <svg viewBox={`0 0 ${width} 92`} className="mb-2 min-w-[520px]" role="img" aria-label="Mission and launch timeline">
          <line x1="28" x2={width - 28} y1="44" y2="44" stroke="var(--color-line)" />
          {x.ticks(Math.min(8, Math.max(2, items.length))).map((tick) => <g key={tick.valueOf()}><line x1={x(tick)} x2={x(tick)} y1="38" y2="50" stroke="var(--color-dim)" /><text x={x(tick)} y="72" textAnchor="middle" fill="var(--color-muted)" fontSize="10">{tick.getUTCFullYear()}</text></g>)}
        </svg>
        <div className="flex min-w-[520px] gap-0" role="list" aria-label="Timeline events">
          {items.map((item) => <TimelineEvent key={item.id} event={item} selected={selected?.id === item.id} onSelect={() => setSelectedId(item.id)} />)}
        </div>
        {!items.length && <p className="py-8 text-sm text-muted">No events match the current filters.</p>}
      </div>
      {selected && (
        <section className="border border-line border-l-signal bg-panel p-6" aria-live="polite">
          <p className="font-display text-xs font-semibold uppercase tracking-[0.18em] text-cyan">{selected.kind} · {selected.date.toLocaleDateString()}</p>
          <h2 className="mt-2 font-display text-2xl font-semibold text-ink">{selected.title}</h2>
          <p className="mt-3 max-w-3xl text-sm leading-6 text-muted">{selected.description || "No description supplied by the mission feed."}</p>
          <div className="mt-5 flex flex-wrap gap-x-6 gap-y-2 text-xs uppercase tracking-[0.1em] text-dim"><span>{selected.type}</span><span>{selected.orbitAbbrev || "Orbit not supplied"}</span><span>{selected.metadata}</span>{selected.rocketName && <span>{selected.rocketName}</span>}</div>
        </section>
      )}
    </div>
  );
}
