import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import MissionTimeline from "@/components/timeline/MissionTimeline";

describe("MissionTimeline", () => {
  it("renders mission and launch events and opens the first detail", () => {
    render(<MissionTimeline missions={[{ name: "Lunar Pathfinder", description: "A lunar survey.", type: "Robotic", launchDesignator: "2024-03-01", orbitName: "Lunar orbit", orbitAbbrev: "LO" }]} launches={[{ name: "Test Flight", statusName: "Success", net: "2023-08-01T00:00:00Z", windowStart: "2023-08-01T00:00:00Z", windowEnd: "2023-08-01T01:00:00Z", probability: null, holdReason: null, failReason: null, hashtag: null, rocketName: "Atlas", missionName: "Demo", orbitAbbrev: "LEO", crewNames: [] }]} typeFilter="" orbitFilter="" />);
    expect(screen.getByRole("list", { name: "Timeline events" })).toBeInTheDocument();
    expect(screen.getAllByText("Test Flight").length).toBeGreaterThan(0);
    fireEvent.click(screen.getByRole("button", { name: /Lunar Pathfinder/ }));
    expect(screen.getByText("A lunar survey.")).toBeInTheDocument();
  });
});
