import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { Quest, Skill } from "@/lib/types";
import { EditQuestForm } from "./EditQuestForm";

export default async function EditQuestPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const [quest, skills] = await Promise.all([
    apiFetchJson<Quest>(`/api/quests/${id}`),
    apiFetchJson<Skill[]>("/api/skills"),
  ]);

  if (!quest) {
    notFound();
  }

  return <EditQuestForm quest={quest} skills={skills ?? []} />;
}
