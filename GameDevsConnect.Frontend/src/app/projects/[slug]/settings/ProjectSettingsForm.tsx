"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import type { Project, ProjectVisibility } from "@/lib/types";
import { Button, Input, PageContainer, Select, Textarea } from "@/components/ui";

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
    <PageContainer className="max-w-md">
      <h1 className="mb-6 font-display text-sm text-accent-bright">PROJEKT-EINSTELLUNGEN</h1>
      <form onSubmit={handleSave}>
        <label htmlFor="title" className="mb-1 block text-sm text-text-muted">Titel</label>
        <Input id="title" value={title} onChange={(e) => setTitle(e.target.value)} className="mb-4" />

        <label htmlFor="description" className="mb-1 block text-sm text-text-muted">Beschreibung</label>
        <Textarea id="description" value={description} onChange={(e) => setDescription(e.target.value)} rows={4} className="mb-4" />

        <label htmlFor="engine" className="mb-1 block text-sm text-text-muted">Engine</label>
        <Input id="engine" value={engine} onChange={(e) => setEngine(e.target.value)} className="mb-4" />

        <label htmlFor="genre" className="mb-1 block text-sm text-text-muted">Genre</label>
        <Input id="genre" value={genre} onChange={(e) => setGenre(e.target.value)} className="mb-4" />

        <label htmlFor="visibility" className="mb-1 block text-sm text-text-muted">Sichtbarkeit</label>
        <Select
          id="visibility"
          value={visibility}
          onChange={(e) => setVisibility(e.target.value as ProjectVisibility)}
          className="mb-4"
        >
          <option value="Public">Public</option>
          <option value="Private">Private</option>
        </Select>

        <Button type="submit" disabled={saving}>{saving ? "Speichere..." : "Speichern"}</Button>
      </form>

      {isOwner && (
        <>
          <hr className="my-8 border-border" />
          <Button type="button" variant="danger" onClick={handleDelete}>
            Projekt löschen
          </Button>
        </>
      )}

      {message && <p className="mt-3 text-sm text-text-muted">{message}</p>}
    </PageContainer>
  );
}
