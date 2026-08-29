import { redirect } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { ActivityEvent, CurrentUser } from "@/lib/types";
import { HomeFeed } from "./HomeFeed";

export default async function Home() {
  const me = await apiFetchJson<CurrentUser>("/api/auth/me");

  if (!me) {
    redirect("/login");
  }

  const [forYou, following] = await Promise.all([
    apiFetchJson<ActivityEvent[]>("/api/feed/for-you"),
    apiFetchJson<ActivityEvent[]>("/api/feed"),
  ]);

  return <HomeFeed forYou={forYou ?? []} following={following ?? []} />;
}
