import { create } from "zustand";

interface SolarSystemState {
  selectedPlanetId: string | null;
  timeScale: number;
  isPaused: boolean;
  simulatedDate: Date;
  cameraDistance: number;
  showOrbits: boolean;
  showLabels: boolean;
  scaleMode: "readable" | "realistic";
  setSelectedPlanetId: (selectedPlanetId: string | null) => void;
  setTimeScale: (timeScale: number) => void;
  setIsPaused: (isPaused: boolean) => void;
  setSimulatedDate: (simulatedDate: Date) => void;
  setCameraDistance: (cameraDistance: number) => void;
  setShowOrbits: (showOrbits: boolean) => void;
  setShowLabels: (showLabels: boolean) => void;
  setScaleMode: (scaleMode: "readable" | "realistic") => void;
}

export const useSolarSystemStore = create<SolarSystemState>((set) => ({
  selectedPlanetId: null,
  timeScale: 1,
  isPaused: true,
  simulatedDate: new Date(),
  cameraDistance: 14,
  showOrbits: true,
  showLabels: true,
  scaleMode: "readable",
  setSelectedPlanetId: (selectedPlanetId) => set({ selectedPlanetId }),
  setTimeScale: (timeScale) => set({ timeScale }),
  setIsPaused: (isPaused) => set({ isPaused }),
  setSimulatedDate: (simulatedDate) => set({ simulatedDate }),
  setCameraDistance: (cameraDistance) => set({ cameraDistance }),
  setShowOrbits: (showOrbits) => set({ showOrbits }),
  setShowLabels: (showLabels) => set({ showLabels }),
  setScaleMode: (scaleMode) => set({ scaleMode }),
}));
