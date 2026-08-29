import Link from "next/link";
import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, UserProfile } from "@/lib/types";

export default async function UserProfilePage({
  params,
}: {
  params: Promise<{ username: string }>;
}) {
  const { username } = await params;
  const [profile, me] = await Promise.all([
    apiFetchJson<UserProfile>(`/api/users/${encodeURIComponent(username)}`),
    apiFetchJson<CurrentUser>("/api/auth/me"),
  ]);

  if (!profile) {
    notFound();
  }

  const isOwnProfile = me?.id === profile.id;

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
        </div>
      </div>

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
        <p>Noch keine Contributions (kommt in Phase 3).</p>
      </section>
    </main>
  );
}
