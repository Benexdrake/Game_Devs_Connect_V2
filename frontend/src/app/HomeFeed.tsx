"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import type { ActivityEvent, CurrentUser } from "@/lib/types";

export function HomeFeed({ me, feed }: { me: CurrentUser; feed: ActivityEvent[] }) {
  const router = useRouter();

  async function handleLogout() {
    await fetch("/api/auth/logout", { method: "POST", credentials: "include" });
    router.push("/");
    router.refresh();
  }

  return (
    <main style={{ maxWidth: 640, margin: "0 auto", padding: "2rem 1rem" }}>
      <div style={{ display: "flex", alignItems: "center", gap: "1rem", marginBottom: "1rem" }}>
        {me.avatarUrl && (
          // eslint-disable-next-line @next/next/no-img-element
          <img src={me.avatarUrl} alt={me.username} width={40} height={40} style={{ borderRadius: "50%" }} />
        )}
        <p style={{ margin: 0 }}>Angemeldet als {me.username}</p>
      </div>

      <nav style={{ display: "flex", gap: "1rem", marginBottom: "1.5rem", flexWrap: "wrap" }}>
        <Link href={`/users/${me.username}`}>Mein Profil</Link>
        <Link href="/settings/profile">Profil bearbeiten</Link>
        <Link href="/projects/new">Neues Projekt</Link>
        <Link href="/quests">Quests entdecken</Link>
        <button type="button" onClick={handleLogout}>
          Logout
        </button>
      </nav>

      <h2>Dein Feed</h2>
      {feed.length === 0 ? (
        <p>
          Noch nichts zu sehen. Folge Usern oder Projekten, um ihre neuen Quests und angenommenen Contributions hier
          zu sehen.
        </p>
      ) : (
        <ul style={{ listStyle: "none", padding: 0 }}>
          {feed.map((event) => (
            <li key={event.id} style={{ borderBottom: "1px solid #eee", padding: "0.75rem 0" }}>
              <p style={{ margin: 0 }}>{event.summary}</p>
              <p style={{ margin: 0, fontSize: "0.8em", color: "#888" }}>{new Date(event.createdAt).toLocaleString()}</p>
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}
