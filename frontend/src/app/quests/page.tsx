import Link from "next/link";
import { apiFetchJson } from "@/lib/api";
import type { Quest, Skill } from "@/lib/types";

type SearchParams = {
  search?: string;
  category?: string;
  skillId?: string;
  projectSlug?: string;
  difficulty?: string;
  minXp?: string;
  engine?: string;
};

const CATEGORIES = ["Programming", "Art2D", "Art3D", "Animation", "Audio", "Design", "Writing", "Other"];
const DIFFICULTIES = ["Easy", "Medium", "Hard"];

export default async function QuestsPage({
  searchParams,
}: {
  searchParams: Promise<SearchParams>;
}) {
  const params = await searchParams;
  const query = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value) query.set(key, value);
  }

  const [quests, skills] = await Promise.all([
    apiFetchJson<Quest[]>(`/api/quests?${query.toString()}`),
    apiFetchJson<Skill[]>("/api/skills"),
  ]);

  return (
    <main style={{ maxWidth: 720, margin: "0 auto", padding: "2rem 1rem" }}>
      <h1>Quests entdecken</h1>

      <form method="get" style={{ display: "flex", flexWrap: "wrap", gap: "0.5rem", marginBottom: "1.5rem" }}>
        <input name="search" placeholder="Suche..." defaultValue={params.search ?? ""} />
        <select name="category" defaultValue={params.category ?? ""}>
          <option value="">Alle Kategorien</option>
          {CATEGORIES.map((c) => (
            <option key={c} value={c}>{c}</option>
          ))}
        </select>
        <select name="skillId" defaultValue={params.skillId ?? ""}>
          <option value="">Alle Skills</option>
          {(skills ?? []).map((s) => (
            <option key={s.id} value={s.id}>{s.name}</option>
          ))}
        </select>
        <select name="difficulty" defaultValue={params.difficulty ?? ""}>
          <option value="">Alle Schwierigkeiten</option>
          {DIFFICULTIES.map((d) => (
            <option key={d} value={d}>{d}</option>
          ))}
        </select>
        <input name="projectSlug" placeholder="Projekt-Slug" defaultValue={params.projectSlug ?? ""} />
        <input name="engine" placeholder="Engine" defaultValue={params.engine ?? ""} />
        <input name="minXp" type="number" placeholder="Min. XP" defaultValue={params.minXp ?? ""} />
        <button type="submit">Filtern</button>
      </form>

      {!quests || quests.length === 0 ? (
        <p>Keine offenen Quests gefunden.</p>
      ) : (
        <ul style={{ listStyle: "none", padding: 0 }}>
          {quests.map((quest) => (
            <li key={quest.id} style={{ border: "1px solid #ccc", borderRadius: 8, padding: "1rem", marginBottom: "0.75rem" }}>
              <Link href={`/quests/${quest.id}`} style={{ fontWeight: 600 }}>{quest.title}</Link>
              <p style={{ margin: "0.25rem 0" }}>
                {quest.projectTitle} · {quest.category} · {quest.difficulty} · {quest.xpReward} XP
              </p>
              {quest.requiredSkills.length > 0 && (
                <p style={{ margin: 0, color: "#666" }}>
                  Skills: {quest.requiredSkills.map((s) => s.name).join(", ")}
                </p>
              )}
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}
