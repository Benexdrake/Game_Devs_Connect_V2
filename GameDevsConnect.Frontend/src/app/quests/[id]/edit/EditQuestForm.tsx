"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { SKILL_CATEGORY_LABELS, type Quest, type QuestDifficulty, type Skill, type SkillCategory } from "@/lib/types";
import { BackLink, Button, Input, PageContainer, Select, Textarea } from "@/components/ui";

const CATEGORIES: SkillCategory[] = ["Programming", "Engines", "Art2D", "Art3D", "Animation", "Audio", "Design", "Writing", "Production", "Other"];
const DIFFICULTIES = ["Easy", "Medium", "Hard"] as const;

// Mirrors QuestDifficultyXp.For on the backend - the server computes the
// real value, this is only shown so edits preview the reward up front.
const XP_BY_DIFFICULTY: Record<(typeof DIFFICULTIES)[number], number> = {
  Easy: 100,
  Medium: 250,
  Hard: 500,
};

export function EditQuestForm({ quest, skills }: { quest: Quest; skills: Skill[] }) {
  const router = useRouter();
  const [title, setTitle] = useState(quest.title);
  const [description, setDescription] = useState(quest.description ?? "");
  const [category, setCategory] = useState<SkillCategory>(quest.category);
  const [difficulty, setDifficulty] = useState<QuestDifficulty>(quest.difficulty);
  const [maxContributors, setMaxContributors] = useState(quest.maxContributors);
  const [selectedSkillIds, setSelectedSkillIds] = useState<string[]>(quest.requiredSkills.map((s) => s.id));
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function toggleSkill(id: string) {
    setSelectedSkillIds((prev) => (prev.includes(id) ? prev.filter((s) => s !== id) : [...prev, id]));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSaving(true);
    setError(null);
    try {
      const res = await fetch(`/api/projects/${quest.projectSlug}/quests/${quest.id}`, {
        method: "PATCH",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          title,
          description,
          category,
          difficulty,
          maxContributors,
          requiredSkillIds: selectedSkillIds,
        }),
      });
      if (!res.ok) {
        setError("Speichern fehlgeschlagen (nur möglich solange die Quest offen ist).");
        return;
      }
      router.push(`/quests/${quest.id}`);
      router.refresh();
    } finally {
      setSaving(false);
    }
  }

  async function handleCancel() {
    if (!confirm(`Quest "${quest.title}" wirklich abbrechen?`)) return;
    const res = await fetch(`/api/projects/${quest.projectSlug}/quests/${quest.id}`, {
      method: "DELETE",
      credentials: "include",
    });
    if (res.ok) {
      router.push(`/projects/${quest.projectSlug}`);
    } else {
      setError("Abbrechen fehlgeschlagen.");
    }
  }

  return (
    <PageContainer width="md">
      <div className="mb-6 flex items-center gap-2">
        <BackLink fallbackHref={`/quests/${quest.id}`} />
        <h1 className="font-display text-sm text-accent-bright">QUEST BEARBEITEN</h1>
      </div>
      <form onSubmit={handleSubmit}>
        <label htmlFor="title" className="mb-1 block text-sm text-text-muted">Titel</label>
        <Input id="title" value={title} onChange={(e) => setTitle(e.target.value)} required className="mb-4" />

        <label htmlFor="description" className="mb-1 block text-sm text-text-muted">Beschreibung</label>
        <Textarea
          id="description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          rows={4}
          className="mb-4"
        />

        <label htmlFor="category" className="mb-1 block text-sm text-text-muted">Kategorie</label>
        <Select id="category" value={category} onChange={(e) => setCategory(e.target.value as SkillCategory)} className="mb-4">
          {CATEGORIES.map((c) => (
            <option key={c} value={c}>{SKILL_CATEGORY_LABELS[c]}</option>
          ))}
        </Select>

        <label htmlFor="difficulty" className="mb-1 block text-sm text-text-muted">Schwierigkeit</label>
        <Select
          id="difficulty"
          value={difficulty}
          onChange={(e) => setDifficulty(e.target.value as QuestDifficulty)}
          className="mb-4"
        >
          {DIFFICULTIES.map((d) => (
            <option key={d} value={d}>{d}</option>
          ))}
        </Select>

        <label htmlFor="xpReward" className="mb-1 block text-sm text-text-muted">
          XP Reward (durch Schwierigkeit festgelegt)
        </label>
        <Input id="xpReward" type="number" value={XP_BY_DIFFICULTY[difficulty]} readOnly disabled className="mb-4" />

        <label htmlFor="maxContributors" className="mb-1 block text-sm text-text-muted">Max. Contributors</label>
        <Input
          id="maxContributors"
          type="number"
          min={1}
          value={maxContributors}
          onChange={(e) => setMaxContributors(Number(e.target.value))}
          className="mb-4"
        />

        <fieldset className="mb-4 rounded-md border border-border p-3">
          <legend className="px-1 text-sm text-text-muted">Benötigte Skills</legend>
          <div className="grid grid-cols-2 gap-1">
            {skills.map((skill) => (
              <label key={skill.id} className="flex items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  checked={selectedSkillIds.includes(skill.id)}
                  onChange={() => toggleSkill(skill.id)}
                  className="accent-accent"
                />
                {skill.name}
              </label>
            ))}
          </div>
        </fieldset>

        <Button type="submit" disabled={saving}>
          {saving ? "Speichere..." : "Speichern"}
        </Button>
      </form>

      <hr className="my-8 border-border" />
      <Button type="button" variant="danger" onClick={handleCancel}>
        Quest abbrechen
      </Button>

      {error && <p className="mt-3 text-sm text-danger">{error}</p>}
    </PageContainer>
  );
}
