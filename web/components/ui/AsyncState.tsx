export function LoadingState({ label = "Loading feed" }: { label?: string }) {
  return <div className="border border-line bg-panel p-8 text-sm text-muted" role="status">{label}…</div>;
}

export function ErrorState({ message, onRetry }: { message: string; onRetry?: () => void }) {
  return (
    <div className="border border-danger/50 bg-danger-soft/40 p-6" role="alert">
      <p className="text-sm leading-6 text-ink">{message}</p>
      {onRetry && <button onClick={onRetry} className="mt-4 border border-danger px-3 py-2 text-xs font-semibold uppercase tracking-[0.14em] text-danger hover:bg-danger/10">Try again</button>}
    </div>
  );
}
