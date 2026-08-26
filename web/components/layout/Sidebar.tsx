"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";

const groups = [
  {
    label: "Observe",
    links: [
        { href: "/iss-tracker", label: "ISS tracker", hint: "Live position" },
        { href: "/launches", label: "Launches", hint: "Countdowns" },
        { href: "/solar-system", label: "Solar system", hint: "Planetary positions" },
    ],
  },
  {
    label: "Explore",
    links: [
      { href: "/missions", label: "Missions", hint: "Stations and history" },
      { href: "/asteroids", label: "Asteroids", hint: "Near-Earth objects" },
      { href: "/apod", label: "APOD", hint: "Daily astronomy" },
      { href: "/rockets", label: "Rockets", hint: "Vehicle library" },
    ],
  },
];

export default function Sidebar() {
  const pathname = usePathname();

  return (
    <aside className="hidden w-64 shrink-0 border-r border-line px-5 pb-10 pt-9 lg:block" aria-label="Feature navigation">
      <div className="mb-7 border-b border-line pb-6">
        <p className="font-display text-[11px] font-semibold uppercase tracking-[0.22em] text-signal">Mission control</p>
        <p className="mt-2 max-w-[18ch] text-sm leading-5 text-muted">A working view of the sky, its missions, and its machines.</p>
      </div>
      <nav className="space-y-8">
        {groups.map((group) => (
          <div key={group.label}>
            <p className="mb-3 px-3 text-[10px] font-semibold uppercase tracking-[0.2em] text-dim">{group.label}</p>
            <div className="space-y-1">
              {group.links.map((link) => {
                const active = pathname.startsWith(link.href);
                return (
                  <Link
                    key={link.href}
                    href={link.href}
                    prefetch={false}
                    aria-current={active ? "page" : undefined}
                    className={`block border-l-2 px-3 py-2.5 transition-colors ${
                      active ? "border-signal bg-signal/10 text-ink" : "border-transparent text-muted hover:border-line hover:bg-panel/70 hover:text-ink"
                    }`}
                  >
                    <span className="block font-display text-sm font-medium">{link.label}</span>
                    <span className="mt-0.5 block text-xs text-dim">{link.hint}</span>
                  </Link>
                );
              })}
            </div>
          </div>
        ))}
      </nav>
    </aside>
  );
}
