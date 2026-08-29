import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, Project } from "@/lib/types";
import { ProjectSettingsForm } from "./ProjectSettingsForm";

export default async function ProjectSettingsPage({
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

  const myRole = me ? project.members.find((m) => m.userId === me.id)?.role : undefined;
  if (myRole !== "Owner" && myRole !== "Admin") {
    notFound();
  }

  return <ProjectSettingsForm project={project} isOwner={myRole === "Owner"} />;
}
