import { apiFetchJson } from "@/lib/api";
import type { Engine, Genre, GitHubRepo } from "@/lib/types";
import { NewProjectForm } from "./NewProjectForm";

export default async function NewProjectPage() {
  const [engines, genres, repos] = await Promise.all([
    apiFetchJson<Engine[]>("/api/engines"),
    apiFetchJson<Genre[]>("/api/genres"),
    apiFetchJson<GitHubRepo[]>("/api/github/repos"),
  ]);

  return <NewProjectForm engines={engines ?? []} genres={genres ?? []} repos={repos} />;
}
