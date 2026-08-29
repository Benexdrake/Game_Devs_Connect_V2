import Link from "next/link";
import { apiFetchJson } from "@/lib/api";
import { SKILL_CATEGORY_LABELS, type Engine, type Quest, type Skill, type SkillCategory } from "@/lib/types";
import { Badge, Button, Input, PageContainer, Panel, Select } from "@/components/ui";

type SearchParams = {
  search?: string;
  category?: string;
  skillId?: string;
  projectSlug?: string;
  difficulty?: string;
  minXp?: string;
  engineId?: string;
};

const CATEGORIES: SkillCategory[] = ["Programming", "Engines", "Art2D", "Art3D", "Animation", "Audio", "Design", "Writing", "Production", "Other"];
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

  const [quests, skills, engines] = await Promise.all([
    apiFetchJson<Quest[]>(`/api/quests?${query.toString()}`),
    apiFetchJson<Skill[]>("/api/skills"),
    apiFetchJson<Engine[]>("/api/engines"),
  ]);

  return (
    <PageContainer>
      <h1 className="mb-6 font-display text-sm text-accent-bright">QUESTS ENTDECKEN</h1>

      <form method="get" className="mb-6 flex flex-wrap gap-2">
        <Input name="search" placeholder="Suche..." defaultValue={params.search ?? ""} className="w-40" />
        <Select name="category" defaultValue={params.category ?? ""} className="w-auto">
          <option value="">Alle Kategorien</option>
          {CATEGORIES.map((c) => (
            <option key={c} value={c}>{SKILL_CATEGORY_LABELS[c]}</option>
          ))}
        </Select>
        <Select name="skillId" defaultValue={params.skillId ?? ""} className="w-auto">
          <option value="">Alle Skills</option>
          {(skills ?? []).map((s) => (
            <option key={s.id} value={s.id}>{s.name}</option>
          ))}
        </Select>
        <Select name="difficulty" defaultValue={params.difficulty ?? ""} className="w-auto">
          <option value="">Alle Schwierigkeiten</option>
          {DIFFICULTIES.map((d) => (
            <option key={d} value={d}>{d}</option>
          ))}
        </Select>
        <Input name="projectSlug" placeholder="Projekt-Slug" defaultValue={params.projectSlug ?? ""} className="w-32" />
        <Select name="engineId" defaultValue={params.engineId ?? ""} className="w-auto">
          <option value="">Alle Engines</option>
          {(engines ?? []).map((e) => (
            <option key={e.id} value={e.id}>{e.name}</option>
          ))}
        </Select>
        <Input name="minXp" type="number" placeholder="Min. XP" defaultValue={params.minXp ?? ""} className="w-24" />
        <Button type="submit">Filtern</Button>
      </form>

      {!quests || quests.length === 0 ? (
        <Panel className="text-text-muted">Keine offenen Quests gefunden.</Panel>
      ) : (
        <ul className="list-none space-y-3 p-0">
          {quests.map((quest) => (
            <li key={quest.id}>
              <Panel>
                <Link href={`/quests/${quest.id}`} className="font-medium text-text hover:text-accent-bright">
                  {quest.title}
                </Link>
                <div className="mt-1 flex flex-wrap items-center gap-2 text-sm text-text-muted">
                  <span>{quest.projectTitle}</span>
                  <Badge>{SKILL_CATEGORY_LABELS[quest.category]}</Badge>
                  <Badge tone="accent">{quest.difficulty}</Badge>
                  <Badge tone="warning">{quest.xpReward} XP</Badge>
                </div>
                {quest.requiredSkills.length > 0 && (
                  <p className="m-0 mt-2 text-sm text-text-muted">
                    Skills: {quest.requiredSkills.map((s) => s.name).join(", ")}
                  </p>
                )}
              </Panel>
            </li>
          ))}
        </ul>
      )}
    </PageContainer>
  );
}
