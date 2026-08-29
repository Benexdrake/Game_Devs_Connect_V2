"use client";

import Link from "next/link";
import { useState } from "react";
import clsx from "clsx";
import type { ActivityEvent } from "@/lib/types";
import { PageContainer, Panel } from "@/components/ui";

type Tab = "forYou" | "following";

const TABS: { id: Tab; label: string }[] = [
  { id: "forYou", label: "Für dich" },
  { id: "following", label: "Folge ich" },
];

const EMPTY_STATE: Record<Tab, string> = {
  forYou: "Noch nichts zu sehen. Wähle Skills in deinem Profil aus, damit wir passende Quests und Updates finden.",
  following: "Noch nichts zu sehen. Folge Usern oder Projekten, um ihre neuen Quests und angenommenen Contributions hier zu sehen.",
};

export function HomeFeed({ forYou, following }: { forYou: ActivityEvent[]; following: ActivityEvent[] }) {
  const [tab, setTab] = useState<Tab>("forYou");
  const feed = tab === "forYou" ? forYou : following;

  return (
    <PageContainer>
      <nav className="mb-4 flex gap-1 border-b border-border">
        {TABS.map((t) => (
          <button
            key={t.id}
            type="button"
            onClick={() => setTab(t.id)}
            className={clsx(
              "border-b-2 px-3 py-2 text-sm transition-colors",
              tab === t.id
                ? "border-accent-bright text-accent-bright"
                : "border-transparent text-text-muted hover:text-text",
            )}
          >
            {t.label}
          </button>
        ))}
      </nav>

      {feed.length === 0 ? (
        <Panel className="text-text-muted">{EMPTY_STATE[tab]}</Panel>
      ) : (
        <ul className="list-none space-y-2 p-0">
          {feed.map((event) => (
            <li key={event.id} className="border-b border-border py-3">
              {event.linkUrl ? (
                <Link href={event.linkUrl} className="text-text hover:text-accent-bright">
                  {event.summary}
                </Link>
              ) : (
                <p className="m-0">{event.summary}</p>
              )}
              <p className="m-0 text-xs text-text-muted">{new Date(event.createdAt).toLocaleString()}</p>
            </li>
          ))}
        </ul>
      )}
    </PageContainer>
  );
}
