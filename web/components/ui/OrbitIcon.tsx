export default function OrbitIcon({ className = "" }: { className?: string }) {
  return (
    <svg className={className} viewBox="0 0 100 100" fill="none" aria-hidden="true">
      <circle cx="50" cy="50" r="43" stroke="var(--color-signal)" strokeWidth="1.5" />
      <ellipse cx="50" cy="50" rx="43" ry="18" transform="rotate(-24 50 50)" stroke="var(--color-cyan)" strokeWidth="1.5" />
      <ellipse cx="50" cy="50" rx="43" ry="13" transform="rotate(42 50 50)" stroke="var(--color-success)" strokeWidth="1.5" />
      <circle cx="50" cy="50" r="15" fill="var(--color-orbit-950)" stroke="var(--color-cyan)" strokeWidth="1.5" />
      <circle cx="50" cy="50" r="4" fill="var(--color-cyan)" />
    </svg>
  );
}
