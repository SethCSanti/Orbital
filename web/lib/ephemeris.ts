export interface VisibilityWindow {
  riseAt: Date;
  overheadAt: Date;
  setAt: Date;
  minutesUntilRise: number;
  isVisibleNow: boolean;
}

const ORBITAL_PERIOD_MINUTES = 92.68;

function wrapDegrees(value: number): number {
  return ((value + 180) % 360 + 360) % 360 - 180;
}

/**
 * Estimates the next ISS pass from the current live position. Without TLE
 * history or observer elevation data, this is deliberately a rough orbital-
 * period approximation for UI guidance, not a pass prediction service.
 */
export function estimateIssVisibility(
  current: { latitude: number; longitude: number },
  observer: { latitude: number; longitude: number },
  now = new Date(),
): VisibilityWindow {
  const latitudeGap = Math.abs(current.latitude - observer.latitude);
  const longitudeGap = Math.abs(wrapDegrees(current.longitude - observer.longitude));
  const angularGap = Math.sqrt(latitudeGap ** 2 + longitudeGap ** 2);
  const passFraction = Math.min(1, angularGap / 180);
  const minutesUntilRise = Math.max(0, Math.round(passFraction * ORBITAL_PERIOD_MINUTES));
  const riseAt = new Date(now.getTime() + minutesUntilRise * 60_000);
  const overheadAt = new Date(riseAt.getTime() + 7 * 60_000);
  const setAt = new Date(riseAt.getTime() + 14 * 60_000);
  const isVisibleNow = angularGap < 22;

  return { riseAt, overheadAt, setAt, minutesUntilRise, isVisibleNow };
}
