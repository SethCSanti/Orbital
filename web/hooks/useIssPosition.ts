"use client";

import { useCallback, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { api } from "@/lib/api";
import { useSignalR } from "@/hooks/useSignalR";
import type { IssPositionUpdate } from "@/types/iss";

export function useIssPosition() {
  const initialQuery = useQuery({
    queryKey: ["iss", "position"],
    queryFn: api.iss.position,
    staleTime: 5_000,
  });
  const [livePosition, setLivePosition] = useState<IssPositionUpdate | null>(null);
  const handlePosition = useCallback((position: IssPositionUpdate) => setLivePosition(position), []);
  const signal = useSignalR<IssPositionUpdate>("/hubs/iss", "ReceiveIssPosition", handlePosition);

  return {
    ...initialQuery,
    data: livePosition ?? initialQuery.data ?? null,
    connectionState: signal.connectionState,
    signalError: signal.error,
  };
}
