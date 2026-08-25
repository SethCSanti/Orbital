"use client";

import { useEffect, useRef } from "react";
import * as d3 from "d3";
import type { Asteroid } from "@/types/asteroid";

interface PlotPoint {
  asteroid: Asteroid;
  diameter: number;
}

export default function AsteroidScatterPlot({ asteroids, selectedId, onSelect }: { asteroids: Asteroid[]; selectedId?: string; onSelect: (asteroid: Asteroid) => void }) {
  const svgRef = useRef<SVGSVGElement>(null);

  useEffect(() => {
    const svg = d3.select(svgRef.current);
    svg.selectAll("*").remove();

    const points: PlotPoint[] = asteroids.map((asteroid) => ({ asteroid, diameter: (asteroid.estimatedDiameterMinKm + asteroid.estimatedDiameterMaxKm) / 2 })).filter((point) => Number.isFinite(point.asteroid.missDistanceKm) && Number.isFinite(point.diameter));
    if (points.length === 0) return;

    const width = 760;
    const height = 390;
    const margin = { top: 24, right: 24, bottom: 58, left: 70 };
    const minX = d3.min(points, (point) => point.asteroid.missDistanceKm) ?? 0;
    const maxX = d3.max(points, (point) => point.asteroid.missDistanceKm) ?? 1;
    const minY = d3.min(points, (point) => point.diameter) ?? 0;
    const maxY = d3.max(points, (point) => point.diameter) ?? 1;
    const x = d3.scaleLinear().domain([minX, maxX === minX ? minX + 1 : maxX]).nice().range([margin.left, width - margin.right]);
    const y = d3.scaleLinear().domain([minY, maxY === minY ? minY + 1 : maxY]).nice().range([height - margin.bottom, margin.top]);

    const plot = svg.attr("viewBox", `0 0 ${width} ${height}`).append("g");
    plot.append("g").attr("transform", `translate(0,${height - margin.bottom})`).call(d3.axisBottom(x).ticks(6).tickFormat((value) => d3.format(".2s")(Number(value))));
    plot.append("g").attr("transform", `translate(${margin.left},0)`).call(d3.axisLeft(y).ticks(5).tickFormat((value) => `${d3.format(".2s")(Number(value))} km`));
    plot.append("text").attr("x", width / 2).attr("y", height - 12).attr("text-anchor", "middle").attr("fill", "var(--color-muted)").attr("font-size", 11).text("Miss distance from Earth (km)");
    plot.append("text").attr("transform", "rotate(-90)").attr("x", -(height / 2)).attr("y", 16).attr("text-anchor", "middle").attr("fill", "var(--color-muted)").attr("font-size", 11).text("Mean estimated diameter");

    plot.selectAll("circle")
      .data(points)
      .join("circle")
      .attr("cx", (point) => x(point.asteroid.missDistanceKm))
      .attr("cy", (point) => y(point.diameter))
      .attr("r", (point) => point.asteroid.neoReferenceId === selectedId ? 7 : 5)
      .attr("fill", (point) => point.asteroid.isPotentiallyHazardous ? "var(--color-danger)" : "var(--color-signal)")
      .attr("stroke", "var(--color-ink)")
      .attr("stroke-width", (point) => point.asteroid.neoReferenceId === selectedId ? 2 : 0.5)
      .attr("opacity", 0.9)
      .style("cursor", "pointer")
      .on("click", (_, point) => onSelect(point.asteroid))
      .append("title")
      .text((point) => point.asteroid.name);
  }, [asteroids, onSelect, selectedId]);

  return (
    <div>
      <div className="overflow-x-auto">
        <svg ref={svgRef} className="min-w-[680px] w-full" aria-label="Asteroid miss distance and estimated diameter scatter plot" role="img" />
      </div>
      <div className="mt-3 flex flex-wrap gap-5 text-xs text-muted" aria-label="Chart legend">
        <span className="flex items-center gap-2"><span className="h-2.5 w-2.5 rounded-full bg-signal" /> Not classified hazardous</span>
        <span className="flex items-center gap-2"><span className="h-2.5 w-2.5 rounded-full bg-danger" /> Potentially hazardous</span>
      </div>
    </div>
  );
}
