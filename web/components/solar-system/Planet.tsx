"use client";

import { Html } from "@react-three/drei";
import type { ThreeEvent } from "@react-three/fiber";

interface PlanetProps {
  name: string;
  position: [number, number, number];
  radius: number;
  color: string;
  showLabel: boolean;
  selected: boolean;
  onSelect: (name: string) => void;
}

export default function Planet({ name, position, radius, color, showLabel, selected, onSelect }: PlanetProps) {
  function handleClick(event: ThreeEvent<MouseEvent>) {
    event.stopPropagation();
    onSelect(name);
  }

  return <mesh position={position} onClick={handleClick}>
    <sphereGeometry args={[radius, 24, 16]} />
    <meshStandardMaterial color={color} emissive={selected ? color : "#000000"} emissiveIntensity={selected ? 0.28 : 0} roughness={0.85} />
    {showLabel && <Html center distanceFactor={12} position={[0, radius + 0.28, 0]}><span className={`whitespace-nowrap border px-2 py-1 font-display text-[10px] uppercase tracking-[0.12em] ${selected ? "border-signal bg-orbit-900 text-signal-strong" : "border-line/70 bg-orbit-950/80 text-muted"}`}>{name}</span></Html>}
  </mesh>;
}
