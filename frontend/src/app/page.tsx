import { apiFetchJson } from "@/lib/api";
import type { ActivityEvent, CurrentUser } from "@/lib/types";
import { HomeFeed } from "./HomeFeed";

export default async function Home() {
  const me = await apiFetchJson<CurrentUser>("/api/auth/me");

  if (!me) {
    return (
      <main style={{ maxWidth: 640, margin: "0 auto", padding: "4rem 1rem", textAlign: "center" }}>
        <h1>Gamedevs Connect</h1>
        <p style={{ fontSize: "1.1em", color: "#555" }}>
          Die Plattform, auf der Game Developer gemeinsam Spiele entwickeln - über echte
          Contributions, nicht nur Posts.
        </p>
        <p>
          <a href="/api/auth/login/github" style={{ fontWeight: 600 }}>
            Login with GitHub
          </a>
        </p>
      </main>
    );
  }

  const feed = (await apiFetchJson<ActivityEvent[]>("/api/feed")) ?? [];

  return <HomeFeed me={me} feed={feed} />;
}
