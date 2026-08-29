import type { Metadata } from "next";
import Link from "next/link";
import { Geist, Geist_Mono, Press_Start_2P } from "next/font/google";
import { LoginLink } from "./LoginLink";
import { NotificationBell } from "./NotificationBell";
import { SearchBar } from "./SearchBar";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

const pressStart2P = Press_Start_2P({
  variable: "--font-press-start",
  subsets: ["latin"],
  weight: "400",
});

export const metadata: Metadata = {
  title: "Gamedevs Connect",
  description: "The collaboration network for indie game developers.",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} ${pressStart2P.variable}`}
    >
      <body>
        <header className="flex items-center gap-4 border-b border-border bg-surface px-4 py-3">
          <Link
            href="/"
            className="font-display text-[10px] text-accent-bright transition-colors hover:text-accent sm:text-xs"
          >
            GAMEDEVS CONNECT
          </Link>
          <Link href="/discover" className="text-sm text-text-muted transition-colors hover:text-text">
            Discover
          </Link>
          <SearchBar />
          <span className="ml-auto">
            <LoginLink />
          </span>
          <NotificationBell />
        </header>
        {children}
      </body>
    </html>
  );
}
