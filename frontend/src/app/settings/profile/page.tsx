"use client";

import { useCallback, useEffect, useState } from "react";
import type { CurrentUser, Skill, UserLink, UserProfile } from "@/lib/types";

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

  if (loading) return <main style={{ padding: "2rem" }}>Lade...</main>;
  if (!me) return <main style={{ padding: "2rem" }}>Bitte zuerst einloggen.</main>;

  const skillsByCategory = allSkills.reduce<Record<string, Skill[]>>((acc, skill) => {
    (acc[skill.category] ??= []).push(skill);
    return acc;
  }, {});

  return (
    <main style={{ maxWidth: 640, margin: "0 auto", padding: "2rem 1rem" }}>
      <h1>Profil bearbeiten</h1>

      <label htmlFor="bio">Bio</label>
      <textarea
        id="bio"
        value={bio}
        onChange={(e) => setBio(e.target.value)}
        rows={4}
        style={{ width: "100%", display: "block", marginBottom: "1rem" }}
      />

      <h2>Links</h2>
      {links.map((link, index) => (
        <div key={index} style={{ display: "flex", gap: "0.5rem", marginBottom: "0.5rem" }}>
          <input
            placeholder="Label"
            value={link.label}
            onChange={(e) => updateLink(index, "label", e.target.value)}
          />
          <input
            placeholder="URL"
            value={link.url}
            onChange={(e) => updateLink(index, "url", e.target.value)}
            style={{ flex: 1 }}
          />
          <button type="button" onClick={() => removeLink(index)}>
            Entfernen
          </button>
        </div>
      ))}
      <button type="button" onClick={addLink}>
        + Link hinzufügen
      </button>

      <h2>Skills</h2>
      {Object.entries(skillsByCategory).map(([category, skills]) => (
        <fieldset key={category} style={{ marginBottom: "0.5rem" }}>
          <legend>{category}</legend>
          {skills.map((skill) => (
            <label key={skill.id} style={{ marginRight: "1rem" }}>
              <input
                type="checkbox"
                checked={selectedSkillIds.has(skill.id)}
                onChange={() => toggleSkill(skill.id)}
              />{" "}
              {skill.name}
            </label>
          ))}
        </fieldset>
      ))}

      <button type="button" onClick={handleSave} disabled={saving} style={{ marginTop: "1rem" }}>
        {saving ? "Speichere..." : "Speichern"}
      </button>
      {message && <p>{message}</p>}
    </main>
  );
}
