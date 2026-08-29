"use client";

import { useRef, useState } from "react";
import { useRouter } from "next/navigation";
import clsx from "clsx";
import type { Engine, Genre, GitHubRepo, Project, ProjectStatus, ProjectVisibility } from "@/lib/types";
import { BackLink, Button, Input, MarkdownEditor, PageContainer, Select } from "@/components/ui";

const STATUSES: ProjectStatus[] = ["Concept", "InDevelopment", "Beta", "Released", "Archived"];

export function NewProjectForm({
  engines,
  genres,
  repos,
}: {
  engines: Engine[];
  genres: Genre[];
  repos: GitHubRepo[] | null;
}) {
  const router = useRouter();
  const bannerInputRef = useRef<HTMLInputElement>(null);

  const [repoFullName, setRepoFullName] = useState("");
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [bannerUrl, setBannerUrl] = useState<string | null>(null);
  const [bannerUploading, setBannerUploading] = useState(false);
  const [engineId, setEngineId] = useState("");
  const [genreIds, setGenreIds] = useState<string[]>([]);
  const [status, setStatus] = useState<ProjectStatus>("Concept");
  const [visibility, setVisibility] = useState<ProjectVisibility>("Public");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  function handleRepoChange(fullName: string) {
    setRepoFullName(fullName);
    if (fullName && title.trim() === "") {
      const repo = repos?.find((r) => r.fullName === fullName);
      if (repo) setTitle(repo.name);
    }
  }

  function toggleGenre(id: string) {
    setGenreIds((prev) => (prev.includes(id) ? prev.filter((g) => g !== id) : [...prev, id]));
  }

  async function handleBannerChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    e.target.value = "";
    if (!file) return;
    setBannerUploading(true);
    setError(null);
    try {
      const formData = new FormData();
      formData.append("file", file);
      const res = await fetch("/api/uploads/images", { method: "POST", credentials: "include", body: formData });
      if (!res.ok) {
        setError("Bild-Upload fehlgeschlagen.");
        return;
      }
      const { url } = (await res.json()) as { url: string };
      setBannerUrl(url);
    } finally {
      setBannerUploading(false);
    }
  }

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
          bannerUrl,
          engineId: engineId || null,
          genreIds,
          githubRepoFullName: repoFullName || null,
          status,
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
    <PageContainer>
      <div className="mb-6 flex items-center gap-2">
        <BackLink fallbackHref="/" />
        <h1 className="font-display text-sm text-accent-bright">NEUES PROJEKT</h1>
      </div>
      <form onSubmit={handleSubmit}>
        {/* Simple fields stay at a normal form width even though the page itself is
            wide - only the banner and description editor below benefit from the
            extra room. */}
        <div className="mx-auto max-w-3xl">
          <div className="mb-4 grid grid-cols-1 gap-4 sm:grid-cols-2">
            <div>
              <label htmlFor="repo" className="mb-1 block text-sm text-text-muted">GitHub-Repo</label>
              <Select
                id="repo"
                value={repoFullName}
                onChange={(e) => handleRepoChange(e.target.value)}
                disabled={!repos || repos.length === 0}
              >
                <option value="">Kein Repo verknüpfen</option>
                {(repos ?? []).map((repo) => (
                  <option key={repo.fullName} value={repo.fullName}>
                    {repo.fullName}
                  </option>
                ))}
              </Select>
              {repos === null && (
                <p className="mt-1 text-xs text-text-muted">
                  GitHub nicht verbunden.{" "}
                  <a href="/api/auth/login/github" className="text-accent hover:text-accent-bright">
                    GitHub erneut verbinden
                  </a>
                  , um ein Repo auszuwählen.
                </p>
              )}
            </div>

            <div>
              <label htmlFor="title" className="mb-1 block text-sm text-text-muted">Titel</label>
              <Input id="title" value={title} onChange={(e) => setTitle(e.target.value)} required />
            </div>
          </div>

          <div className="mb-4 grid grid-cols-1 gap-4 sm:grid-cols-3">
            <div>
              <label htmlFor="visibility" className="mb-1 block text-sm text-text-muted">Sichtbarkeit</label>
              <Select id="visibility" value={visibility} onChange={(e) => setVisibility(e.target.value as ProjectVisibility)}>
                <option value="Public">Public</option>
                <option value="Private">Private</option>
              </Select>
            </div>

            <div>
              <label htmlFor="status" className="mb-1 block text-sm text-text-muted">Status</label>
              <Select id="status" value={status} onChange={(e) => setStatus(e.target.value as ProjectStatus)}>
                {STATUSES.map((s) => (
                  <option key={s} value={s}>{s}</option>
                ))}
              </Select>
            </div>

            <div>
              <label htmlFor="engine" className="mb-1 block text-sm text-text-muted">Engine</label>
              <Select id="engine" value={engineId} onChange={(e) => setEngineId(e.target.value)}>
                <option value="">Keine Auswahl</option>
                {engines.map((engine) => (
                  <option key={engine.id} value={engine.id}>
                    {engine.name}
                  </option>
                ))}
              </Select>
            </div>
          </div>
        </div>

        {/* Genre uses the full page width (not the max-w-3xl block above) - it's a
            wrapping chip list, so it benefits from fitting more per row. */}
        <div className="mb-4">
          <label className="mb-1 block text-sm text-text-muted">Genre</label>
          <div className="flex flex-wrap gap-2">
            {genres.map((genre) => (
              <Button
                key={genre.id}
                type="button"
                variant={genreIds.includes(genre.id) ? "primary" : "secondary"}
                onClick={() => toggleGenre(genre.id)}
                className={clsx(genreIds.includes(genre.id) && "border-accent-bright")}
              >
                {genre.name}
              </Button>
            ))}
          </div>
        </div>

        <label className="mb-1 block text-sm text-text-muted">Headerbild</label>
        <div className="relative mb-4 h-40 w-full overflow-hidden rounded-md border border-border bg-canvas">
          {bannerUrl ? (
            // eslint-disable-next-line @next/next/no-img-element
            <img src={bannerUrl} alt="" className="h-full w-full object-cover" />
          ) : (
            <div className="flex h-full items-center justify-center text-sm text-text-muted">Kein Headerbild</div>
          )}
          <button
            type="button"
            onClick={() => bannerInputRef.current?.click()}
            disabled={bannerUploading}
            className="absolute inset-x-0 bottom-0 bg-surface/80 px-3 py-1.5 text-left text-xs text-text hover:text-accent-bright disabled:cursor-not-allowed"
          >
            {bannerUploading ? "Lädt hoch..." : bannerUrl ? "Bild ändern" : "Bild hochladen"}
          </button>
        </div>
        <input ref={bannerInputRef} type="file" accept="image/*" onChange={handleBannerChange} className="hidden" />

        <label className="mb-1 block text-sm text-text-muted">Beschreibung</label>
        <MarkdownEditor value={description} onChange={setDescription} maxLength={5000} maxUploads={10} className="mb-4" />

        <Button type="submit" disabled={submitting}>
          {submitting ? "Erstelle..." : "Projekt erstellen"}
        </Button>
        {error && <p className="mt-3 text-sm text-danger">{error}</p>}
      </form>
    </PageContainer>
  );
}
