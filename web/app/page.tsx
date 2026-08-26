import OrbitIcon from "@/components/ui/OrbitIcon";

export default function HomePage() {
  return (
    <div className="mx-auto max-w-6xl">
      <section className="home-hero relative overflow-hidden border border-signal/60 bg-orbit-900 px-6 py-12 sm:px-10 sm:py-16">
        <div className="relative z-10 max-w-2xl">
          <p className="font-display text-xs font-semibold uppercase tracking-[0.22em] text-cyan">Orbital / signal desk</p>
          <h1 className="mt-4 max-w-3xl font-display text-4xl font-semibold tracking-tight text-ink sm:text-6xl">Read the sky in motion.</h1>
          <p className="mt-5 max-w-xl text-lg leading-8 text-muted">A calm workspace for making sense of movement, history, and distance.</p>
          <div className="mt-9 flex items-center gap-3 border-t border-line pt-4 text-xs uppercase tracking-[0.16em]">
            <span className="h-2 w-2 rounded-full bg-success" aria-hidden="true" />
            <span className="text-success">Signal ready</span>
            <span className="text-dim">Use the sidebar to begin</span>
          </div>
        </div>
        <div className="home-orbit-mark"><OrbitIcon className="h-full w-full" /></div>
      </section>
      <section className="grid border-x border-b border-line bg-panel sm:grid-cols-[1.2fr_0.8fr]" aria-label="Orbital note">
        <div className="border-b border-line p-6 sm:border-b-0 sm:border-r sm:p-8">
          <p className="font-display text-xs font-semibold uppercase tracking-[0.18em] text-signal">Field note / 01</p>
          <h2 className="mt-6 max-w-md font-display text-2xl font-semibold tracking-tight text-ink">Space data, all in one place.</h2>
          <p className="mt-3 max-w-lg text-sm leading-7 text-muted">Browse the latest information about launches, missions, and objects in space.</p>
        </div>
        <div className="flex items-center p-6 sm:p-8">
          <div className="w-full border-l-2 border-cyan pl-5">
            <p className="font-display text-xs font-semibold uppercase tracking-[0.18em] text-cyan">Operating principle</p>
            <p className="mt-3 text-sm leading-7 text-muted">Select a section from the sidebar to get started.</p>
          </div>
        </div>
      </section>
    </div>
  );
}
