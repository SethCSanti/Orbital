import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("@react-three/fiber", () => ({ Canvas: ({ children }: { children?: unknown }) => children }));
vi.mock("@react-three/drei", () => ({
  Html: ({ children }: { children?: unknown }) => children,
  Line: () => null,
  OrbitControls: () => null,
  Stars: () => null,
}));

import SolarSystemCanvas, { scalePosition } from "@/components/solar-system/SolarSystemCanvas";

describe("SolarSystemCanvas", () => {
  it("renders planet labels from backend position data", () => {
    render(<SolarSystemCanvas positions={[{ name: "Earth", x: 1, y: 0, z: 0, orbitalPeriodDays: 365.256 }]} scaleMode="readable" showOrbits showLabels selectedPlanet="Earth" cameraDistance={14} onSelect={vi.fn()} />);
    expect(screen.getByLabelText("Interactive 3D solar system")).toBeInTheDocument();
    expect(screen.getByText("Earth")).toBeInTheDocument();
  });

  it("compresses readable distances while preserving direction", () => {
    const readable = scalePosition({ name: "Neptune", x: 30, y: 0, z: 0, orbitalPeriodDays: 60_182 }, "readable");
    const realistic = scalePosition({ name: "Neptune", x: 30, y: 0, z: 0, orbitalPeriodDays: 60_182 }, "realistic");
    expect(readable[0]).toBeLessThan(realistic[0]);
    expect(readable[0]).toBeGreaterThan(0);
  });
});
