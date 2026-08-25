import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

vi.mock("cesium", () => ({
  Cartesian3: { fromDegrees: vi.fn(() => ({ x: 0, y: 0, z: 0 })) },
  Color: { CYAN: { withAlpha: vi.fn(() => "cyan") }, WHITE: "white", fromCssColorString: vi.fn(() => "blue") },
}));
vi.mock("resium", () => ({
  Viewer: ({ children }: { children?: unknown }) => children,
  Entity: ({ children }: { children?: unknown }) => children,
  PolylineGraphics: () => null,
  PointGraphics: () => null,
}));

import IssGlobe from "@/components/globe/IssGlobe";

describe("IssGlobe", () => {
  it("renders a live marker and recent track container", () => {
    render(<IssGlobe position={{ latitude: 12.5, longitude: -33.2, timestamp: "2026-08-25T12:00:00Z" }} trail={[{ latitude: 11, longitude: -34, timestamp: "2026-08-25T11:55:00Z" }, { latitude: 12.5, longitude: -33.2, timestamp: "2026-08-25T12:00:00Z" }]} />);
    expect(screen.getByLabelText("Live ISS globe")).toBeInTheDocument();
  });
});
