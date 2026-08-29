import { apiFetchJson } from "@/lib/api";
import type { Skill } from "@/lib/types";
import { NewQuestForm } from "./NewQuestForm";

export default async function NewQuestPage({
  params,
}: {
  params: Promise<{ slug: string }>;
}) {
  const { slug } = await params;
  const skills = (await apiFetchJson<Skill[]>("/api/skills")) ?? [];

  return <NewQuestForm projectSlug={slug} skills={skills} />;
}
