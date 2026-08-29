import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { ActivityEvent, CurrentUser, Post, Project, Quest } from "@/lib/types";
import { ProjectView } from "./ProjectView";

export default async function ProjectPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const [project, me, quests, activity, posts] = await Promise.all([
    apiFetchJson<Project>(`/api/projects/${encodeURIComponent(slug)}`),
    apiFetchJson<CurrentUser>("/api/auth/me"),
    apiFetchJson<Quest[]>(`/api/projects/${encodeURIComponent(slug)}/quests`),
    apiFetchJson<ActivityEvent[]>(`/api/projects/${encodeURIComponent(slug)}/activity`),
    apiFetchJson<Post[]>(`/api/projects/${encodeURIComponent(slug)}/posts`),
  ]);

  if (!project) {
    notFound();
  }

  return (
    <ProjectView
      project={project}
      me={me}
      quests={quests ?? []}
      activity={activity ?? []}
      posts={posts ?? []}
    />
  );
}
