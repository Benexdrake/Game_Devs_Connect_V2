"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import type { Skill } from "@/lib/types";

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
    <main style={{ maxWidth: 480, margin: "0 auto", padding: "2rem 1rem" }}>
      <h1>Neue Quest</h1>
      <form onSubmit={handleSubmit}>
        <label htmlFor="title">Titel</label>
        <input
          id="title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          required
          style={{ display: "block", width: "100%", marginBottom: "1rem" }}
        />

        <label htmlFor="description">Beschreibung</label>
        <textarea
          id="description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          rows={4}
          style={{ display: "block", width: "100%", marginBottom: "1rem" }}
        />

        <label htmlFor="category">Kategorie</label>
        <select
          id="category"
          value={category}
          onChange={(e) => setCategory(e.target.value)}
          style={{ display: "block", width: "100%", marginBottom: "1rem" }}
        >
          {CATEGORIES.map((c) => (
            <option key={c} value={c}>{c}</option>
          ))}
        </select>

        <label htmlFor="difficulty">Schwierigkeit</label>
        <select
          id="difficulty"
          value={difficulty}
          onChange={(e) => setDifficulty(e.target.value as (typeof DIFFICULTIES)[number])}
          style={{ display: "block", width: "100%", marginBottom: "1rem" }}
        >
          {DIFFICULTIES.map((d) => (
            <option key={d} value={d}>{d}</option>
          ))}
        </select>

        <label htmlFor="xpReward">XP Reward (durch Schwierigkeit festgelegt)</label>
        <input
          id="xpReward"
          type="number"
          value={XP_BY_DIFFICULTY[difficulty]}
          readOnly
          disabled
          style={{ display: "block", width: "100%", marginBottom: "1rem" }}
        />

        <label htmlFor="maxContributors">Max. Contributors</label>
        <input
          id="maxContributors"
          type="number"
          min={1}
          value={maxContributors}
          onChange={(e) => setMaxContributors(Number(e.target.value))}
          style={{ display: "block", width: "100%", marginBottom: "1rem" }}
        />

        <fieldset style={{ marginBottom: "1rem" }}>
          <legend>Benötigte Skills</legend>
          {skills.map((skill) => (
            <label key={skill.id} style={{ display: "block" }}>
              <input
                type="checkbox"
                checked={selectedSkillIds.includes(skill.id)}
                onChange={() => toggleSkill(skill.id)}
              />{" "}
              {skill.name}
            </label>
          ))}
        </fieldset>

        <button type="submit" disabled={saving}>
          {saving ? "Speichere..." : "Quest erstellen"}
        </button>
      </form>
      {error && <p style={{ color: "red" }}>{error}</p>}
    </main>
  );
}
