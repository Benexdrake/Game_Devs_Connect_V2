"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import type { Project, ProjectVisibility } from "@/lib/types";
import { Button, Input, PageContainer, Select, Textarea } from "@/components/ui";

export default function NewProjectPage() {
  const router = useRouter();
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [engine, setEngine] = useState("");
  const [genre, setGenre] = useState("");
  const [visibility, setVisibility] = useState<ProjectVisibility>("Public");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      const res = await fetch("/api/projects", {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          title,
          description: description || null,
          engine: engine || null,
          genre: genre || null,
          visibility,
        }),
      });
      if (!res.ok) {
        setError("Projekt konnte nicht erstellt werden.");
        return;
      }
      const project = (await res.json()) as Project;
      router.push(`/projects/${project.slug}`);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <PageContainer className="max-w-md">
      <h1 className="mb-6 font-display text-sm text-accent-bright">NEUES PROJEKT</h1>
      <form onSubmit={handleSubmit}>
        <label htmlFor="title" className="mb-1 block text-sm text-text-muted">Titel</label>
        <Input id="title" value={title} onChange={(e) => setTitle(e.target.value)} required className="mb-4" />

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

        <Button type="submit" disabled={submitting}>
          {submitting ? "Erstelle..." : "Projekt erstellen"}
        </Button>
        {error && <p className="mt-3 text-sm text-danger">{error}</p>}
      </form>
    </PageContainer>
  );
}
