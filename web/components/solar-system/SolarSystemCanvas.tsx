"use client";

import { Canvas } from "@react-three/fiber";
import { OrbitControls, Stars } from "@react-three/drei";
import Planet from "@/components/solar-system/Planet";
import OrbitPath from "@/components/solar-system/OrbitPath";
import type { PlanetPosition } from "@/types/solarSystem";

const PLANET_STYLE: Record<string, { radius: number; color: string }> = {
  Mercury: { radius: 0.12, color: "#9b8d7a" }, Venus: { radius: 0.18, color: "#d0a86f" }, Earth: { radius: 0.2, color: "#5b98c4" }, Mars: { radius: 0.15, color: "#bf6f5b" }, Jupiter: { radius: 0.48, color: "#c89c76" }, Saturn: { radius: 0.4, color: "#d8bb83" }, Uranus: { radius: 0.28, color: "#7fc5c9" }, Neptune: { radius: 0.27, color: "#5c80ca" },
};

function visualDistance(value: number, mode: "readable" | "realistic") {
  return mode === "realistic" ? value * 0.65 : Math.sign(value) * Math.log10(1 + Math.abs(value) * 4) * 2.4;
}

export function scalePosition(position: PlanetPosition, mode: "readable" | "realistic"): [number, number, number] {
  return [visualDistance(position.x, mode), visualDistance(position.z, mode), visualDistance(position.y, mode)];
}

export default function SolarSystemCanvas({ positions, scaleMode, showOrbits, showLabels, selectedPlanet, cameraDistance, onSelect }: { positions: PlanetPosition[]; scaleMode: "readable" | "realistic"; showOrbits: boolean; showLabels: boolean; selectedPlanet: string | null; cameraDistance: number; onSelect: (name: string) => void }) {
  return <div className="relative h-[min(70vh,720px)] min-h-[520px] overflow-hidden border border-line bg-[#050a12]" aria-label="Interactive 3D solar system"><Canvas camera={{ position: [0, cameraDistance * 0.35, cameraDistance], fov: 45 }} dpr={[1, 1.5]}><color attach="background" args={["#050a12"]} /><ambientLight intensity={0.65} /><pointLight position={[0, 0, 0]} intensity={12} distance={80} color="#ffe1a3" /><Stars radius={90} depth={45} count={1800} factor={2.4} saturation={0} fade speed={0.2} /><mesh onClick={() => onSelect("")}><sphereGeometry args={[0.55, 32, 24]} /><meshBasicMaterial color="#ffc46b" /></mesh>{positions.map((position) => { const style = PLANET_STYLE[position.name] ?? { radius: 0.18, color: "#a6d4ff" }; const scaled = scalePosition(position, scaleMode); const orbitRadius = Math.sqrt(scaled[0] ** 2 + scaled[2] ** 2); return <group key={position.name}><OrbitPath radius={orbitRadius} visible={showOrbits} /><Planet name={position.name} position={scaled} radius={style.radius} color={style.color} showLabel={showLabels} selected={selectedPlanet === position.name} onSelect={onSelect} /></group>; })}<OrbitControls enablePan={false} minDistance={3} maxDistance={55} /></Canvas></div>;
}
