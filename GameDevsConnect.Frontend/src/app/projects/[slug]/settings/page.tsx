import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, Engine, Genre, GitHubRepo, Project } from "@/lib/types";
import { ProjectSettingsForm } from "./ProjectSettingsForm";

export default async function ProjectSettingsPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const [project, me, engines, genres, repos] = await Promise.all([
    apiFetchJson<Project>(`/api/projects/${encodeURIComponent(slug)}`),
    apiFetchJson<CurrentUser>("/api/auth/me"),
    apiFetchJson<Engine[]>("/api/engines"),
    apiFetchJson<Genre[]>("/api/genres"),
    apiFetchJson<GitHubRepo[]>("/api/github/repos"),
  ]);

  if (!project) {
    notFound();
  }

  const myRole = me ? project.members.find((m) => m.userId === me.id)?.role : undefined;
  if (myRole !== "Owner" && myRole !== "Admin") {
    notFound();
  }

  return (
    <ProjectSettingsForm
      project={project}
      isOwner={myRole === "Owner"}
      engines={engines ?? []}
      genres={genres ?? []}
      repos={repos}
    />
  );
}
