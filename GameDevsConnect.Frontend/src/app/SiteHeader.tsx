"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Search } from "lucide-react";
import { LeftNav } from "./LeftNav";
import { NotificationBell } from "./NotificationBell";

export function SiteHeader() {
  const pathname = usePathname();

  if (pathname === "/login") {
    return null;
  }

  return (
    <>
      <header className="border-b border-border bg-surface">
        <div className="mx-auto flex max-w-3xl items-center gap-4 px-4 py-3">
          <Link
            href="/"
            className="font-display text-[10px] text-accent-bright transition-colors hover:text-accent sm:text-xs"
          >
            GAMEDEVS CONNECT
          </Link>
          <Link
            href="/search"
            title="Suche"
            className="rounded-md border border-border p-1.5 text-text-muted transition-colors hover:border-accent hover:text-accent-bright"
          >
            <Search size={18} />
          </Link>
          <NotificationBell />
        </div>
      </header>
      <LeftNav />
    </>
  );
}
