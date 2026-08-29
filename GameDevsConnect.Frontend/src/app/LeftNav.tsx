"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useEffect, useState } from "react";
import clsx from "clsx";
import { Bell, Compass, Home, Search, Swords, User } from "lucide-react";
import type { CurrentUser, NotificationsResult } from "@/lib/types";

const POLL_INTERVAL_MS = 30000;

export function LeftNav() {
  const pathname = usePathname();
  const [me, setMe] = useState<CurrentUser | null>(null);
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    let cancelled = false;

    async function poll() {
      try {
        const meRes = await fetch("/api/auth/me", { credentials: "include" });
        if (!meRes.ok) {
          if (!cancelled) setMe(null);
          return;
        }
        const meJson: CurrentUser = await meRes.json();
        if (cancelled) return;
        setMe(meJson);

        const notifRes = await fetch("/api/notifications?pageSize=1", { credentials: "include" });
        if (notifRes.ok && !cancelled) {
          const data: NotificationsResult = await notifRes.json();
          setUnreadCount(data.unreadCount);
        }
      } catch {
        // Ignore - next poll tick will retry.
      }
    }

    poll();
    const interval = setInterval(poll, POLL_INTERVAL_MS);
    return () => {
      cancelled = true;
      clearInterval(interval);
    };
  }, []);

  const items = [
    { href: "/", icon: Home, label: "Home", badge: 0 },
    { href: "/notifications", icon: Bell, label: "Notifications", badge: unreadCount },
    { href: "/discover", icon: Compass, label: "Discover", badge: 0 },
    { href: "/quests", icon: Swords, label: "Quests", badge: 0 },
    { href: "/search", icon: Search, label: "Suche", badge: 0 },
    ...(me ? [{ href: `/users/${me.username}`, icon: User, label: "Profil", badge: 0 }] : []),
  ];

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
              "relative flex h-12 w-12 items-center justify-center rounded-md border-2 transition-colors",
              active
                ? "border-accent-bright bg-accent/20 text-accent-bright"
                : "border-border text-text-muted hover:border-accent hover:text-accent-bright",
            )}
          >
            <Icon size={22} />
            {item.badge > 0 && (
              <span className="absolute -top-1.5 -right-1.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-danger px-1 text-[10px] font-medium text-text">
                {item.badge > 9 ? "9+" : item.badge}
              </span>
            )}
          </Link>
        );
      })}
    </nav>
  );
}
