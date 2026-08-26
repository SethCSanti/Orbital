"use client";

export default function Pagination({ page, pageSize, total, onChange }: { page: number; pageSize: number; total: number; onChange: (page: number) => void }) {
  const pages = Math.max(1, Math.ceil(total / pageSize));
  if (pages <= 1) return null;
  return <nav className="flex items-center justify-between border-t border-line pt-4 text-sm" aria-label="Catalog pages">
    <span className="text-muted">Page {page} of {pages} · {total.toLocaleString()} records</span>
    <div className="flex gap-2"><button type="button" disabled={page <= 1} onClick={() => onChange(page - 1)} className="border border-line px-3 py-2 text-xs uppercase tracking-[0.12em] text-muted disabled:opacity-40">Previous</button><button type="button" disabled={page >= pages} onClick={() => onChange(page + 1)} className="border border-line px-3 py-2 text-xs uppercase tracking-[0.12em] text-muted disabled:opacity-40">Next</button></div>
  </nav>;
}
