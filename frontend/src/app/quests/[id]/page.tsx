import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, Quest } from "@/lib/types";
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

  return <QuestDetailView quest={quest} me={me} />;
}
