"use client";

import { useCallback, useEffect, useState } from "react";
import { X } from "lucide-react";
import clsx from "clsx";
import type { NotificationsResult } from "@/lib/types";
import { PageContainer } from "@/components/ui";

export default function NotificationsPage() {
  const [data, setData] = useState<NotificationsResult | null>(null);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch("/api/notifications?pageSize=50", { credentials: "include" });
      const result: NotificationsResult | null = res.ok ? await res.json() : null;
      setData(result);

      // Seeing this page counts as reading everything on it: mark unread
      // notifications read in the background without re-rendering, so the
      // "unread" highlight still shows what was new during this visit.
      if (result && result.unreadCount > 0) {
        void fetch("/api/notifications/read-all", { method: "PATCH", credentials: "include" });
      }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect -- client-side data load on mount, no server-driven alternative here
    load();
  }, [load]);

  async function remove(id: string) {
    setData((prev) => (prev ? { ...prev, items: prev.items.filter((n) => n.id !== id) } : prev));
    await fetch(`/api/notifications/${id}`, { method: "DELETE", credentials: "include" });
  }

  return (
    <PageContainer>
      <h1 className="font-display text-sm text-accent-bright">NOTIFICATIONS</h1>

      {loading ? (
        <p className="mt-4 text-text-muted">Lade...</p>
      ) : !data || data.items.length === 0 ? (
        <p className="mt-4 text-text-muted">Keine Benachrichtigungen.</p>
      ) : (
        <ul className="mt-4 list-none space-y-1 p-0">
          {data.items.map((n) => (
            <li
              key={n.id}
              className={clsx(
                "flex items-start gap-2 rounded-md border border-transparent p-3",
                !n.isRead && "border-accent/40 bg-accent/10",
              )}
            >
              <div className="flex-1">
                <p className="m-0">{n.message}</p>
                <p className="m-0 text-xs text-text-muted">{new Date(n.createdAt).toLocaleString()}</p>
              </div>
              <button
                type="button"
                aria-label="Löschen"
                title="Löschen"
                onClick={() => remove(n.id)}
                className="rounded p-1 text-text-muted hover:bg-canvas hover:text-danger"
              >
                <X size={16} />
              </button>
            </li>
          ))}
        </ul>
      )}
    </PageContainer>
  );
}
