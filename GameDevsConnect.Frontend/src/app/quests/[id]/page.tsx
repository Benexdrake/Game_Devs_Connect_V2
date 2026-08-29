import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, Project, ProjectRole, Quest, Submission } from "@/lib/types";
import { QuestDetailView } from "./QuestDetailView";

export default async function QuestDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const [quest, me] = await Promise.all([
    apiFetchJson<Quest>(`/api/quests/${id}`),
    apiFetchJson<CurrentUser>("/api/auth/me"),
  ]);

  if (!quest) {
    notFound();
  }

  const [project, submissions] = await Promise.all([
    apiFetchJson<Project>(`/api/projects/${quest.projectSlug}`),
    apiFetchJson<Submission[]>(`/api/quests/${id}/submissions`),
  ]);

  const myRole: ProjectRole | null =
    (me && project?.members.find((m) => m.userId === me.id)?.role) ?? null;

  return <QuestDetailView quest={quest} me={me} myRole={myRole} submissions={submissions ?? []} />;
}
