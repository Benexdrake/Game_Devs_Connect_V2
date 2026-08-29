"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect, useRef, useState } from "react";
import { Menu } from "lucide-react";
import { LeftNav } from "./LeftNav";

const PLACEHOLDER_ITEMS = ["Platzhalter 1", "Platzhalter 2", "Platzhalter 3"];

export function SiteHeader() {
  const pathname = usePathname();
  const router = useRouter();
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (!menuOpen) return;

    function handleClickOutside(e: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setMenuOpen(false);
      }
    }

    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [menuOpen]);

  async function handleLogout() {
    setMenuOpen(false);
    await fetch("/api/auth/logout", { method: "POST", credentials: "include" });
    router.push("/login");
    router.refresh();
  }

  if (pathname === "/login") {
    return null;
  }

  return (
    <>
      <header className="border-b border-border bg-surface">
        <div className="mx-auto flex max-w-[1200px] items-center px-4 py-3">
          <Link
            href="/"
            className="font-display text-[10px] text-accent-bright transition-colors hover:text-accent sm:text-xs"
          >
            GAMEDEVS CONNECT
          </Link>

          <div ref={menuRef} className="relative ml-auto">
            <button
              type="button"
              aria-label="Menü"
              onClick={() => setMenuOpen((open) => !open)}
              className="flex h-9 w-9 items-center justify-center rounded-md border-2 border-border text-text-muted transition-colors hover:border-accent hover:text-accent-bright"
            >
              <Menu size={18} />
            </button>

            {menuOpen && (
              <div className="absolute top-full right-0 z-30 mt-2 w-48 rounded-md border-2 border-border bg-surface py-1 shadow-lg">
                {PLACEHOLDER_ITEMS.map((label) => (
                  <span key={label} className="block cursor-default px-3 py-2 text-sm text-text-muted">
                    {label}
                  </span>
                ))}
                <div className="my-1 border-t border-border" />
                <button
                  type="button"
                  onClick={handleLogout}
                  className="block w-full px-3 py-2 text-left text-sm text-danger hover:bg-canvas"
                >
                  Logout
                </button>
              </div>
            )}
          </div>
        </div>
      </header>
      <LeftNav />
    </>
  );
}
