"use client";

import { Line } from "@react-three/drei";
import { memo } from "react";

function OrbitPath({ radius, visible }: { radius: number; visible: boolean }) {
  if (!visible) return null;
  const points = Array.from({ length: 97 }, (_, index) => {
    const angle = (index / 96) * Math.PI * 2;
    return [Math.cos(angle) * radius, 0, Math.sin(angle) * radius] as [number, number, number];
  });
  return <Line points={points} color="#38506b" lineWidth={0.7} transparent opacity={0.8} />;
}

export default memo(OrbitPath);
