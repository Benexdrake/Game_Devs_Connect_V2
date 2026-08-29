"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import type { Project, ProjectVisibility } from "@/lib/types";

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
    <main style={{ maxWidth: 480, margin: "0 auto", padding: "2rem 1rem" }}>
      <h1>Neues Projekt</h1>
      <form onSubmit={handleSubmit}>
        <label htmlFor="title">Titel</label>
        <input id="title" value={title} onChange={(e) => setTitle(e.target.value)} required style={{ display: "block", width: "100%", marginBottom: "1rem" }} />

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

        <button type="submit" disabled={submitting}>
          {submitting ? "Erstelle..." : "Projekt erstellen"}
        </button>
        {error && <p>{error}</p>}
      </form>
    </main>
  );
}
