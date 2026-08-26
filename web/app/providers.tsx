"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { useState, type ReactNode } from "react";

export default function Providers({ children }: { children: ReactNode }) {
  const [queryClient] = useState(
    () => new QueryClient({
      defaultOptions: {
        queries: {
          retry: (failureCount, error) => failureCount < 1 && !(error instanceof Error && "status" in error && Number((error as { status?: number }).status) < 500),
          retryDelay: (attempt) => Math.min(1_000 * 2 ** attempt, 5_000),
          staleTime: 5 * 60_000,
          gcTime: 30 * 60_000,
          refetchOnWindowFocus: false,
          refetchOnReconnect: true,
        },
      },
    }),
  );

  return <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>;
}
