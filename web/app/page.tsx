import Link from "next/link";

const quickLinks = [
  ["/launches", "Launch control", "Upcoming missions and the next scheduled liftoff."],
  ["/iss-tracker", "ISS tracker", "Watch the station’s live position stream."],
  ["/asteroids", "Near-Earth objects", "Compare close approaches by distance and size."],
  ["/apod", "Today’s sky", "NASA’s astronomy picture and archive."],
] as const;

export default function HomePage() {
  return (
    <div className="mx-auto max-w-6xl">
      <section className="border-b border-line pb-10 pt-4 sm:pt-10">
        <p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-signal">Orbital / live workspace</p>
        <h1 className="mt-4 max-w-3xl font-display text-4xl font-semibold tracking-tight text-ink sm:text-6xl">Read the sky in motion.</h1>
        <p className="mt-5 max-w-2xl text-lg leading-8 text-muted">A practical view of launches, people in orbit, planetary bodies, and the objects passing close to Earth.</p>
      </section>
      <section className="grid gap-px border-x border-b border-line bg-line sm:grid-cols-2" aria-label="Feature shortcuts">
        {quickLinks.map(([href, title, description]) => (
          <Link key={href} href={href} className="group bg-orbit-950 p-6 transition-colors hover:bg-panel sm:p-8">
            <p className="text-xs font-semibold uppercase tracking-[0.18em] text-dim">Open module ↗</p>
            <h2 className="mt-8 font-display text-xl font-semibold text-ink group-hover:text-signal-strong">{title}</h2>
            <p className="mt-2 max-w-sm text-sm leading-6 text-muted">{description}</p>
          </Link>
        ))}
      </section>
    </div>
  );
}
