"use client";

import { useCallback, useEffect, useState } from "react";
import type { CurrentUser, Skill, UserLink, UserProfile } from "@/lib/types";
import { Button, Input, PageContainer, Textarea } from "@/components/ui";

export default function ProfileSettingsPage() {
  const [me, setMe] = useState<CurrentUser | null>(null);
  const [allSkills, setAllSkills] = useState<Skill[]>([]);
  const [bio, setBio] = useState("");
  const [links, setLinks] = useState<UserLink[]>([]);
  const [selectedSkillIds, setSelectedSkillIds] = useState<Set<string>>(new Set());
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const meRes = await fetch("/api/auth/me", { credentials: "include" });
      if (!meRes.ok) {
        setMe(null);
        return;
      }
      const currentUser = (await meRes.json()) as CurrentUser;
      setMe(currentUser);

      const [profileRes, skillsRes] = await Promise.all([
        fetch(`/api/users/${encodeURIComponent(currentUser.username)}`, { credentials: "include" }),
        fetch("/api/skills", { credentials: "include" }),
      ]);

      if (profileRes.ok) {
        const profile = (await profileRes.json()) as UserProfile;
        setBio(profile.bio ?? "");
        setLinks(profile.links);
        setSelectedSkillIds(new Set(profile.skills.map((s) => s.id)));
      }
      if (skillsRes.ok) {
        setAllSkills((await skillsRes.json()) as Skill[]);
      }
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect -- client-side profile load on mount
    load();
  }, [load]);

  function toggleSkill(id: string) {
    setSelectedSkillIds((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  }

  function updateLink(index: number, field: keyof UserLink, value: string) {
    setLinks((prev) => prev.map((l, i) => (i === index ? { ...l, [field]: value } : l)));
  }

  function addLink() {
    setLinks((prev) => [...prev, { label: "", url: "" }]);
  }

  function removeLink(index: number) {
    setLinks((prev) => prev.filter((_, i) => i !== index));
  }

  async function handleSave() {
    setSaving(true);
    setMessage(null);
    try {
      const profileRes = await fetch("/api/users/me", {
        method: "PATCH",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ bio, links: links.filter((l) => l.label && l.url) }),
      });
      const skillsRes = await fetch("/api/users/me/skills", {
        method: "PUT",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ skillIds: Array.from(selectedSkillIds) }),
      });
      setMessage(profileRes.ok && skillsRes.ok ? "Gespeichert." : "Speichern fehlgeschlagen.");
    } finally {
      setSaving(false);
    }
  }

  if (loading) return <PageContainer className="text-text-muted">Lade...</PageContainer>;
  if (!me) return <PageContainer className="text-text-muted">Bitte zuerst einloggen.</PageContainer>;

  const skillsByCategory = allSkills.reduce<Record<string, Skill[]>>((acc, skill) => {
    (acc[skill.category] ??= []).push(skill);
    return acc;
  }, {});

  return (
    <PageContainer>
      <h1 className="mb-6 font-display text-sm text-accent-bright">PROFIL BEARBEITEN</h1>

      <label htmlFor="bio" className="mb-1 block text-sm text-text-muted">Bio</label>
      <Textarea id="bio" value={bio} onChange={(e) => setBio(e.target.value)} rows={4} className="mb-6" />

      <h2 className="mb-2 font-display text-xs text-accent-bright">LINKS</h2>
      {links.map((link, index) => (
        <div key={index} className="mb-2 flex gap-2">
          <Input placeholder="Label" value={link.label} onChange={(e) => updateLink(index, "label", e.target.value)} className="w-32" />
          <Input placeholder="URL" value={link.url} onChange={(e) => updateLink(index, "url", e.target.value)} className="flex-1" />
          <Button type="button" variant="danger" onClick={() => removeLink(index)}>
            Entfernen
          </Button>
        </div>
      ))}
      <Button type="button" variant="secondary" onClick={addLink}>
        + Link hinzufügen
      </Button>

      <h2 className="mb-2 mt-6 font-display text-xs text-accent-bright">SKILLS</h2>
      {Object.entries(skillsByCategory).map(([category, skills]) => (
        <fieldset key={category} className="mb-3 rounded-md border border-border p-3">
          <legend className="px-1 text-sm text-text-muted">{category}</legend>
          <div className="flex flex-wrap gap-3">
            {skills.map((skill) => (
              <label key={skill.id} className="flex items-center gap-1.5 text-sm">
                <input
                  type="checkbox"
                  checked={selectedSkillIds.has(skill.id)}
                  onChange={() => toggleSkill(skill.id)}
                  className="accent-accent"
                />
                {skill.name}
              </label>
            ))}
          </div>
        </fieldset>
      ))}

      <Button type="button" onClick={handleSave} disabled={saving} className="mt-4">
        {saving ? "Speichere..." : "Speichern"}
      </Button>
      {message && <p className="mt-3 text-sm text-text-muted">{message}</p>}
    </PageContainer>
  );
}
