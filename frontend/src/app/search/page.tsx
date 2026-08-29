import Link from "next/link";
import { apiFetchJson } from "@/lib/api";
import type { SearchResults } from "@/lib/types";

export default async function SearchPage({
  searchParams,
}: {
  searchParams: Promise<{ q?: string }>;
}) {
  const { q } = await searchParams;
  const results = q ? await apiFetchJson<SearchResults>(`/api/search?q=${encodeURIComponent(q)}`) : null;

  return (
    <main style={{ maxWidth: 720, margin: "0 auto", padding: "2rem 1rem" }}>
      <h1>Suche{q ? `: "${q}"` : ""}</h1>

      {!q && <p>Bitte einen Suchbegriff eingeben.</p>}

      {q && !results && <p>Keine Ergebnisse.</p>}

      {results && (
        <>
          <section style={{ marginBottom: "1.5rem" }}>
            <h2>Projects</h2>
            {results.projects.length === 0 ? (
              <p>Keine Projekte gefunden.</p>
            ) : (
              <ul style={{ listStyle: "none", padding: 0 }}>
                {results.projects.map((p) => (
                  <li key={p.slug} style={{ marginBottom: "0.5rem" }}>
                    <Link href={`/projects/${p.slug}`} style={{ fontWeight: 600 }}>{p.title}</Link>
                    {" — "}
                    {p.genre} · {p.engine}
                  </li>
                ))}
              </ul>
            )}
          </section>

          <section style={{ marginBottom: "1.5rem" }}>
            <h2>Quests</h2>
            {results.quests.length === 0 ? (
              <p>Keine Quests gefunden.</p>
            ) : (
              <ul style={{ listStyle: "none", padding: 0 }}>
                {results.quests.map((quest) => (
                  <li key={quest.id} style={{ marginBottom: "0.5rem" }}>
                    <Link href={`/quests/${quest.id}`} style={{ fontWeight: 600 }}>{quest.title}</Link>
                    {" — "}
                    {quest.projectTitle} · {quest.difficulty} · {quest.xpReward} XP
                  </li>
                ))}
              </ul>
            )}
          </section>

          <section>
            <h2>Users</h2>
            {results.users.length === 0 ? (
              <p>Keine User gefunden.</p>
            ) : (
              <ul style={{ listStyle: "none", padding: 0 }}>
                {results.users.map((user) => (
                  <li key={user.username} style={{ marginBottom: "0.5rem" }}>
                    <Link href={`/users/${user.username}`}>{user.username}</Link>
                  </li>
                ))}
              </ul>
            )}
          </section>
        </>
      )}
    </main>
  );
}
