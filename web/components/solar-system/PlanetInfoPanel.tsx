"use client";

const PLANET_FACTS: Record<string, { diameter: string; mass: string; moons: string }> = {
  Mercury: { diameter: "4,879 km", mass: "3.30 × 10²³ kg", moons: "0" },
  Venus: { diameter: "12,104 km", mass: "4.87 × 10²⁴ kg", moons: "0" },
  Earth: { diameter: "12,742 km", mass: "5.97 × 10²⁴ kg", moons: "1" },
  Mars: { diameter: "6,779 km", mass: "6.42 × 10²³ kg", moons: "2" },
  Jupiter: { diameter: "139,820 km", mass: "1.90 × 10²⁷ kg", moons: "95" },
  Saturn: { diameter: "116,460 km", mass: "5.68 × 10²⁶ kg", moons: "146" },
  Uranus: { diameter: "50,724 km", mass: "8.68 × 10²⁵ kg", moons: "28" },
  Neptune: { diameter: "49,244 km", mass: "1.02 × 10²⁶ kg", moons: "16" },
};

export default function PlanetInfoPanel({ name, orbitalPeriodDays, distanceAu, onClose }: { name: string | null; orbitalPeriodDays?: number; distanceAu?: number; onClose: () => void }) {
  if (!name) return null;
  const facts = PLANET_FACTS[name];
  if (!facts) return null;
  return <aside className="absolute bottom-4 left-4 z-10 w-[min(320px,calc(100%-2rem))] border border-line border-l-signal bg-orbit-900/95 p-5 shadow-2xl" aria-label={`${name} details`}><div className="flex items-start justify-between gap-4"><div><p className="font-display text-xs font-semibold uppercase tracking-[0.18em] text-cyan">Selected planet</p><h2 className="mt-2 font-display text-2xl text-ink">{name}</h2></div><button type="button" onClick={onClose} className="text-muted hover:text-ink" aria-label="Close planet details">×</button></div><dl className="mt-5 grid grid-cols-2 gap-4 text-sm"><div><dt className="text-muted">Diameter</dt><dd className="mt-1 text-ink">{facts.diameter}</dd></div><div><dt className="text-muted">Mass</dt><dd className="mt-1 text-ink">{facts.mass}</dd></div><div><dt className="text-muted">Moons</dt><dd className="mt-1 text-ink">{facts.moons}</dd></div><div><dt className="text-muted">Distance</dt><dd className="mt-1 text-ink">{distanceAu?.toFixed(3) ?? "—"} AU</dd></div><div className="col-span-2"><dt className="text-muted">Orbital period</dt><dd className="mt-1 text-ink">{orbitalPeriodDays?.toLocaleString() ?? "—"} days</dd></div></dl><p className="mt-5 border-t border-line pt-4 text-xs leading-5 text-dim">Mass, diameter, and moon counts are fixed reference values; the API supplies position and orbital period only.</p></aside>;
}
