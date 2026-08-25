import { create } from "zustand";

interface FiltersState {
  missionType: string;
  orbitAbbrev: string;
  setMissionType: (missionType: string) => void;
  setOrbitAbbrev: (orbitAbbrev: string) => void;
}

export const useFiltersStore = create<FiltersState>((set) => ({
  missionType: "",
  orbitAbbrev: "",
  setMissionType: (missionType) => set({ missionType }),
  setOrbitAbbrev: (orbitAbbrev) => set({ orbitAbbrev }),
}));
