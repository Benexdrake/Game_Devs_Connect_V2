"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import type { CurrentUser, ProjectRole, Quest, Submission, SubmissionDecision } from "@/lib/types";

type LinkInput = { url: string; label: string };

export function QuestDetailView({
  quest,
  me,
  myRole,
  submissions,
}: {
  quest: Quest;
  me: CurrentUser | null;
  myRole: ProjectRole | null;
  submissions: Submission[];
}) {
  const router = useRouter();
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const [description, setDescription] = useState("");
  const [links, setLinks] = useState<LinkInput[]>([{ url: "", label: "" }]);
  const [files, setFiles] = useState<FileList | null>(null);
  const [reviewComment, setReviewComment] = useState("");

  const isCreator = me?.id === quest.creatorId;
  const isActiveClaimer = me !== null && me.id === quest.claimedByUserId;
  const canManage = myRole === "Owner" || myRole === "Admin";
  const canClaim = me !== null && !isCreator && quest.status === "Open";
  const canRelease = isActiveClaimer && quest.status === "InProgress";
  const canSubmit = isActiveClaimer && quest.status === "InProgress";
  const pendingSubmission = submissions.find((s) => s.status === "PendingReview");

  function refresh() {
    router.refresh();
  }

  async function handleClaim() {
    setBusy(true);
    setError(null);
    try {
      const res = await fetch(`/api/quests/${quest.id}/claim`, { method: "POST", credentials: "include" });
      if (!res.ok) {
        setError("Claim fehlgeschlagen (evtl. schon vergeben).");
        return;
      }
      refresh();
    } finally {
      setBusy(false);
    }
  }

  async function handleRelease() {
    setBusy(true);
    setError(null);
    try {
      const res = await fetch(`/api/quests/${quest.id}/release`, { method: "POST", credentials: "include" });
      if (!res.ok) {
        setError("Freigeben fehlgeschlagen.");
        return;
      }
      refresh();
    } finally {
      setBusy(false);
    }
  }

  function updateLink(index: number, field: keyof LinkInput, value: string) {
    setLinks((prev) => prev.map((l, i) => (i === index ? { ...l, [field]: value } : l)));
  }

  function addLinkRow() {
    setLinks((prev) => [...prev, { url: "", label: "" }]);
  }

  function removeLinkRow(index: number) {
    setLinks((prev) => prev.filter((_, i) => i !== index));
  }

  async function handleSubmitSubmission(e: React.FormEvent) {
    e.preventDefault();
    setBusy(true);
    setError(null);
    try {
      const cleanLinks = links
        .filter((l) => l.url.trim().length > 0)
        .map((l) => ({ url: l.url.trim(), label: l.label.trim() || null }));

      const res = await fetch(`/api/quests/${quest.id}/submissions`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ description, links: cleanLinks }),
      });
      if (!res.ok) {
        setError("Einreichen fehlgeschlagen.");
        return;
      }
      const submission: Submission = await res.json();

      if (files && files.length > 0) {
        const formData = new FormData();
        for (const file of Array.from(files)) {
          formData.append("files", file);
        }
        const uploadRes = await fetch(`/api/submissions/${submission.id}/files`, {
          method: "POST",
          credentials: "include",
          body: formData,
        });
        if (!uploadRes.ok) {
          setError("Submission wurde erstellt, aber der Datei-Upload ist fehlgeschlagen.");
          refresh();
          return;
        }
      }

      setDescription("");
      setLinks([{ url: "", label: "" }]);
      setFiles(null);
      refresh();
    } finally {
      setBusy(false);
    }
  }

  async function handleReview(submissionId: string, decision: SubmissionDecision) {
    setBusy(true);
    setError(null);
    try {
      const res = await fetch(`/api/submissions/${submissionId}/review`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ decision, comment: reviewComment || null }),
      });
      if (!res.ok) {
        setError("Review fehlgeschlagen.");
        return;
      }
      setReviewComment("");
      refresh();
    } finally {
      setBusy(false);
    }
  }

  return (
    <main style={{ maxWidth: 640, margin: "0 auto", padding: "2rem 1rem" }}>
      <p>
        <Link href={`/projects/${quest.projectSlug}`}>← {quest.projectTitle}</Link>
      </p>
      <h1>{quest.title}</h1>
      <p>
        {quest.category} · {quest.difficulty} · {quest.xpReward} XP · {quest.status}
      </p>
      {quest.deadline && <p>Deadline: {new Date(quest.deadline).toLocaleDateString()}</p>}
      {quest.requiredSkills.length > 0 && (
        <p>Benötigte Skills: {quest.requiredSkills.map((s) => s.name).join(", ")}</p>
      )}
      <p>{quest.description}</p>
      <p>
        Erstellt von <Link href={`/users/${quest.creatorUsername}`}>{quest.creatorUsername}</Link>
      </p>

      {error && <p style={{ color: "red" }}>{error}</p>}

      {canClaim && (
        <button type="button" disabled={busy} onClick={handleClaim}>
          Quest claimen
        </button>
      )}
      {isCreator && quest.status === "Open" && (
        <Link href={`/quests/${quest.id}/edit`} style={{ marginLeft: "0.5rem" }}>
          Bearbeiten
        </Link>
      )}
      {canRelease && (
        <button type="button" disabled={busy} onClick={handleRelease} style={{ marginLeft: "0.5rem" }}>
          Freigeben
        </button>
      )}

      {canSubmit && (
        <section style={{ marginTop: "2rem", borderTop: "1px solid #ccc", paddingTop: "1rem" }}>
          <h2>Submission einreichen</h2>
          <form onSubmit={handleSubmitSubmission}>
            <label htmlFor="submission-description">Beschreibung</label>
            <textarea
              id="submission-description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              required
              rows={4}
              style={{ display: "block", width: "100%", marginBottom: "1rem" }}
            />

            <fieldset style={{ marginBottom: "1rem" }}>
              <legend>Links</legend>
              {links.map((link, i) => (
                <div key={i} style={{ display: "flex", gap: "0.5rem", marginBottom: "0.5rem" }}>
                  <input
                    placeholder="https://..."
                    value={link.url}
                    onChange={(e) => updateLink(i, "url", e.target.value)}
                    style={{ flex: 2 }}
                  />
                  <input
                    placeholder="Label (optional)"
                    value={link.label}
                    onChange={(e) => updateLink(i, "label", e.target.value)}
                    style={{ flex: 1 }}
                  />
                  <button type="button" onClick={() => removeLinkRow(i)}>
                    ×
                  </button>
                </div>
              ))}
              <button type="button" onClick={addLinkRow}>
                + Link hinzufügen
              </button>
            </fieldset>

            <label htmlFor="submission-files">Dateien</label>
            <input
              id="submission-files"
              type="file"
              multiple
              onChange={(e) => setFiles(e.target.files)}
              style={{ display: "block", width: "100%", marginBottom: "1rem" }}
            />

            <button type="submit" disabled={busy}>
              {busy ? "Sende..." : "Einreichen"}
            </button>
          </form>
        </section>
      )}

      {submissions.length > 0 && (
        <section style={{ marginTop: "2rem", borderTop: "1px solid #ccc", paddingTop: "1rem" }}>
          <h2>Submissions</h2>
          <ul style={{ listStyle: "none", padding: 0 }}>
            {submissions.map((submission) => (
              <li
                key={submission.id}
                style={{ border: "1px solid #ccc", borderRadius: 8, padding: "0.75rem", marginBottom: "0.75rem" }}
              >
                <p style={{ margin: 0 }}>
                  <strong>{submission.username}</strong> · {submission.status} ·{" "}
                  {new Date(submission.submittedAt).toLocaleString()}
                </p>
                <p>{submission.description}</p>

                {submission.links.length > 0 && (
                  <ul>
                    {submission.links.map((link) => (
                      <li key={link.id}>
                        <a href={link.url} target="_blank" rel="noopener noreferrer">
                          {link.label || link.url}
                        </a>
                      </li>
                    ))}
                  </ul>
                )}

                {submission.files.length > 0 && (
                  <ul>
                    {submission.files.map((file) => (
                      <li key={file.id}>
                        <a href={`/api/submissions/${submission.id}/files/${file.id}`} target="_blank" rel="noopener noreferrer">
                          {file.fileName}
                        </a>
                      </li>
                    ))}
                  </ul>
                )}

                {submission.reviewComment && (
                  <p style={{ color: "#666" }}>Review-Kommentar: {submission.reviewComment}</p>
                )}

                {canManage && pendingSubmission?.id === submission.id && (
                  <div style={{ marginTop: "0.5rem" }}>
                    <textarea
                      placeholder="Kommentar (optional)"
                      value={reviewComment}
                      onChange={(e) => setReviewComment(e.target.value)}
                      rows={2}
                      style={{ display: "block", width: "100%", marginBottom: "0.5rem" }}
                    />
                    <button type="button" disabled={busy} onClick={() => handleReview(submission.id, "Accept")}>
                      Accept
                    </button>
                    <button
                      type="button"
                      disabled={busy}
                      onClick={() => handleReview(submission.id, "RequestChanges")}
                      style={{ marginLeft: "0.5rem" }}
                    >
                      Request Changes
                    </button>
                    <button
                      type="button"
                      disabled={busy}
                      onClick={() => handleReview(submission.id, "Reject")}
                      style={{ marginLeft: "0.5rem", color: "red" }}
                    >
                      Reject
                    </button>
                  </div>
                )}
              </li>
            ))}
          </ul>
        </section>
      )}
    </main>
  );
}
