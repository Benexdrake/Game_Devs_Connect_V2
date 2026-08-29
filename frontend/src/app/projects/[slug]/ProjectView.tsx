"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import type { ActivityEvent, CurrentUser, Post, Project, ProjectRole, Quest } from "@/lib/types";
import { FollowButton } from "@/app/FollowButton";

type Tab = "overview" | "team" | "quests" | "posts" | "activity";

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
    <main style={{ maxWidth: 720, margin: "0 auto", padding: "2rem 1rem" }}>
      {project.bannerUrl && (
        // eslint-disable-next-line @next/next/no-img-element
        <img src={project.bannerUrl} alt="" style={{ width: "100%", maxHeight: 200, objectFit: "cover" }} />
      )}
      <h1>{project.title}</h1>
      <p>
        {project.genre} · {project.engine} · {project.status} · {project.visibility}
      </p>
      {canManage && <Link href={`/projects/${project.slug}/settings`}>Projekt-Einstellungen</Link>}
      {me && !isMember && (
        <span style={{ marginLeft: "0.5rem" }}>
          <FollowButton
            followUrl={`/api/projects/${project.slug}/follow`}
            initialFollowing={project.isFollowedByMe}
          />
        </span>
      )}

      <nav style={{ display: "flex", gap: "1rem", margin: "1rem 0", borderBottom: "1px solid #ccc" }}>
        <button type="button" onClick={() => setTab("overview")}>Overview</button>
        <button type="button" onClick={() => setTab("team")}>Team</button>
        <button type="button" onClick={() => setTab("quests")}>Quests</button>
        <button type="button" onClick={() => setTab("posts")}>Updates</button>
        <button type="button" onClick={() => setTab("activity")}>Activity</button>
      </nav>

      {error && <p style={{ color: "red" }}>{error}</p>}

      {tab === "overview" && (
        <section>
          <p>{project.description}</p>
          {project.tags.length > 0 && <p>Tags: {project.tags.join(", ")}</p>}
        </section>
      )}

      {tab === "team" && (
        <section>
          <ul>
            {project.members.map((member) => (
              <li key={member.userId} style={{ marginBottom: "0.5rem" }}>
                <Link href={`/users/${member.username}`}>{member.username}</Link> — {member.role}
                {canManage && member.username !== me?.username && (
                  <>
                    {myRole === "Owner" && (
                      <select
                        value={member.role}
                        disabled={busy}
                        onChange={(e) => handleRoleChange(member.username, e.target.value as ProjectRole)}
                        style={{ marginLeft: "0.5rem" }}
                      >
                        <option value="Owner">Owner</option>
                        <option value="Admin">Admin</option>
                        <option value="Contributor">Contributor</option>
                      </select>
                    )}
                    <button type="button" disabled={busy} onClick={() => handleRemove(member.username)} style={{ marginLeft: "0.5rem" }}>
                      Entfernen
                    </button>
                  </>
                )}
              </li>
            ))}
          </ul>

          {canManage && (
            <form onSubmit={handleInvite} style={{ display: "flex", gap: "0.5rem", marginTop: "1rem" }}>
              <input
                placeholder="Username"
                value={inviteUsername}
                onChange={(e) => setInviteUsername(e.target.value)}
                required
              />
              <select value={inviteRole} onChange={(e) => setInviteRole(e.target.value as ProjectRole)}>
                <option value="Contributor">Contributor</option>
                {myRole === "Owner" && <option value="Admin">Admin</option>}
              </select>
              <button type="submit" disabled={busy}>Einladen</button>
            </form>
          )}
        </section>
      )}

      {tab === "quests" && (
        <section>
          {canManage && (
            <p>
              <Link href={`/projects/${project.slug}/quests/new`}>+ Neue Quest</Link>
            </p>
          )}
          {quests.length === 0 ? (
            <p>Noch keine Quests.</p>
          ) : (
            <ul style={{ listStyle: "none", padding: 0 }}>
              {quests.map((quest) => (
                <li key={quest.id} style={{ border: "1px solid #ccc", borderRadius: 8, padding: "0.75rem", marginBottom: "0.5rem" }}>
                  <Link href={`/quests/${quest.id}`} style={{ fontWeight: 600 }}>{quest.title}</Link>
                  {" — "}
                  {quest.status} · {quest.difficulty} · {quest.xpReward} XP
                  {canManage && (
                    <>
                      {quest.status === "Open" && (
                        <Link href={`/quests/${quest.id}/edit`} style={{ marginLeft: "0.5rem" }}>
                          Bearbeiten
                        </Link>
                      )}
                      {quest.status !== "Cancelled" && (
                        <button
                          type="button"
                          disabled={busy}
                          onClick={() => handleCancelQuest(quest.id)}
                          style={{ marginLeft: "0.5rem" }}
                        >
                          Abbrechen
                        </button>
                      )}
                    </>
                  )}
                </li>
              ))}
            </ul>
          )}
        </section>
      )}
      {tab === "posts" && (
        <section>
          {isMember && (
            <form onSubmit={handleCreatePost} style={{ marginBottom: "1.5rem" }}>
              <textarea
                placeholder="Was gibt's Neues im Projekt?"
                value={postBody}
                onChange={(e) => setPostBody(e.target.value)}
                required
                rows={3}
                style={{ display: "block", width: "100%", marginBottom: "0.5rem" }}
              />
              <input type="file" multiple onChange={(e) => setPostFiles(e.target.files)} style={{ marginBottom: "0.5rem" }} />
              <button type="submit" disabled={busy}>Posten</button>
            </form>
          )}

          {posts.length === 0 ? (
            <p>Noch keine Updates.</p>
          ) : (
            <ul style={{ listStyle: "none", padding: 0 }}>
              {posts.map((post) => (
                <li key={post.id} style={{ border: "1px solid #ccc", borderRadius: 8, padding: "1rem", marginBottom: "1rem" }}>
                  <p style={{ margin: 0 }}>
                    <Link href={`/users/${post.authorUsername}`}>{post.authorUsername}</Link>
                    {" · "}
                    <span style={{ color: "#888", fontSize: "0.85em" }}>{new Date(post.createdAt).toLocaleString()}</span>
                  </p>
                  <p>{post.body}</p>

                  {post.attachments.length > 0 && (
                    <ul>
                      {post.attachments.map((a) => (
                        <li key={a.id}>
                          <a href={`/api/posts/${post.id}/attachments/${a.id}`} target="_blank" rel="noopener noreferrer">
                            {a.fileName}
                          </a>
                        </li>
                      ))}
                    </ul>
                  )}

                  <div style={{ display: "flex", gap: "0.5rem", alignItems: "center", margin: "0.5rem 0" }}>
                    {me && (
                      <button type="button" onClick={() => handleToggleLike(post)}>
                        {post.likedByMe ? "♥" : "♡"} {post.likeCount}
                      </button>
                    )}
                    {!me && <span>♥ {post.likeCount}</span>}
                    {(post.authorId === me?.id || canManage) && (
                      <button type="button" onClick={() => handleDeletePost(post.id)} style={{ color: "red" }}>
                        Löschen
                      </button>
                    )}
                  </div>

                  {post.comments.length > 0 && (
                    <ul style={{ listStyle: "none", padding: 0, marginLeft: "1rem" }}>
                      {post.comments.map((c) => (
                        <li key={c.id} style={{ marginBottom: "0.25rem" }}>
                          <Link href={`/users/${c.authorUsername}`}>{c.authorUsername}</Link>: {c.body}
                        </li>
                      ))}
                    </ul>
                  )}

                  {isMember && (
                    <div style={{ display: "flex", gap: "0.5rem", marginTop: "0.5rem" }}>
                      <input
                        placeholder="Kommentar schreiben..."
                        value={commentDrafts[post.id] ?? ""}
                        onChange={(e) => setCommentDrafts((prev) => ({ ...prev, [post.id]: e.target.value }))}
                        style={{ flex: 1 }}
                      />
                      <button type="button" onClick={() => handleComment(post.id)}>Senden</button>
                    </div>
                  )}
                </li>
              ))}
            </ul>
          )}
        </section>
      )}

      {tab === "activity" && (
        <section>
          {activity.length === 0 ? (
            <p>Noch keine Aktivität.</p>
          ) : (
            <ul style={{ listStyle: "none", padding: 0 }}>
              {activity.map((event) => (
                <li key={event.id} style={{ borderBottom: "1px solid #eee", padding: "0.5rem 0" }}>
                  {event.linkUrl ? (
                    <Link href={event.linkUrl} style={{ margin: 0 }}>
                      {event.summary}
                    </Link>
                  ) : (
                    <p style={{ margin: 0 }}>{event.summary}</p>
                  )}
                  <p style={{ margin: 0, fontSize: "0.8em", color: "#888" }}>{new Date(event.createdAt).toLocaleString()}</p>
                </li>
              ))}
            </ul>
          )}
        </section>
      )}
    </main>
  );
}
