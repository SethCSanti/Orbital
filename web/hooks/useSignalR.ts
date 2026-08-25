"use client";

import { useEffect, useRef, useState } from "react";
import { HubConnectionState } from "@microsoft/signalr";
import { ensureStarted, getHubConnection, subscribeToConnectionState } from "@/lib/signalr";

export function useSignalR<T>(hubPath: string, eventName: string, onMessage: (payload: T) => void) {
  const callbackRef = useRef(onMessage);
  const [connectionState, setConnectionState] = useState(HubConnectionState.Disconnected);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    callbackRef.current = onMessage;
  }, [onMessage]);

  useEffect(() => {
    const connection = getHubConnection(hubPath);
    const handleMessage = (payload: T) => callbackRef.current(payload);
    const unsubscribeState = subscribeToConnectionState(connection, setConnectionState);

    connection.on(eventName, handleMessage);
    ensureStarted(connection, hubPath).catch((reason: unknown) => {
      setError(reason instanceof Error ? reason : new Error("SignalR connection failed."));
    });

    return () => {
      connection.off(eventName, handleMessage);
      unsubscribeState();
    };
  }, [eventName, hubPath]);

  return { connectionState, error };
}
