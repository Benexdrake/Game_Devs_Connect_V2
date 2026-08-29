import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, Project } from "@/lib/types";
import { ProjectView } from "./ProjectView";

export default async function ProjectPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const [project, me] = await Promise.all([
    apiFetchJson<Project>(`/api/projects/${encodeURIComponent(slug)}`),
    apiFetchJson<CurrentUser>("/api/auth/me"),
  ]);

  if (!project) {
    notFound();
  }

  return <ProjectView project={project} me={me} />;
}
