"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import type { Project, ProjectVisibility } from "@/lib/types";

export function ProjectSettingsForm({ project, isOwner }: { project: Project; isOwner: boolean }) {
  const router = useRouter();
  const [title, setTitle] = useState(project.title);
  const [description, setDescription] = useState(project.description ?? "");
  const [engine, setEngine] = useState(project.engine ?? "");
  const [genre, setGenre] = useState(project.genre ?? "");
  const [visibility, setVisibility] = useState<ProjectVisibility>(project.visibility);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState<string | null>(null);

  async function handleSave(e: React.FormEvent) {
    e.preventDefault();
    setSaving(true);
    setMessage(null);
    try {
      const res = await fetch(`/api/projects/${project.slug}`, {
        method: "PATCH",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title, description, engine, genre, visibility }),
      });
      setMessage(res.ok ? "Gespeichert." : "Speichern fehlgeschlagen.");
      if (res.ok) router.refresh();
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete() {
    if (!confirm(`Projekt "${project.title}" wirklich löschen?`)) return;
    const res = await fetch(`/api/projects/${project.slug}`, { method: "DELETE", credentials: "include" });
    if (res.ok) router.push("/");
    else setMessage("Löschen fehlgeschlagen.");
  }

  return (
    <main style={{ maxWidth: 480, margin: "0 auto", padding: "2rem 1rem" }}>
      <h1>Projekt-Einstellungen</h1>
      <form onSubmit={handleSave}>
        <label htmlFor="title">Titel</label>
        <input id="title" value={title} onChange={(e) => setTitle(e.target.value)} style={{ display: "block", width: "100%", marginBottom: "1rem" }} />

        <label htmlFor="description">Beschreibung</label>
        <textarea id="description" value={description} onChange={(e) => setDescription(e.target.value)} rows={4} style={{ display: "block", width: "100%", marginBottom: "1rem" }} />

        <label htmlFor="engine">Engine</label>
        <input id="engine" value={engine} onChange={(e) => setEngine(e.target.value)} style={{ display: "block", width: "100%", marginBottom: "1rem" }} />

        <label htmlFor="genre">Genre</label>
        <input id="genre" value={genre} onChange={(e) => setGenre(e.target.value)} style={{ display: "block", width: "100%", marginBottom: "1rem" }} />

        <label htmlFor="visibility">Sichtbarkeit</label>
        <select
          id="visibility"
          value={visibility}
          onChange={(e) => setVisibility(e.target.value as ProjectVisibility)}
          style={{ display: "block", width: "100%", marginBottom: "1rem" }}
        >
          <option value="Public">Public</option>
          <option value="Private">Private</option>
        </select>

        <button type="submit" disabled={saving}>{saving ? "Speichere..." : "Speichern"}</button>
      </form>

      {isOwner && (
        <>
          <hr style={{ margin: "2rem 0" }} />
          <button type="button" onClick={handleDelete} style={{ color: "red" }}>
            Projekt löschen
          </button>
        </>
      )}

      {message && <p>{message}</p>}
    </main>
  );
}
