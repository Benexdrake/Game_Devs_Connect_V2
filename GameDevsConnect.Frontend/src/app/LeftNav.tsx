"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useEffect, useState } from "react";
import clsx from "clsx";
import { Bell, Compass, Home, Swords, User } from "lucide-react";
import type { CurrentUser } from "@/lib/types";

const BASE_ITEMS = [
  { href: "/", icon: Home, label: "Home" },
  { href: "/discover", icon: Compass, label: "Discover" },
  { href: "/quests", icon: Swords, label: "Quests" },
  { href: "/notifications", icon: Bell, label: "Notifications" },
];

export function LeftNav() {
  const pathname = usePathname();
  const [me, setMe] = useState<CurrentUser | null>(null);

  useEffect(() => {
    let cancelled = false;
    fetch("/api/auth/me", { credentials: "include" })
      .then((res) => (res.ok ? res.json() : null))
      .then((json) => {
        if (!cancelled) setMe(json);
      })
      .catch(() => {
        if (!cancelled) setMe(null);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  const items = me
    ? [...BASE_ITEMS, { href: `/users/${me.username}`, icon: User, label: "Profil" }]
    : BASE_ITEMS;

  return (
    <nav
      className={clsx(
        "fixed inset-x-0 bottom-0 z-20 flex flex-row justify-around gap-2 border-t-2 border-border-strong bg-surface p-2",
        // left-edge tracks the centered max-w-3xl content column (half of
        // 768px, plus this nav's own ~72px width and a 24px gap) instead of
        // hugging the viewport edge, so it sits right next to content like
        // on X regardless of how wide the window is. Falls back to 1rem
        // from the edge once the viewport is too narrow for that gap.
        "lg:inset-x-auto lg:top-1/2 lg:left-[max(1rem,calc(50vw-480px))] lg:bottom-auto lg:flex-col lg:justify-start lg:gap-3 lg:-translate-y-1/2 lg:rounded-lg lg:border-2 lg:p-3",
      )}
    >
      {items.map((item) => {
        const active = pathname === item.href;
        const Icon = item.icon;
        return (
          <Link
            key={item.href}
            href={item.href}
            title={item.label}
            className={clsx(
              "flex h-12 w-12 items-center justify-center rounded-md border-2 transition-colors",
              active
                ? "border-accent-bright bg-accent/20 text-accent-bright"
                : "border-border text-text-muted hover:border-accent hover:text-accent-bright",
            )}
          >
            <Icon size={22} />
          </Link>
        );
      })}
    </nav>
  );
}
