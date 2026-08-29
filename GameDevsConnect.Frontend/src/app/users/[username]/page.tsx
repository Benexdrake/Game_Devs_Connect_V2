import Link from "next/link";
import { notFound } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser, UserProfile, XpSummary } from "@/lib/types";
import { FollowButton } from "@/app/FollowButton";
import { Badge, Button, MarkdownContent, PageContainer, Panel } from "@/components/ui";
import { LINK_PLATFORM_ICONS, LINK_PLATFORM_LABELS } from "@/lib/linkPlatforms";

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
    <PageContainer>
      <div className="flex items-start justify-between gap-4">
        <div className="flex items-center gap-4">
          {profile.avatarUrl && (
            // eslint-disable-next-line @next/next/no-img-element
            <img
              src={profile.avatarUrl}
              alt={profile.username}
              width={80}
              height={80}
              className="rounded-full border-2 border-border-strong"
            />
          )}
          <h1 className="m-0 font-display text-base text-accent-bright">{profile.username}</h1>
        </div>
        <div className="flex shrink-0 gap-2">
          {isOwnProfile && (
            <>
              <Link href="/settings/profile">
                <Button type="button" variant="secondary">
                  Profil bearbeiten
                </Button>
              </Link>
              <Link href="/projects/new">
                <Button type="button">+ Neues Projekt</Button>
              </Link>
            </>
          )}
          {!isOwnProfile && me && (
            <FollowButton
              followUrl={`/api/users/${encodeURIComponent(profile.username)}/follow`}
              initialFollowing={profile.isFollowedByMe}
            />
          )}
        </div>
      </div>

      {xp && (
        <Panel className="my-4">
          <p className="m-0">
            <span className="font-display text-xs text-accent-bright">LVL {xp.level}</span>{" "}
            <span className="text-text-muted">
              · {xp.totalXp} XP ·{" "}
              {xp.reputation === null ? "Reputation: noch keine Daten" : `Reputation: ${xp.reputation} / 5`}
            </span>
          </p>
          <div className="mt-2 h-3 overflow-hidden rounded-full border border-border bg-canvas">
            <div
              className="h-full bg-gradient-to-r from-accent to-accent-bright"
              style={{ width: `${Math.round(Math.min(1, Math.max(0, levelProgress)) * 100)}%` }}
            />
          </div>
          <p className="m-0 mt-1 text-xs text-text-muted">
            {xp.totalXp} / {xp.xpForNextLevel} XP bis Level {xp.level + 1} · {xp.completedQuests} Completed Quests ·{" "}
            {xp.acceptedContributions} Accepted Contributions
          </p>
        </Panel>
      )}

      {profile.bio && <MarkdownContent>{profile.bio}</MarkdownContent>}

      {profile.links.length > 0 && (
        <section className="mb-6">
          <h2 className="mb-2 font-display text-xs text-accent-bright">LINKS</h2>
          <ul className="flex list-none flex-wrap gap-2 p-0">
            {profile.links.map((link) => {
              const Icon = LINK_PLATFORM_ICONS[link.platform];
              const label = link.platform === "Other" ? (link.label ?? "") : LINK_PLATFORM_LABELS[link.platform];
              return (
                <li key={link.url}>
                  <a
                    href={link.url}
                    target="_blank"
                    rel="noopener noreferrer"
                    title={label}
                    className="flex h-12 w-12 items-center justify-center rounded-md border-2 border-border text-text-muted transition-colors hover:border-accent hover:text-accent-bright"
                  >
                    <Icon size={22} />
                  </a>
                </li>
              );
            })}
          </ul>
        </section>
      )}

      <section className="mb-6">
        <h2 className="mb-2 font-display text-xs text-accent-bright">SKILLS</h2>
        {profile.skills.length === 0 ? (
          <p className="text-text-muted">Noch keine Skills angegeben.</p>
        ) : (
          <div className="flex flex-wrap gap-2">
            {profile.skills.map((skill) => (
              <Badge key={skill.id}>{skill.name}</Badge>
            ))}
          </div>
        )}
      </section>

      <section className="mb-6">
        <h2 className="mb-2 font-display text-xs text-accent-bright">PROJECTS</h2>
        {profile.projects.length === 0 ? (
          <p className="text-text-muted">Noch an keinem Projekt beteiligt.</p>
        ) : (
          <ul className="list-none space-y-1 p-0">
            {profile.projects.map((project) => (
              <li key={project.slug}>
                <Link href={`/projects/${project.slug}`} className="text-accent hover:text-accent-bright">
                  {project.title}
                </Link>{" "}
                <span className="text-text-muted">({project.status})</span>
              </li>
            ))}
          </ul>
        )}
      </section>

      <section>
        <h2 className="mb-2 font-display text-xs text-accent-bright">CONTRIBUTIONS</h2>
        {profile.contributions.length === 0 ? (
          <p className="text-text-muted">Noch keine Contributions.</p>
        ) : (
          <ul className="list-none space-y-1 p-0">
            {profile.contributions.map((c) => (
              <li key={c.id} className="text-sm">
                <Link href={`/quests/${c.questId}`} className="text-accent hover:text-accent-bright">
                  {c.questTitle}
                </Link>
                {" — "}
                <Link href={`/projects/${c.projectSlug}`} className="text-accent hover:text-accent-bright">
                  {c.projectTitle}
                </Link>
                <span className="text-text-muted"> · {new Date(c.createdAt).toLocaleDateString()}</span>
              </li>
            ))}
          </ul>
        )}
      </section>
    </PageContainer>
  );
}
