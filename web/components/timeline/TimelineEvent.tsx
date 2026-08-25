import type { TimelineItem } from "@/components/timeline/MissionTimeline";

export function TimelineEvent({ event, selected, onSelect }: { event: TimelineItem; selected: boolean; onSelect: () => void }) {
  return (
    <button
      type="button"
      onClick={onSelect}
      aria-pressed={selected}
      className={`group relative min-w-[180px] text-left ${selected ? "text-signal-strong" : "text-ink"}`}
    >
      <span className={`absolute left-0 top-[19px] h-3 w-3 rounded-full border-2 ${selected ? "border-signal bg-signal" : "border-line bg-orbit-900 group-hover:border-signal"}`} />
      <span className="block pl-7 font-display text-xs font-semibold uppercase tracking-[0.12em] text-muted">{event.date.getUTCFullYear()}</span>
      <span className="mt-2 block border-l border-line pb-5 pl-7 pr-4 text-sm leading-5 group-hover:text-signal-strong">{event.title}</span>
      <span className="block pl-7 text-xs text-dim">{event.kind === "launch" ? event.rocketName : event.type}</span>
    </button>
  );
}

export default TimelineEvent;
