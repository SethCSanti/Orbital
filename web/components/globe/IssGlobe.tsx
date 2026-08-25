"use client";

import { Cartesian3, Color } from "cesium";
import { Entity, PointGraphics, PolylineGraphics, Viewer } from "resium";
import type { IssPositionUpdate } from "@/types/iss";

interface IssGlobeProps {
  position: IssPositionUpdate | null;
  trail: IssPositionUpdate[];
}

export default function IssGlobe({ position, trail }: IssGlobeProps) {
  const trailPositions = trail.map((point) =>
    Cartesian3.fromDegrees(point.longitude, point.latitude, 80_000),
  );
  const markerPosition = position
    ? Cartesian3.fromDegrees(position.longitude, position.latitude, 420_000)
    : undefined;

  return (
    <div className="h-[min(64vh,620px)] min-h-[420px] overflow-hidden border border-line bg-[#050a12]" aria-label="Live ISS globe">
      <Viewer
        full
        animation={false}
        baseLayerPicker={false}
        fullscreenButton={false}
        geocoder={false}
        homeButton={false}
        infoBox={false}
        navigationHelpButton={false}
        sceneModePicker={false}
        selectionIndicator={false}
        timeline={false}
      >
        {trailPositions.length > 1 && (
          <Entity name="ISS recent ground track">
            <PolylineGraphics positions={trailPositions} width={2} material={Color.CYAN.withAlpha(0.72)} />
          </Entity>
        )}
        {markerPosition && (
          <Entity name="International Space Station" position={markerPosition}>
            <PointGraphics color={Color.fromCssColorString("#a6d4ff")} pixelSize={12} outlineColor={Color.WHITE} outlineWidth={2} />
          </Entity>
        )}
      </Viewer>
    </div>
  );
}
