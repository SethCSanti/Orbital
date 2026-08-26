"use client";

import dynamic from "next/dynamic";

const SolarSystemView = dynamic(() => import("./SolarSystemView"), {
  ssr: false,
  loading: () => <div className="mx-auto max-w-[1500px] p-10 text-sm text-muted">Loading solar system…</div>,
});

export default function SolarSystemLoader() {
  return <SolarSystemView />;
}
