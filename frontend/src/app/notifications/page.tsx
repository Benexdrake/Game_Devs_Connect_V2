"use client";

import { useCallback, useEffect, useState } from "react";
import clsx from "clsx";
import type { NotificationsResult } from "@/lib/types";
import { Button, PageContainer } from "@/components/ui";

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
    <PageContainer>
      <div className="flex items-center justify-between">
        <h1 className="font-display text-sm text-accent-bright">NOTIFICATIONS</h1>
        <Button type="button" variant="secondary" onClick={markAllRead}>
          Alle gelesen
        </Button>
      </div>

      {loading ? (
        <p className="mt-4 text-text-muted">Lade...</p>
      ) : !data || data.items.length === 0 ? (
        <p className="mt-4 text-text-muted">Keine Benachrichtigungen.</p>
      ) : (
        <ul className="mt-4 list-none space-y-1 p-0">
          {data.items.map((n) => (
            <li
              key={n.id}
              onClick={() => !n.isRead && markRead(n.id)}
              className={clsx(
                "rounded-md border border-transparent p-3",
                !n.isRead && "cursor-pointer border-accent/40 bg-accent/10",
              )}
            >
              <p className="m-0">{n.message}</p>
              <p className="m-0 text-xs text-text-muted">{new Date(n.createdAt).toLocaleString()}</p>
            </li>
          ))}
        </ul>
      )}
    </PageContainer>
  );
}
