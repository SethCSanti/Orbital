"use client";

import { useEffect, useState } from "react";

export interface CountdownValue {
  days: number;
  hours: number;
  minutes: number;
  seconds: number;
  isPast: boolean;
}

const EMPTY_COUNTDOWN: CountdownValue = { days: 0, hours: 0, minutes: 0, seconds: 0, isPast: false };

function calculateCountdown(target: string | null): CountdownValue {
  if (!target) return EMPTY_COUNTDOWN;
  const difference = new Date(target).getTime() - Date.now();
  if (!Number.isFinite(difference)) return EMPTY_COUNTDOWN;
  const totalSeconds = Math.max(0, Math.floor(difference / 1000));
  return {
    days: Math.floor(totalSeconds / 86_400),
    hours: Math.floor((totalSeconds % 86_400) / 3_600),
    minutes: Math.floor((totalSeconds % 3_600) / 60),
    seconds: totalSeconds % 60,
    isPast: difference <= 0,
  };
}

export function useLaunchCountdown(target: string | null): CountdownValue {
  const [countdown, setCountdown] = useState(() => calculateCountdown(target));

  useEffect(() => {
    const update = () => setCountdown(calculateCountdown(target));
    update();
    const timer = window.setInterval(update, 1_000);
    return () => window.clearInterval(timer);
  }, [target]);

  return countdown;
}
