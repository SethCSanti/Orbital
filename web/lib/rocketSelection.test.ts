import { describe, expect, it } from "vitest";
import { toggleRocketId } from "@/lib/rocketSelection";

describe("rocket selection", () => {
  it("keeps same-named rocket records independent by ID", () => {
    expect(toggleRocketId([11], 22)).toEqual([11, 22]);
    expect(toggleRocketId([11, 22], 11)).toEqual([22]);
  });
});
