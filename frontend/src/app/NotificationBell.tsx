"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import clsx from "clsx";
import type { CurrentUser, NotificationsResult } from "@/lib/types";

const POLL_INTERVAL_MS = 30000;

export function NotificationBell() {
  const [me, setMe] = useState<CurrentUser | null>(null);
  const [data, setData] = useState<NotificationsResult | null>(null);
  const [open, setOpen] = useState(false);

  useEffect(() => {
    let cancelled = false;

    async function poll() {
      try {
        const meRes = await fetch("/api/auth/me", { credentials: "include" });
        if (!meRes.ok) {
          if (!cancelled) {
            setMe(null);
            setData(null);
          }
          return;
        }
        const meJson: CurrentUser = await meRes.json();
        if (cancelled) return;
        setMe(meJson);

        const notifRes = await fetch("/api/notifications?pageSize=10", { credentials: "include" });
        if (notifRes.ok && !cancelled) {
          setData(await notifRes.json());
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

  async function markRead(id: string) {
    await fetch(`/api/notifications/${id}/read`, { method: "PATCH", credentials: "include" });
    setData((prev) =>
      prev
        ? {
            unreadCount: Math.max(0, prev.unreadCount - 1),
            items: prev.items.map((n) => (n.id === id ? { ...n, isRead: true } : n)),
          }
        : prev,
    );
  }

  async function markAllRead() {
    await fetch("/api/notifications/read-all", { method: "PATCH", credentials: "include" });
    setData((prev) => (prev ? { unreadCount: 0, items: prev.items.map((n) => ({ ...n, isRead: true })) } : prev));
  }

  if (!me) {
    return null;
  }

  return (
    <div className="relative ml-auto">
      <button
        type="button"
        onClick={() => setOpen((o) => !o)}
        className="rounded-md border border-border px-2 py-1 text-sm text-text transition-colors hover:border-accent hover:text-accent-bright"
      >
        🔔{data && data.unreadCount > 0 ? ` (${data.unreadCount})` : ""}
      </button>

      {open && (
        <div className="absolute right-0 top-full z-10 mt-1 max-h-96 w-80 overflow-y-auto rounded-lg border-2 border-border-strong bg-surface p-2 text-text shadow-xl">
          <div className="mb-2 flex items-center justify-between">
            <strong className="font-display text-[10px] text-accent-bright">NOTIFICATIONS</strong>
            <button type="button" onClick={markAllRead} className="text-xs text-accent hover:text-accent-bright">
              Alle gelesen
            </button>
          </div>

          {!data || data.items.length === 0 ? (
            <p className="text-sm text-text-muted">Keine Benachrichtigungen.</p>
          ) : (
            <ul className="list-none space-y-1 p-0">
              {data.items.map((n) => (
                <li
                  key={n.id}
                  onClick={() => !n.isRead && markRead(n.id)}
                  className={clsx(
                    "rounded-md border border-transparent p-2",
                    !n.isRead && "cursor-pointer border-accent/40 bg-accent/10",
                  )}
                >
                  <p className="m-0 text-sm">{n.message}</p>
                  <p className="m-0 text-xs text-text-muted">{new Date(n.createdAt).toLocaleString()}</p>
                </li>
              ))}
            </ul>
          )}

          <p className="mt-2 mb-0">
            <Link href="/notifications" onClick={() => setOpen(false)} className="text-xs text-accent hover:text-accent-bright">
              Alle anzeigen
            </Link>
          </p>
        </div>
      )}
    </div>
  );
}
