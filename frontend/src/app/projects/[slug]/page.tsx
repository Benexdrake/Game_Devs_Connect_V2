import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, Project, Quest } from "@/lib/types";
import { ProjectView } from "./ProjectView";

export default async function ProjectPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const [project, me, quests] = await Promise.all([
    apiFetchJson<Project>(`/api/projects/${encodeURIComponent(slug)}`),
    apiFetchJson<CurrentUser>("/api/auth/me"),
    apiFetchJson<Quest[]>(`/api/projects/${encodeURIComponent(slug)}/quests`),
  ]);

  if (!project) {
    notFound();
  }

  return <ProjectView project={project} me={me} quests={quests ?? []} />;
}
