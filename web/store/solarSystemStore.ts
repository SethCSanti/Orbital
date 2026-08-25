import { create } from "zustand";

interface SolarSystemState {
  selectedBody: string | null;
  cameraDistance: number;
  setSelectedBody: (selectedBody: string | null) => void;
  setCameraDistance: (cameraDistance: number) => void;
}

export const useSolarSystemStore = create<SolarSystemState>((set) => ({
  selectedBody: null,
  cameraDistance: 12,
  setSelectedBody: (selectedBody) => set({ selectedBody }),
  setCameraDistance: (cameraDistance) => set({ cameraDistance }),
}));
