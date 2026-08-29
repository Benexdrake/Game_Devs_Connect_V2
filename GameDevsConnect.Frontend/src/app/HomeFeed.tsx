"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import type { ActivityEvent, CurrentUser } from "@/lib/types";
import { Button, PageContainer, Panel } from "@/components/ui";

export function HomeFeed({ me, feed }: { me: CurrentUser; feed: ActivityEvent[] }) {
  const router = useRouter();

  async function handleLogout() {
    await fetch("/api/auth/logout", { method: "POST", credentials: "include" });
    router.push("/");
    router.refresh();
  }

  return (
    <PageContainer>
      <div className="mb-6 flex items-center gap-3">
        {me.avatarUrl && (
          // eslint-disable-next-line @next/next/no-img-element
          <img src={me.avatarUrl} alt={me.username} width={40} height={40} className="rounded-full" />
        )}
        <p className="m-0">
          Angemeldet als <span className="font-medium text-accent-bright">{me.username}</span>
        </p>
        <Button type="button" variant="ghost" onClick={handleLogout} className="ml-auto">
          Logout
        </Button>
      </div>

      <h2 className="mb-3 font-display text-xs text-accent-bright">DEIN FEED</h2>
      {feed.length === 0 ? (
        <Panel className="text-text-muted">
          Noch nichts zu sehen. Folge Usern oder Projekten, um ihre neuen Quests und angenommenen Contributions hier
          zu sehen.
        </Panel>
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
