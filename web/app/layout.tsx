import type { ReactNode } from "react";
import type { Metadata, Viewport } from "next";
import Script from "next/script";
import "./globals.css";
import Navbar from "@/components/layout/Navbar";
import Sidebar from "@/components/layout/Sidebar";
import Providers from "./providers";

export const metadata: Metadata = {
  title: { default: "Orbital — Space Exploration Dashboard", template: "%s | Orbital" },
  description: "A live dashboard for launches, missions, spacecraft, and near-Earth objects.",
  manifest: "/manifest.json",
};

export const viewport: Viewport = {
  themeColor: "#070b12",
  width: "device-width",
  initialScale: 1,
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en">
      <body>
        <Script id="cesium-base-url" strategy="beforeInteractive">
          {`window.CESIUM_BASE_URL = "/cesium";`}
        </Script>
        <Providers>
          <div className="min-h-screen bg-orbit-950">
            <Navbar />
            <div className="mx-auto flex max-w-[1680px]">
              <Sidebar />
              <main id="main-content" className="min-w-0 flex-1 px-4 pb-16 pt-6 sm:px-6 lg:px-10 lg:pt-10">
                {children}
              </main>
            </div>
          </div>
        </Providers>
      </body>
    </html>
  );
}
