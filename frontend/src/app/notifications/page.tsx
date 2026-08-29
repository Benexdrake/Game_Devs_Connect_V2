"use client";

import { useCallback, useEffect, useState } from "react";
import type { NotificationsResult } from "@/lib/types";

export default function NotificationsPage() {
  const [data, setData] = useState<NotificationsResult | null>(null);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch("/api/notifications?pageSize=50", { credentials: "include" });
      setData(res.ok ? await res.json() : null);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect -- client-side data load on mount, no server-driven alternative here
    load();
  }, [load]);

  async function markRead(id: string) {
    await fetch(`/api/notifications/${id}/read`, { method: "PATCH", credentials: "include" });
    load();
  }

  async function markAllRead() {
    await fetch("/api/notifications/read-all", { method: "PATCH", credentials: "include" });
    load();
  }

  return (
    <main style={{ maxWidth: 640, margin: "0 auto", padding: "2rem 1rem" }}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
        <h1>Notifications</h1>
        <button type="button" onClick={markAllRead}>
          Alle gelesen
        </button>
      </div>

      {loading ? (
        <p>Lade...</p>
      ) : !data || data.items.length === 0 ? (
        <p>Keine Benachrichtigungen.</p>
      ) : (
        <ul style={{ listStyle: "none", padding: 0 }}>
          {data.items.map((n) => (
            <li
              key={n.id}
              onClick={() => !n.isRead && markRead(n.id)}
              style={{
                padding: "0.75rem",
                borderBottom: "1px solid #eee",
                background: n.isRead ? "transparent" : "#f0f6ff",
                cursor: n.isRead ? "default" : "pointer",
              }}
            >
              <p style={{ margin: 0 }}>{n.message}</p>
              <p style={{ margin: 0, fontSize: "0.8em", color: "#888" }}>{new Date(n.createdAt).toLocaleString()}</p>
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}
