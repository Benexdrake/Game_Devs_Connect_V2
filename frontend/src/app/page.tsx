"use client";

import Link from "next/link";
import { useCallback, useEffect, useState } from "react";
import styles from "./page.module.css";

type CurrentUser = {
  id: string;
  username: string;
  avatarUrl: string | null;
};

export default function Home() {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchCurrentUser = useCallback(async () => {
    setLoading(true);
    try {
      const res = await fetch("/api/auth/me", { credentials: "include" });
      setUser(res.ok ? await res.json() : null);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect -- client-side session check on mount, no server-driven alternative here
    fetchCurrentUser();
  }, [fetchCurrentUser]);

  async function handleLogout() {
    await fetch("/api/auth/logout", { method: "POST", credentials: "include" });
    setUser(null);
  }

  return (
    <div className={styles.page}>
      <main className={styles.main}>
        <h1>Gamedevs Connect</h1>

        {loading ? (
          <p>Lade...</p>
        ) : user ? (
          <div>
            {user.avatarUrl && (
              // eslint-disable-next-line @next/next/no-img-element
              <img
                src={user.avatarUrl}
                alt={user.username}
                width={48}
                height={48}
                style={{ borderRadius: "50%" }}
              />
            )}
            <p>Angemeldet als {user.username}</p>
            <nav style={{ display: "flex", gap: "1rem", marginBottom: "1rem" }}>
              <Link href={`/users/${user.username}`}>Mein Profil</Link>
              <Link href="/settings/profile">Profil bearbeiten</Link>
              <Link href="/projects/new">Neues Projekt</Link>
              <Link href="/quests">Quests entdecken</Link>
            </nav>
            <button onClick={handleLogout}>Logout</button>
          </div>
        ) : (
          <a href="/api/auth/login/github">Login with GitHub</a>
        )}
      </main>
    </div>
  );
}
