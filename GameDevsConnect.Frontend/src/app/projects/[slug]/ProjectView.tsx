"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import clsx from "clsx";
import type { ActivityEvent, CurrentUser, Post, Project, ProjectRole, Quest } from "@/lib/types";
import { FollowButton } from "@/app/FollowButton";
import { Badge, Button, Input, PageContainer, Panel, Select, Textarea } from "@/components/ui";

type Tab = "overview" | "team" | "quests" | "posts" | "activity";

const TABS: { id: Tab; label: string }[] = [
  { id: "overview", label: "Overview" },
  { id: "team", label: "Team" },
  { id: "quests", label: "Quests" },
  { id: "posts", label: "Updates" },
  { id: "activity", label: "Activity" },
];

export function ProjectView({
  project,
  me,
  quests,
  activity,
  posts,
}: {
  project: Project;
  me: CurrentUser | null;
  quests: Quest[];
  activity: ActivityEvent[];
  posts: Post[];
}) {
  const router = useRouter();
  const [tab, setTab] = useState<Tab>("overview");
  const [inviteUsername, setInviteUsername] = useState("");
  const [inviteRole, setInviteRole] = useState<ProjectRole>("Contributor");
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [postBody, setPostBody] = useState("");
  const [postFiles, setPostFiles] = useState<FileList | null>(null);
  const [commentDrafts, setCommentDrafts] = useState<Record<string, string>>({});

  const myMembership = me ? project.members.find((m) => m.userId === me.id) : undefined;
  const myRole = myMembership?.role;
  const canManage = myRole === "Owner" || myRole === "Admin";
  const isMember = myMembership !== undefined;

  function refresh() {
    router.refresh();
  }

  async function handleInvite(e: React.FormEvent) {
    e.preventDefault();
    setBusy(true);
    setError(null);
    try {
      const res = await fetch(`/api/projects/${project.slug}/members`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username: inviteUsername, role: inviteRole }),
      });
      if (!res.ok) {
        setError("Einladen fehlgeschlagen (Username korrekt? Schon Mitglied?).");
        return;
      }
      setInviteUsername("");
      refresh();
    } finally {
      setBusy(false);
    }
  }

  async function handleRoleChange(username: string, role: ProjectRole) {
    setBusy(true);
    setError(null);
    try {
      const res = await fetch(`/api/projects/${project.slug}/members/${encodeURIComponent(username)}`, {
        method: "PATCH",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ role }),
      });
      if (!res.ok) setError("Rollenänderung fehlgeschlagen.");
      refresh();
    } finally {
      setBusy(false);
    }
  }

  async function handleRemove(username: string) {
    setBusy(true);
    setError(null);
    try {
      const res = await fetch(`/api/projects/${project.slug}/members/${encodeURIComponent(username)}`, {
        method: "DELETE",
        credentials: "include",
      });
      if (!res.ok) setError("Entfernen fehlgeschlagen.");
      refresh();
    } finally {
      setBusy(false);
    }
  }

  async function handleCancelQuest(questId: string) {
    if (!confirm("Quest wirklich abbrechen?")) return;
    setBusy(true);
    setError(null);
    try {
      const res = await fetch(`/api/projects/${project.slug}/quests/${questId}`, {
        method: "DELETE",
        credentials: "include",
      });
      if (!res.ok) setError("Abbrechen fehlgeschlagen.");
      refresh();
    } finally {
      setBusy(false);
    }
  }

  async function handleCreatePost(e: React.FormEvent) {
    e.preventDefault();
    setBusy(true);
    setError(null);
    try {
      const formData = new FormData();
      formData.append("body", postBody);
      if (postFiles) {
        for (const file of Array.from(postFiles)) {
          formData.append("files", file);
        }
      }
      const res = await fetch(`/api/projects/${project.slug}/posts`, {
        method: "POST",
        credentials: "include",
        body: formData,
      });
      if (!res.ok) {
        setError("Post fehlgeschlagen.");
        return;
      }
      setPostBody("");
      setPostFiles(null);
      refresh();
    } finally {
      setBusy(false);
    }
  }

  async function handleDeletePost(postId: string) {
    if (!confirm("Update wirklich löschen?")) return;
    const res = await fetch(`/api/posts/${postId}`, { method: "DELETE", credentials: "include" });
    if (!res.ok) setError("Löschen fehlgeschlagen.");
    refresh();
  }

  async function handleToggleLike(post: Post) {
    const res = await fetch(`/api/posts/${post.id}/like`, {
      method: post.likedByMe ? "DELETE" : "POST",
      credentials: "include",
    });
    if (!res.ok) setError("Like fehlgeschlagen.");
    refresh();
  }

  async function handleComment(postId: string) {
    const body = commentDrafts[postId]?.trim();
    if (!body) return;
    const res = await fetch(`/api/posts/${postId}/comments`, {
      method: "POST",
      credentials: "include",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ body }),
    });
    if (!res.ok) {
      setError("Kommentar fehlgeschlagen.");
      return;
    }
    setCommentDrafts((prev) => ({ ...prev, [postId]: "" }));
    refresh();
  }

  return (
    <PageContainer>
      {project.bannerUrl && (
        // eslint-disable-next-line @next/next/no-img-element
        <img src={project.bannerUrl} alt="" className="mb-4 h-48 w-full rounded-lg object-cover" />
      )}
      <h1 className="mb-1 font-display text-base text-accent-bright">{project.title}</h1>
      <p className="mb-2 text-sm text-text-muted">
        {project.genre} · {project.engine} · {project.status} · {project.visibility}
      </p>
      <div className="mb-4 flex items-center gap-3">
        {canManage && (
          <Link href={`/projects/${project.slug}/settings`} className="text-accent hover:text-accent-bright">
            Projekt-Einstellungen
          </Link>
        )}
        {me && !isMember && (
          <FollowButton followUrl={`/api/projects/${project.slug}/follow`} initialFollowing={project.isFollowedByMe} />
        )}
      </div>

      <nav className="mb-4 flex gap-1 border-b border-border">
        {TABS.map((t) => (
          <button
            key={t.id}
            type="button"
            onClick={() => setTab(t.id)}
            className={clsx(
              "border-b-2 px-3 py-2 text-sm transition-colors",
              tab === t.id
                ? "border-accent-bright text-accent-bright"
                : "border-transparent text-text-muted hover:text-text",
            )}
          >
            {t.label}
          </button>
        ))}
      </nav>

      {error && <p className="mb-3 text-sm text-danger">{error}</p>}

      {tab === "overview" && (
        <section>
          <p>{project.description}</p>
          {project.tags.length > 0 && (
            <div className="mt-2 flex flex-wrap gap-2">
              {project.tags.map((t) => (
                <Badge key={t}>{t}</Badge>
              ))}
            </div>
          )}
        </section>
      )}

      {tab === "team" && (
        <section>
          <ul className="list-none space-y-2 p-0">
            {project.members.map((member) => (
              <li key={member.userId} className="flex items-center gap-2">
                <Link href={`/users/${member.username}`} className="text-accent hover:text-accent-bright">
                  {member.username}
                </Link>
                <Badge tone="accent">{member.role}</Badge>
                {canManage && member.username !== me?.username && (
                  <>
                    {myRole === "Owner" && (
                      <Select
                        value={member.role}
                        disabled={busy}
                        onChange={(e) => handleRoleChange(member.username, e.target.value as ProjectRole)}
                        className="w-auto py-1"
                      >
                        <option value="Owner">Owner</option>
                        <option value="Admin">Admin</option>
                        <option value="Contributor">Contributor</option>
                      </Select>
                    )}
                    <Button type="button" variant="danger" disabled={busy} onClick={() => handleRemove(member.username)}>
                      Entfernen
                    </Button>
                  </>
                )}
              </li>
            ))}
          </ul>

          {canManage && (
            <form onSubmit={handleInvite} className="mt-4 flex gap-2">
              <Input
                placeholder="Username"
                value={inviteUsername}
                onChange={(e) => setInviteUsername(e.target.value)}
                required
                className="w-auto flex-1"
              />
              <Select value={inviteRole} onChange={(e) => setInviteRole(e.target.value as ProjectRole)} className="w-auto">
                <option value="Contributor">Contributor</option>
                {myRole === "Owner" && <option value="Admin">Admin</option>}
              </Select>
              <Button type="submit" disabled={busy}>Einladen</Button>
            </form>
          )}
        </section>
      )}

      {tab === "quests" && (
        <section>
          {canManage && (
            <p className="mb-3">
              <Link href={`/projects/${project.slug}/quests/new`} className="text-accent hover:text-accent-bright">
                + Neue Quest
              </Link>
            </p>
          )}
          {quests.length === 0 ? (
            <Panel className="text-text-muted">Noch keine Quests.</Panel>
          ) : (
            <ul className="list-none space-y-2 p-0">
              {quests.map((quest) => (
                <li key={quest.id}>
                  <Panel className="flex flex-wrap items-center gap-2">
                    <Link href={`/quests/${quest.id}`} className="font-medium text-text hover:text-accent-bright">
                      {quest.title}
                    </Link>
                    <Badge tone={quest.status === "Open" ? "success" : "neutral"}>{quest.status}</Badge>
                    <Badge tone="accent">{quest.difficulty}</Badge>
                    <span className="text-sm text-text-muted">{quest.xpReward} XP</span>
                    {quest.claimedByUsername && (
                      <span className="text-sm text-text-muted">claimed von {quest.claimedByUsername}</span>
                    )}
                    {canManage && (
                      <span className="ml-auto flex gap-2">
                        {quest.status === "Open" && (
                          <Link href={`/quests/${quest.id}/edit`} className="text-accent hover:text-accent-bright">
                            Bearbeiten
                          </Link>
                        )}
                        {quest.status !== "Cancelled" && (
                          <Button type="button" variant="danger" disabled={busy} onClick={() => handleCancelQuest(quest.id)}>
                            Abbrechen
                          </Button>
                        )}
                      </span>
                    )}
                  </Panel>
                </li>
              ))}
            </ul>
          )}
        </section>
      )}

      {tab === "posts" && (
        <section>
          {isMember && (
            <form onSubmit={handleCreatePost} className="mb-6">
              <Textarea
                placeholder="Was gibt's Neues im Projekt?"
                value={postBody}
                onChange={(e) => setPostBody(e.target.value)}
                required
                rows={3}
                className="mb-2"
              />
              <input
                type="file"
                multiple
                onChange={(e) => setPostFiles(e.target.files)}
                className="mb-2 block w-full text-sm text-text-muted file:mr-3 file:rounded-md file:border file:border-border file:bg-canvas file:px-3 file:py-1.5 file:text-text"
              />
              <Button type="submit" disabled={busy}>Posten</Button>
            </form>
          )}

          {posts.length === 0 ? (
            <Panel className="text-text-muted">Noch keine Updates.</Panel>
          ) : (
            <ul className="list-none space-y-4 p-0">
              {posts.map((post) => (
                <li key={post.id}>
                  <Panel>
                    <p className="m-0 text-sm">
                      <Link href={`/users/${post.authorUsername}`} className="text-accent hover:text-accent-bright">
                        {post.authorUsername}
                      </Link>{" "}
                      <span className="text-text-muted">{new Date(post.createdAt).toLocaleString()}</span>
                    </p>
                    <p>{post.body}</p>

                    {post.attachments.length > 0 && (
                      <ul className="list-disc pl-5 text-sm">
                        {post.attachments.map((a) => (
                          <li key={a.id}>
                            <a
                              href={`/api/posts/${post.id}/attachments/${a.id}`}
                              target="_blank"
                              rel="noopener noreferrer"
                              className="text-accent hover:text-accent-bright"
                            >
                              {a.fileName}
                            </a>
                          </li>
                        ))}
                      </ul>
                    )}

                    <div className="my-2 flex items-center gap-3">
                      {me && (
                        <Button type="button" variant="ghost" onClick={() => handleToggleLike(post)}>
                          {post.likedByMe ? "♥" : "♡"} {post.likeCount}
                        </Button>
                      )}
                      {!me && <span className="text-sm text-text-muted">♥ {post.likeCount}</span>}
                      {(post.authorId === me?.id || canManage) && (
                        <Button type="button" variant="danger" onClick={() => handleDeletePost(post.id)}>
                          Löschen
                        </Button>
                      )}
                    </div>

                    {post.comments.length > 0 && (
                      <ul className="list-none space-y-1 border-t border-border p-0 pt-2">
                        {post.comments.map((c) => (
                          <li key={c.id} className="text-sm">
                            <Link href={`/users/${c.authorUsername}`} className="text-accent hover:text-accent-bright">
                              {c.authorUsername}
                            </Link>
                            : {c.body}
                          </li>
                        ))}
                      </ul>
                    )}

                    {isMember && (
                      <div className="mt-2 flex gap-2">
                        <Input
                          placeholder="Kommentar schreiben..."
                          value={commentDrafts[post.id] ?? ""}
                          onChange={(e) => setCommentDrafts((prev) => ({ ...prev, [post.id]: e.target.value }))}
                          className="flex-1"
                        />
                        <Button type="button" variant="secondary" onClick={() => handleComment(post.id)}>
                          Senden
                        </Button>
                      </div>
                    )}
                  </Panel>
                </li>
              ))}
            </ul>
          )}
        </section>
      )}

      {tab === "activity" && (
        <section>
          {activity.length === 0 ? (
            <Panel className="text-text-muted">Noch keine Aktivität.</Panel>
          ) : (
            <ul className="list-none space-y-2 p-0">
              {activity.map((event) => (
                <li key={event.id} className="border-b border-border py-2">
                  {event.linkUrl ? (
                    <Link href={event.linkUrl} className="text-text hover:text-accent-bright">
                      {event.summary}
                    </Link>
                  ) : (
                    <p className="m-0">{event.summary}</p>
                  )}
                  <p className="m-0 text-xs text-text-muted">{new Date(event.createdAt).toLocaleString()}</p>
                </li>
              ))}
            </ul>
          )}
        </section>
      )}
    </PageContainer>
  );
}
