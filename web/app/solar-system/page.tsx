import { Suspense } from "react";
import SolarSystemView from "./SolarSystemView";

export default function SolarSystemPage() {
  return <Suspense fallback={<div className="mx-auto max-w-[1500px] p-10 text-sm text-muted">Loading solar system…</div>}><SolarSystemView /></Suspense>;
}
