import type { ReactNode } from "react";
import CesiumInit from "@/components/globe/CesiumInit";

export default function IssTrackerLayout({ children }: { children: ReactNode }) {
  return (
    <>
      <CesiumInit />
      {children}
    </>
  );
}
