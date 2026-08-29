"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import type { CurrentUser, ProjectRole, Quest, Submission, SubmissionDecision } from "@/lib/types";
import { Badge, Button, Input, PageContainer, Panel, Textarea } from "@/components/ui";

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
    if (!confirm("Claim aufgeben? Die Quest wird wieder für alle offen und du kannst danach nichts mehr einreichen, bis du sie erneut claimst.")) {
      return;
    }
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
    <PageContainer width="xl">
      <p className="mb-2">
        <Link href={`/projects/${quest.projectSlug}`} className="text-accent hover:text-accent-bright">
          ← {quest.projectTitle}
        </Link>
      </p>
      <h1 className="mb-3 font-display text-base text-accent-bright">{quest.title}</h1>
      <div className="mb-3 flex flex-wrap items-center gap-2">
        <Badge>{quest.category}</Badge>
        <Badge tone="accent">{quest.difficulty}</Badge>
        <Badge tone="warning">{quest.xpReward} XP</Badge>
        <Badge tone={quest.status === "Open" ? "success" : "neutral"}>{quest.status}</Badge>
      </div>
      {quest.deadline && <p className="text-sm text-text-muted">Deadline: {new Date(quest.deadline).toLocaleDateString()}</p>}
      {quest.requiredSkills.length > 0 && (
        <p className="text-sm text-text-muted">Benötigte Skills: {quest.requiredSkills.map((s) => s.name).join(", ")}</p>
      )}
      <p>{quest.description}</p>
      <p className="text-sm text-text-muted">
        Erstellt von{" "}
        <Link href={`/users/${quest.creatorUsername}`} className="text-accent hover:text-accent-bright">
          {quest.creatorUsername}
        </Link>
      </p>
      {quest.claimedByUsername && !isActiveClaimer && (
        <p className="text-sm text-text-muted">
          Wird aktuell bearbeitet von{" "}
          <Link href={`/users/${quest.claimedByUsername}`} className="text-accent hover:text-accent-bright">
            {quest.claimedByUsername}
          </Link>
        </p>
      )}

      {error && <p className="mt-2 text-sm text-danger">{error}</p>}

      <div className="mt-4 flex items-center gap-3">
        {canClaim && (
          <Button type="button" disabled={busy} onClick={handleClaim}>
            Quest claimen
          </Button>
        )}
        {isCreator && quest.status === "Open" && (
          <Link href={`/quests/${quest.id}/edit`} className="text-accent hover:text-accent-bright">
            Bearbeiten
          </Link>
        )}
      </div>

      {canSubmit && (
        <section className="mt-8 border-t border-border pt-4">
          <Panel className="border-accent bg-accent/10 text-text">
            ✓ Du hast diese Quest geclaimt. Reiche unten deine Submission ein, sobald du fertig bist.
          </Panel>
          <h2 className="my-3 font-display text-xs text-accent-bright">SUBMISSION EINREICHEN</h2>
          <form onSubmit={handleSubmitSubmission}>
            <label htmlFor="submission-description" className="mb-1 block text-sm text-text-muted">
              Beschreibung
            </label>
            <Textarea
              id="submission-description"
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              required
              rows={4}
              className="mb-4"
            />

            <fieldset className="mb-4 rounded-md border border-border p-3">
              <legend className="px-1 text-sm text-text-muted">Links</legend>
              {links.map((link, i) => (
                <div key={i} className="mb-2 flex gap-2">
                  <Input
                    placeholder="https://..."
                    value={link.url}
                    onChange={(e) => updateLink(i, "url", e.target.value)}
                    className="flex-[2]"
                  />
                  <Input
                    placeholder="Label (optional)"
                    value={link.label}
                    onChange={(e) => updateLink(i, "label", e.target.value)}
                    className="flex-1"
                  />
                  <Button type="button" variant="ghost" onClick={() => removeLinkRow(i)}>
                    ×
                  </Button>
                </div>
              ))}
              <Button type="button" variant="secondary" onClick={addLinkRow}>
                + Link hinzufügen
              </Button>
            </fieldset>

            <label htmlFor="submission-files" className="mb-1 block text-sm text-text-muted">
              Dateien
            </label>
            <input
              id="submission-files"
              type="file"
              multiple
              onChange={(e) => setFiles(e.target.files)}
              className="mb-4 block w-full text-sm text-text-muted file:mr-3 file:rounded-md file:border file:border-border file:bg-canvas file:px-3 file:py-1.5 file:text-text"
            />

            <Button type="submit" disabled={busy}>
              {busy ? "Sende..." : "Einreichen"}
            </Button>
          </form>

          {canRelease && (
            <p className="mt-4">
              <Button type="button" variant="ghost" disabled={busy} onClick={handleRelease}>
                Claim aufgeben
              </Button>
            </p>
          )}
        </section>
      )}

      {(canManage || isActiveClaimer || submissions.length > 0) && (
        <section className="mt-8 border-t border-border pt-4">
          <h2 className="mb-3 font-display text-xs text-accent-bright">SUBMISSIONS</h2>
          {submissions.length === 0 && <p className="text-text-muted">Noch keine Submission eingereicht.</p>}
          <ul className="list-none space-y-3 p-0">
            {submissions.map((submission) => (
              <li key={submission.id}>
                <Panel>
                  <p className="m-0 text-sm">
                    <strong className="text-text">{submission.username}</strong>{" "}
                    <span className="text-text-muted">
                      · {submission.status} · {new Date(submission.submittedAt).toLocaleString()}
                    </span>
                  </p>
                  <p>{submission.description}</p>

                  {submission.links.length > 0 && (
                    <ul className="list-disc pl-5 text-sm">
                      {submission.links.map((link) => (
                        <li key={link.id}>
                          <a
                            href={link.url}
                            target="_blank"
                            rel="noopener noreferrer"
                            className="text-accent hover:text-accent-bright"
                          >
                            {link.label || link.url}
                          </a>
                        </li>
                      ))}
                    </ul>
                  )}

                  {submission.files.length > 0 && (
                    <ul className="list-disc pl-5 text-sm">
                      {submission.files.map((file) => (
                        <li key={file.id}>
                          <a
                            href={`/api/submissions/${submission.id}/files/${file.id}`}
                            target="_blank"
                            rel="noopener noreferrer"
                            className="text-accent hover:text-accent-bright"
                          >
                            {file.fileName}
                          </a>
                        </li>
                      ))}
                    </ul>
                  )}

                  {submission.reviewComment && (
                    <p className="text-sm text-text-muted">Review-Kommentar: {submission.reviewComment}</p>
                  )}

                  {canManage && pendingSubmission?.id === submission.id && (
                    <div className="mt-2">
                      <Textarea
                        placeholder="Kommentar (optional)"
                        value={reviewComment}
                        onChange={(e) => setReviewComment(e.target.value)}
                        rows={2}
                        className="mb-2"
                      />
                      <div className="flex gap-2">
                        <Button type="button" disabled={busy} onClick={() => handleReview(submission.id, "Accept")}>
                          Accept
                        </Button>
                        <Button
                          type="button"
                          variant="secondary"
                          disabled={busy}
                          onClick={() => handleReview(submission.id, "RequestChanges")}
                        >
                          Request Changes
                        </Button>
                        <Button
                          type="button"
                          variant="danger"
                          disabled={busy}
                          onClick={() => handleReview(submission.id, "Reject")}
                        >
                          Reject
                        </Button>
                      </div>
                    </div>
                  )}
                </Panel>
              </li>
            ))}
          </ul>
        </section>
      )}
    </PageContainer>
  );
}
