import { Suspense } from "react";
import MissionsView from "./MissionsView";

export default function MissionsPage() {
  return <Suspense fallback={<div className="border border-line bg-panel p-8 text-sm text-muted">Loading mission atlas…</div>}><MissionsView /></Suspense>;
}
