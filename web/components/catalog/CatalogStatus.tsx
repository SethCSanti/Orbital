"use client";

import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";

export default function CatalogStatus() {
  const query = useQuery({
    queryKey: ["catalog", "status"],
    queryFn: ({ signal }) => api.catalog.status(signal),
    staleTime: 30_000,
    refetchInterval: (current) => current.state.data?.some((item) => item.status === "pending" || item.status === "running" || item.status === "partial") ? 10_000 : false,
  });
  const state = query.data?.find((item) => item.catalog === "launch-history");
  if (!state || state.status === "complete") return null;
  const label = state.status === "running" ? "Archive syncing" : state.status === "partial" ? "Archive partially synced" : "Archive pending";
  return <p className="border border-line bg-panel px-3 py-2 text-xs text-muted" role="status">{label} · {state.recordsImported.toLocaleString()} records available{state.totalAvailable ? ` of ${state.totalAvailable.toLocaleString()}` : ""}</p>;
}
