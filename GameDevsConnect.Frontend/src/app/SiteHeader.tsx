"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { LeftNav } from "./LeftNav";

export function SiteHeader() {
  const pathname = usePathname();

  if (pathname === "/login") {
    return null;
  }

  return (
    <>
      <header className="border-b border-border bg-surface">
        <div className="mx-auto flex max-w-3xl items-center px-4 py-3">
          <Link
            href="/"
            className="font-display text-[10px] text-accent-bright transition-colors hover:text-accent sm:text-xs"
          >
            GAMEDEVS CONNECT
          </Link>
        </div>
      </header>
      <LeftNav />
    </>
  );
}
