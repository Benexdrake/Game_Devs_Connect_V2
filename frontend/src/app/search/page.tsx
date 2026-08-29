import Link from "next/link";
import { apiFetchJson } from "@/lib/api";
import type { SearchResults } from "@/lib/types";
import { PageContainer, Panel } from "@/components/ui";

export default async function SearchPage({
  searchParams,
}: {
  searchParams: Promise<{ q?: string }>;
}) {
  const { q } = await searchParams;
  const results = q ? await apiFetchJson<SearchResults>(`/api/search?q=${encodeURIComponent(q)}`) : null;

  return (
    <PageContainer>
      <h1 className="mb-6 font-display text-sm text-accent-bright">
        SUCHE{q ? `: "${q}"` : ""}
      </h1>

      {!q && <p className="text-text-muted">Bitte einen Suchbegriff eingeben.</p>}

      {q && !results && <Panel className="text-text-muted">Keine Ergebnisse.</Panel>}

      {results && (
        <>
          <section className="mb-6">
            <h2 className="mb-2 font-display text-xs text-accent-bright">PROJECTS</h2>
            {results.projects.length === 0 ? (
              <p className="text-text-muted">Keine Projekte gefunden.</p>
            ) : (
              <ul className="list-none space-y-1 p-0">
                {results.projects.map((p) => (
                  <li key={p.slug}>
                    <Link href={`/projects/${p.slug}`} className="font-medium text-text hover:text-accent-bright">
                      {p.title}
                    </Link>
                    <span className="text-text-muted"> — {p.genre} · {p.engine}</span>
                  </li>
                ))}
              </ul>
            )}
          </section>

          <section className="mb-6">
            <h2 className="mb-2 font-display text-xs text-accent-bright">QUESTS</h2>
            {results.quests.length === 0 ? (
              <p className="text-text-muted">Keine Quests gefunden.</p>
            ) : (
              <ul className="list-none space-y-1 p-0">
                {results.quests.map((quest) => (
                  <li key={quest.id}>
                    <Link href={`/quests/${quest.id}`} className="font-medium text-text hover:text-accent-bright">
                      {quest.title}
                    </Link>
                    <span className="text-text-muted"> — {quest.projectTitle} · {quest.difficulty} · {quest.xpReward} XP</span>
                  </li>
                ))}
              </ul>
            )}
          </section>

          <section>
            <h2 className="mb-2 font-display text-xs text-accent-bright">USERS</h2>
            {results.users.length === 0 ? (
              <p className="text-text-muted">Keine User gefunden.</p>
            ) : (
              <ul className="list-none space-y-1 p-0">
                {results.users.map((user) => (
                  <li key={user.username}>
                    <Link href={`/users/${user.username}`} className="text-accent hover:text-accent-bright">
                      {user.username}
                    </Link>
                  </li>
                ))}
              </ul>
            )}
          </section>
        </>
      )}
    </PageContainer>
  );
}
