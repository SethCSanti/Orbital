"use client";

import Link from "next/link";

export default function Navbar() {
  return (
    <header className="sticky top-0 z-40 border-b border-line bg-orbit-950/95 backdrop-blur">
      <div className="mx-auto flex h-16 max-w-[1680px] items-center justify-between gap-6 px-4 sm:px-6 lg:px-10">
        <Link href="/" prefetch={false} className="group flex shrink-0 items-center gap-3" aria-label="Orbital home">
          <span className="relative flex h-8 w-8 items-center justify-center rounded-full border border-signal/70 text-signal">
            <span className="h-2 w-2 rounded-full bg-cyan" />
            <span className="absolute h-5 w-9 -rotate-[25deg] rounded-full border border-signal/50" />
          </span>
          <span>
            <span className="block font-display text-sm font-semibold uppercase tracking-[0.24em] text-ink">Orbital</span>
            <span className="hidden text-[10px] uppercase tracking-[0.2em] text-dim sm:block">Space intelligence</span>
          </span>
        </Link>

        <div className="hidden shrink-0 items-center gap-2 text-[10px] font-semibold uppercase tracking-[0.18em] text-dim md:flex">
          <span className="h-2 w-2 rounded-full bg-success" aria-hidden="true" />
          API online
        </div>
      </div>
    </header>
  );
}
