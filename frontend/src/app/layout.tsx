import type { Metadata } from "next";
import Link from "next/link";
import { Geist, Geist_Mono } from "next/font/google";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Gamedevs Connect",
  description: "The collaboration network for indie game developers.",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html lang="en" className={`${geistSans.variable} ${geistMono.variable}`}>
      <body>
        <header style={{ padding: "0.75rem 1rem", borderBottom: "1px solid #ccc" }}>
          <Link href="/" style={{ fontWeight: 600, textDecoration: "none" }}>
            ← Gamedevs Connect
          </Link>
        </header>
        {children}
      </body>
    </html>
  );
}
