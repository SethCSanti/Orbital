import type { ApodEntry } from "@/types/apod";

function youtubeEmbedUrl(url: string) {
  try {
    const parsed = new URL(url);
    if (parsed.hostname === "youtu.be") return `https://www.youtube-nocookie.com/embed/${parsed.pathname.slice(1)}`;
    if (parsed.hostname.endsWith("youtube.com")) {
      const videoId = parsed.searchParams.get("v") ?? parsed.pathname.split("/").pop();
      return videoId ? `https://www.youtube-nocookie.com/embed/${videoId}` : url;
    }
  } catch {
    return url;
  }
  return url;
}

export default function ApodViewer({ entry }: { entry: ApodEntry }) {
  const isVideo = entry.mediaType.toLowerCase() === "video";

  return (
    <article className="overflow-hidden border border-line bg-panel">
      <div className="grid lg:grid-cols-[minmax(0,1.35fr)_minmax(300px,0.65fr)]">
        <div className="min-h-[300px] bg-orbit-900">
          {isVideo ? (
            <iframe
              title={entry.title}
              src={youtubeEmbedUrl(entry.url)}
              className="h-full min-h-[360px] w-full"
              allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share"
              allowFullScreen
            />
          ) : (
            <img
              src={entry.hdUrl ?? entry.url}
              alt={entry.title}
              className="h-full min-h-[360px] w-full object-cover"
            />
          )}
        </div>
        <div className="flex flex-col justify-between border-t border-line p-6 lg:border-l lg:border-t-0 lg:p-8">
          <div>
            <p className="font-display text-xs font-semibold uppercase tracking-[0.18em] text-signal">Astronomy picture of the day</p>
            <h2 className="mt-4 font-display text-2xl font-semibold leading-tight text-ink">{entry.title}</h2>
            <p className="mt-2 text-sm text-muted">
              <time dateTime={entry.date}>{entry.date}</time>
              {entry.copyright ? ` · ${entry.copyright}` : ""}
            </p>
            <p className="mt-6 text-[15px] leading-7 text-muted">{entry.explanation}</p>
          </div>
          <a href={entry.hdUrl ?? entry.url} target="_blank" rel="noreferrer" className="mt-8 text-sm font-semibold text-signal hover:text-signal-strong">
            Open original source ↗
          </a>
        </div>
      </div>
    </article>
  );
}
