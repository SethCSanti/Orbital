import {
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
  type HubConnection,
} from "@microsoft/signalr";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5110";
const connections = new Map<string, HubConnection>();
const startPromises = new Map<string, Promise<void>>();
const stateListeners = new Map<HubConnection, Set<(state: HubConnectionState) => void>>();

function getHubUrl(path: string) {
  const configuredBase = process.env.NEXT_PUBLIC_SIGNALR_HUB_URL;
  if (configuredBase) return `${configuredBase.replace(/\/$/, "")}/${path.replace(/^\//, "")}`;
  return `${API_BASE_URL.replace(/\/$/, "")}/${path.replace(/^\//, "")}`;
}

export function getHubConnection(path: string): HubConnection {
  const existing = connections.get(path);
  if (existing) return existing;

  const connection = new HubConnectionBuilder()
    .withUrl(getHubUrl(path))
    .withAutomaticReconnect()
    // Surface failures through the hook's in-app status instead of logging
    // repeated negotiation errors during local API restarts.
    .configureLogging(LogLevel.None)
    .build();
  connections.set(path, connection);
  return connection;
}

export function ensureStarted(connection: HubConnection, key: string): Promise<void> {
  if (connection.state === HubConnectionState.Connected) return Promise.resolve();

  const existingStart = startPromises.get(key);
  if (existingStart) return existingStart;

  const start = connection
    .start()
    .catch((error) => {
      startPromises.delete(key);
      throw error;
    })
    .finally(() => startPromises.delete(key));
  startPromises.set(key, start);
  return start;
}

export function subscribeToConnectionState(
  connection: HubConnection,
  onStateChange: (state: HubConnectionState) => void,
) {
  let listeners = stateListeners.get(connection);
  if (!listeners) {
    listeners = new Set();
    stateListeners.set(connection, listeners);
    connection.onreconnecting(() => notifyState(connection, HubConnectionState.Reconnecting));
    connection.onreconnected(() => notifyState(connection, HubConnectionState.Connected));
    connection.onclose(() => notifyState(connection, HubConnectionState.Disconnected));
  }

  listeners.add(onStateChange);
  onStateChange(connection.state);

  return () => {
    listeners?.delete(onStateChange);
    if (listeners?.size === 0) stateListeners.delete(connection);
  };
}

function notifyState(connection: HubConnection, state: HubConnectionState) {
  stateListeners.get(connection)?.forEach((listener) => listener(state));
}
