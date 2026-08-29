import Link from "next/link";
import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, UserProfile, XpSummary } from "@/lib/types";
import { FollowButton } from "@/app/FollowButton";

export default async function UserProfilePage({
  params,
}: {
  params: Promise<{ username: string }>;
}) {
  const { username } = await params;
  const [profile, me, xp] = await Promise.all([
    apiFetchJson<UserProfile>(`/api/users/${encodeURIComponent(username)}`),
    apiFetchJson<CurrentUser>("/api/auth/me"),
    apiFetchJson<XpSummary>(`/api/users/${encodeURIComponent(username)}/xp-summary`),
  ]);

  if (!profile) {
    notFound();
  }

  const isOwnProfile = me?.id === profile.id;

  const levelSpan = xp ? xp.xpForNextLevel - xp.xpForCurrentLevel : 0;
  const levelProgress = xp && levelSpan > 0 ? (xp.totalXp - xp.xpForCurrentLevel) / levelSpan : 0;

  return (
    <main style={{ maxWidth: 720, margin: "0 auto", padding: "2rem 1rem" }}>
      <div style={{ display: "flex", alignItems: "center", gap: "1rem" }}>
        {profile.avatarUrl && (
          // eslint-disable-next-line @next/next/no-img-element
          <img
            src={profile.avatarUrl}
            alt={profile.username}
            width={80}
            height={80}
            style={{ borderRadius: "50%" }}
          />
        )}
        <div>
          <h1 style={{ margin: 0 }}>{profile.username}</h1>
          {isOwnProfile && <Link href="/settings/profile">Profil bearbeiten</Link>}
          {!isOwnProfile && me && (
            <FollowButton
              followUrl={`/api/users/${encodeURIComponent(profile.username)}/follow`}
              initialFollowing={profile.isFollowedByMe}
            />
          )}
        </div>
      </div>

      {xp && (
        <section style={{ margin: "1rem 0" }}>
          <p style={{ margin: 0 }}>
            <strong>Level {xp.level}</strong> · {xp.totalXp} XP ·{" "}
            {xp.reputation === null ? "Reputation: noch keine Daten" : `Reputation: ${xp.reputation} / 5`}
          </p>
          <div style={{ background: "#eee", borderRadius: 4, height: 8, marginTop: "0.25rem", overflow: "hidden" }}>
            <div
              style={{
                background: "#4a90d9",
                height: "100%",
                width: `${Math.round(Math.min(1, Math.max(0, levelProgress)) * 100)}%`,
              }}
            />
          </div>
          <p style={{ margin: "0.25rem 0 0", color: "#666", fontSize: "0.9em" }}>
            {xp.totalXp} / {xp.xpForNextLevel} XP bis Level {xp.level + 1} · {xp.completedQuests} Completed Quests ·{" "}
            {xp.acceptedContributions} Accepted Contributions
          </p>
        </section>
      )}

      {profile.bio && <p>{profile.bio}</p>}

      {profile.links.length > 0 && (
        <section>
          <h2>Links</h2>
          <ul>
            {profile.links.map((link) => (
              <li key={link.url}>
                <a href={link.url} target="_blank" rel="noopener noreferrer">
                  {link.label}
                </a>
              </li>
            ))}
          </ul>
        </section>
      )}

      <section>
        <h2>Skills</h2>
        {profile.skills.length === 0 ? (
          <p>Noch keine Skills angegeben.</p>
        ) : (
          <ul style={{ display: "flex", flexWrap: "wrap", gap: "0.5rem", listStyle: "none", padding: 0 }}>
            {profile.skills.map((skill) => (
              <li key={skill.id} style={{ border: "1px solid #ccc", borderRadius: 4, padding: "0.25rem 0.5rem" }}>
                {skill.name}
              </li>
            ))}
          </ul>
        )}
      </section>

      <section>
        <h2>Projects</h2>
        {profile.projects.length === 0 ? (
          <p>Noch an keinem Projekt beteiligt.</p>
        ) : (
          <ul>
            {profile.projects.map((project) => (
              <li key={project.slug}>
                <Link href={`/projects/${project.slug}`}>{project.title}</Link> ({project.status})
              </li>
            ))}
          </ul>
        )}
      </section>

      <section>
        <h2>Contributions</h2>
        {profile.contributions.length === 0 ? (
          <p>Noch keine Contributions.</p>
        ) : (
          <ul>
            {profile.contributions.map((c) => (
              <li key={c.id}>
                <Link href={`/quests/${c.questId}`}>{c.questTitle}</Link>
                {" — "}
                <Link href={`/projects/${c.projectSlug}`}>{c.projectTitle}</Link>
                {" · "}
                {new Date(c.createdAt).toLocaleDateString()}
              </li>
            ))}
          </ul>
        )}
      </section>
    </main>
  );
}
