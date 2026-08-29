"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
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
    <div style={{ position: "relative", marginLeft: "auto" }}>
      <button type="button" onClick={() => setOpen((o) => !o)}>
        🔔{data && data.unreadCount > 0 ? ` (${data.unreadCount})` : ""}
      </button>

      {open && (
        <div
          style={{
            position: "absolute",
            right: 0,
            top: "100%",
            background: "white",
            border: "1px solid #ccc",
            borderRadius: 8,
            width: 320,
            maxHeight: 400,
            overflowY: "auto",
            zIndex: 10,
            padding: "0.5rem",
          }}
        >
          <div style={{ display: "flex", justifyContent: "space-between", marginBottom: "0.5rem" }}>
            <strong>Notifications</strong>
            <button type="button" onClick={markAllRead}>
              Alle gelesen
            </button>
          </div>

          {!data || data.items.length === 0 ? (
            <p>Keine Benachrichtigungen.</p>
          ) : (
            <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
              {data.items.map((n) => (
                <li
                  key={n.id}
                  onClick={() => !n.isRead && markRead(n.id)}
                  style={{
                    padding: "0.5rem",
                    borderBottom: "1px solid #eee",
                    background: n.isRead ? "transparent" : "#f0f6ff",
                    cursor: n.isRead ? "default" : "pointer",
                  }}
                >
                  <p style={{ margin: 0, fontSize: "0.9em" }}>{n.message}</p>
                  <p style={{ margin: 0, fontSize: "0.75em", color: "#888" }}>
                    {new Date(n.createdAt).toLocaleString()}
                  </p>
                </li>
              ))}
            </ul>
          )}

          <p style={{ marginTop: "0.5rem", marginBottom: 0 }}>
            <Link href="/notifications" onClick={() => setOpen(false)}>
              Alle anzeigen
            </Link>
          </p>
        </div>
      )}
    </div>
  );
}
