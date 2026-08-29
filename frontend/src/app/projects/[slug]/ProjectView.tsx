"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import type { CurrentUser, Project, ProjectRole, Quest } from "@/lib/types";

type Tab = "overview" | "team" | "quests" | "activity";

export function ProjectView({ project, me, quests }: { project: Project; me: CurrentUser | null; quests: Quest[] }) {
  const router = useRouter();
  const [tab, setTab] = useState<Tab>("overview");
  const [inviteUsername, setInviteUsername] = useState("");
  const [inviteRole, setInviteRole] = useState<ProjectRole>("Contributor");
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const myMembership = me ? project.members.find((m) => m.userId === me.id) : undefined;
  const myRole = myMembership?.role;
  const canManage = myRole === "Owner" || myRole === "Admin";

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

      <nav style={{ display: "flex", gap: "1rem", margin: "1rem 0", borderBottom: "1px solid #ccc" }}>
        <button type="button" onClick={() => setTab("overview")}>Overview</button>
        <button type="button" onClick={() => setTab("team")}>Team</button>
        <button type="button" onClick={() => setTab("quests")}>Quests</button>
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
      {tab === "activity" && <p>Activity kommt in Phase 5.</p>}
    </main>
  );
}
