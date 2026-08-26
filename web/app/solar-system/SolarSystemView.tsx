"use client";

import { useEffect, useMemo, useRef, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import SolarSystemCanvas from "@/components/solar-system/SolarSystemCanvas";
import PlanetInfoPanel from "@/components/solar-system/PlanetInfoPanel";
import { ErrorState, LoadingState } from "@/components/ui/AsyncState";
import { useUrlState } from "@/hooks/useUrlState";
import { api } from "@/lib/api";
import { useSolarSystemStore } from "@/store/solarSystemStore";

export default function SolarSystemView() {
  const [planetQuery, setPlanetQuery] = useUrlState("planet", "");
  const [scaleQuery, setScaleQuery] = useUrlState("scale", "readable");
  const [cameraQuery, setCameraQuery] = useUrlState("camera", "14");
  const { selectedPlanetId, setSelectedPlanetId, timeScale, setTimeScale, isPaused, setIsPaused, simulatedDate, setSimulatedDate, cameraDistance, setCameraDistance, showOrbits, setShowOrbits, showLabels, setShowLabels, scaleMode, setScaleMode } = useSolarSystemStore();
  const selectedPlanet = selectedPlanetId || planetQuery || null;
  const [simulationAnchor] = useState(() => simulatedDate);
  const simulatedDateRef = useRef(simulatedDate);
  const query = useQuery({ queryKey: ["solar-system", simulationAnchor.toISOString()], queryFn: ({ signal }) => api.solarSystem.positions(simulationAnchor.toISOString(), signal), staleTime: Infinity });

  useEffect(() => { if (planetQuery && planetQuery !== selectedPlanetId) setSelectedPlanetId(planetQuery); }, [planetQuery, selectedPlanetId, setSelectedPlanetId]);
  useEffect(() => { const parsed = Number(cameraQuery); if (Number.isFinite(parsed) && parsed > 0 && parsed !== cameraDistance) setCameraDistance(parsed); }, [cameraQuery, cameraDistance, setCameraDistance]);
  useEffect(() => { if (scaleQuery === "readable" || scaleQuery === "realistic") setScaleMode(scaleQuery); }, [scaleQuery, setScaleMode]);
  useEffect(() => { simulatedDateRef.current = simulatedDate; }, [simulatedDate]);
  useEffect(() => {
    if (isPaused) return;
    let frame = 0;
    let previousTime = performance.now();
    const advance = (currentTime: number) => {
      const elapsedSeconds = Math.min(currentTime - previousTime, 100) / 1_000;
      previousTime = currentTime;
      const nextDate = new Date(simulatedDateRef.current.getTime() + elapsedSeconds * timeScale * 86_400_000);
      simulatedDateRef.current = nextDate;
      setSimulatedDate(nextDate);
      frame = window.requestAnimationFrame(advance);
    };
    frame = window.requestAnimationFrame(advance);
    return () => window.cancelAnimationFrame(frame);
  }, [isPaused, setSimulatedDate, timeScale]);

  const selectedPosition = useMemo(() => query.data?.find((position) => position.name === selectedPlanet), [query.data, selectedPlanet]);
  const elapsedDays = (simulatedDate.getTime() - simulationAnchor.getTime()) / 86_400_000;
  function selectPlanet(name: string) { const value = name || null; setSelectedPlanetId(value); setPlanetQuery(value ?? ""); }
  function changeCamera(value: number) { setCameraDistance(value); setCameraQuery(String(value)); }
  function changeScale(value: "readable" | "realistic") { setScaleMode(value); setScaleQuery(value); }

  return <div className="mx-auto max-w-[1500px] space-y-8 px-5 py-8 lg:px-10">
    <header className="max-w-3xl"><p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-cyan">Ephemeris / heliocentric view</p><h1 className="mt-3 font-display text-3xl font-semibold tracking-tight text-ink md:text-5xl">Solar system</h1><p className="mt-4 text-base leading-7 text-muted">Explore simplified planetary positions calculated by the backend at a selectable simulation time. Distances are in astronomical units.</p></header>
    <div className="grid gap-3 border-y border-line py-4 sm:grid-cols-2 lg:grid-cols-5"><button type="button" onClick={() => setIsPaused(!isPaused)} className="border border-line bg-panel px-4 py-3 text-left text-sm text-ink hover:border-signal">{isPaused ? "▶ Play simulation" : "Ⅱ Pause simulation"}<span className="mt-1 block text-xs text-muted">{simulatedDate.toUTCString()}</span></button><label className="border border-line bg-panel px-4 py-2 text-xs text-muted">Speed<input type="range" min="1" max="365" step="1" value={timeScale} onChange={(event) => setTimeScale(Number(event.target.value))} className="mt-2 block w-full accent-signal" /><span className="text-ink">{timeScale} days / second</span></label><label className="border border-line bg-panel px-4 py-2 text-xs text-muted">Camera distance<input type="range" min="6" max="35" step="1" value={cameraDistance} onChange={(event) => changeCamera(Number(event.target.value))} className="mt-2 block w-full accent-signal" /><span className="text-ink">{cameraDistance}</span></label><label className="border border-line bg-panel px-2 py-2 text-xs text-muted">Distance scale<select value={scaleMode} onChange={(event) => changeScale(event.target.value as "readable" | "realistic")} className="mt-2 block w-full border border-line bg-orbit-900 px-2 py-2 text-sm text-ink"><option value="readable">Readable / compressed</option><option value="realistic">Relative AU</option></select></label><div className="flex items-center gap-4 border border-line bg-panel px-4 py-3 text-sm"><label className="flex items-center gap-2 text-ink"><input type="checkbox" checked={showOrbits} onChange={(event) => setShowOrbits(event.target.checked)} className="accent-signal" /> Orbits</label><label className="flex items-center gap-2 text-ink"><input type="checkbox" checked={showLabels} onChange={(event) => setShowLabels(event.target.checked)} className="accent-signal" /> Labels</label></div></div>
    {query.isLoading && <LoadingState label="Calculating planetary positions" />}{query.error && <ErrorState message={query.error instanceof Error ? query.error.message : "Solar system data unavailable."} onRetry={() => void query.refetch()} />}{query.data && <div className="relative"><SolarSystemCanvas positions={query.data} elapsedDays={elapsedDays} scaleMode={scaleMode} showOrbits={showOrbits} showLabels={showLabels} selectedPlanet={selectedPlanet} cameraDistance={cameraDistance} onSelect={selectPlanet} /><PlanetInfoPanel name={selectedPlanet} distanceAu={selectedPosition ? Math.sqrt(selectedPosition.x ** 2 + selectedPosition.y ** 2 + selectedPosition.z ** 2) : undefined} orbitalPeriodDays={selectedPosition?.orbitalPeriodDays} onClose={() => selectPlanet("")} /></div>}
  </div>;
}
