"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import type { Skill } from "@/lib/types";
import { Button, Input, PageContainer, Select, Textarea } from "@/components/ui";

const CATEGORIES = ["Programming", "Art2D", "Art3D", "Animation", "Audio", "Design", "Writing", "Other"];
const DIFFICULTIES = ["Easy", "Medium", "Hard"] as const;

// Mirrors QuestDifficultyXp.For on the backend - the server computes the
// real value, this is only shown so the creator sees the reward up front.
const XP_BY_DIFFICULTY: Record<(typeof DIFFICULTIES)[number], number> = {
  Easy: 100,
  Medium: 250,
  Hard: 500,
};

export function NewQuestForm({ projectSlug, skills }: { projectSlug: string; skills: Skill[] }) {
  const router = useRouter();
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [category, setCategory] = useState(CATEGORIES[0]);
  const [difficulty, setDifficulty] = useState<(typeof DIFFICULTIES)[number]>(DIFFICULTIES[0]);
  const [maxContributors, setMaxContributors] = useState(1);
  const [selectedSkillIds, setSelectedSkillIds] = useState<string[]>([]);
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
      const res = await fetch(`/api/projects/${projectSlug}/quests`, {
        method: "POST",
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
        setError("Erstellen fehlgeschlagen.");
        return;
      }
      router.push(`/projects/${projectSlug}`);
      router.refresh();
    } finally {
      setSaving(false);
    }
  }

  return (
    <PageContainer width="md">
      <h1 className="mb-6 font-display text-sm text-accent-bright">NEUE QUEST</h1>
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
        <Select id="category" value={category} onChange={(e) => setCategory(e.target.value)} className="mb-4">
          {CATEGORIES.map((c) => (
            <option key={c} value={c}>{c}</option>
          ))}
        </Select>

        <label htmlFor="difficulty" className="mb-1 block text-sm text-text-muted">Schwierigkeit</label>
        <Select
          id="difficulty"
          value={difficulty}
          onChange={(e) => setDifficulty(e.target.value as (typeof DIFFICULTIES)[number])}
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
          {saving ? "Speichere..." : "Quest erstellen"}
        </Button>
      </form>
      {error && <p className="mt-3 text-sm text-danger">{error}</p>}
    </PageContainer>
  );
}
