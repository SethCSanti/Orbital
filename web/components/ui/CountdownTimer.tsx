"use client";

import { useLaunchCountdown } from "@/hooks/useLaunchCountdown";

const labels = [
  ["days", "Days"],
  ["hours", "Hours"],
  ["minutes", "Minutes"],
  ["seconds", "Seconds"],
] as const;

export default function CountdownTimer({ target }: { target: string }) {
  const countdown = useLaunchCountdown(target);

  return (
    <div aria-label={`Countdown to launch on ${new Date(target).toLocaleString()}`}>
      <div className="grid grid-cols-4 divide-x divide-line border-y border-line">
        {labels.map(([key, label]) => (
          <div key={key} className="px-3 py-4 text-center first:pl-0 last:pr-0 sm:px-5">
            <p className="font-display text-2xl font-semibold tabular-nums text-signal-strong sm:text-4xl">{String(countdown[key]).padStart(2, "0")}</p>
            <p className="mt-1 text-[10px] font-semibold uppercase tracking-[0.16em] text-dim">{label}</p>
          </div>
        ))}
      </div>
      <p className="mt-3 text-xs uppercase tracking-[0.14em] text-muted">{countdown.isPast ? "Launch window reached" : "Time to scheduled launch"}</p>
    </div>
  );
}
