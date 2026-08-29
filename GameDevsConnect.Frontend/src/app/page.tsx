import { apiFetchJson } from "@/lib/api";
import type { ActivityEvent, CurrentUser } from "@/lib/types";
import { HomeFeed } from "./HomeFeed";

export default async function Home() {
  const me = await apiFetchJson<CurrentUser>("/api/auth/me");

  if (!me) {
    return (
      <main className="mx-auto flex max-w-xl flex-col items-center gap-6 px-4 py-24 text-center">
        <h1 className="font-display text-lg leading-relaxed text-accent-bright sm:text-2xl">
          GAMEDEVS
          <br />
          CONNECT
        </h1>
        <p className="text-lg text-text-muted">
          Die Plattform, auf der Game Developer gemeinsam Spiele entwickeln – über echte
          Contributions, nicht nur Posts.
        </p>
        <a
          href="/api/auth/login/github"
          className="rounded-md border-2 border-accent bg-accent px-5 py-2.5 font-medium text-surface transition-colors hover:border-accent-bright hover:bg-accent-bright"
        >
          Login with GitHub
        </a>
      </main>
    );
  }

  const feed = (await apiFetchJson<ActivityEvent[]>("/api/feed")) ?? [];

  return <HomeFeed me={me} feed={feed} />;
}
