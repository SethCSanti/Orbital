import type { Astronaut } from "@/types/astronaut";

const nationalityFlags: Record<string, string> = {
  australia: "🇦🇺",
  canada: "🇨🇦",
  china: "🇨🇳",
  france: "🇫🇷",
  germany: "🇩🇪",
  india: "🇮🇳",
  italy: "🇮🇹",
  japan: "🇯🇵",
  russia: "🇷🇺",
  "united kingdom": "🇬🇧",
  uk: "🇬🇧",
  "united states": "🇺🇸",
  usa: "🇺🇸",
};

function flagFor(nationality: string | null) {
  if (!nationality) return "🌐";
  return nationalityFlags[nationality.toLowerCase()] ?? "🌐";
}

export default function AstronautCard({ astronaut }: { astronaut: Astronaut }) {
  const initials = astronaut.name.split(" ").map((part) => part[0]).join("").slice(0, 2).toUpperCase();

  return (
    <article className="group border border-line bg-panel p-4 transition-colors hover:border-signal/60">
      <div className="flex items-start gap-4">
        <div className="h-20 w-20 shrink-0 overflow-hidden bg-orbit-800">
          {astronaut.profileImageUrl ? (
            <img src={astronaut.profileImageUrl} alt="" className="h-full w-full object-cover grayscale transition duration-300 group-hover:grayscale-0" loading="lazy" />
          ) : (
            <div className="flex h-full w-full items-center justify-center font-display text-lg font-semibold text-signal">{initials}</div>
          )}
        </div>
        <div className="min-w-0">
          <h2 className="font-display text-lg font-semibold text-ink">{astronaut.name}</h2>
          <p className="mt-1 text-sm text-muted">{flagFor(astronaut.nationality)} {astronaut.nationality ?? "Nationality not listed"}</p>
          <p className="mt-4 text-xs font-semibold uppercase tracking-[0.14em] text-dim">Flights completed</p>
          <p className="mt-1 font-display text-xl text-signal-strong">{astronaut.flightsCount}</p>
        </div>
      </div>
      {astronaut.biography && <p className="mt-4 line-clamp-3 text-sm leading-6 text-muted">{astronaut.biography}</p>}
    </article>
  );
}
