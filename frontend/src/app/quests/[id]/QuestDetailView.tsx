"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useState } from "react";
import type { CurrentUser, Quest } from "@/lib/types";

export function QuestDetailView({ quest, me }: { quest: Quest; me: CurrentUser | null }) {
  const router = useRouter();
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const isCreator = me?.id === quest.creatorId;
  const canClaim = me !== null && !isCreator && quest.status === "Open";
  const canRelease = me !== null && !isCreator && quest.status === "InProgress";

  async function handleClaim() {
    setBusy(true);
    setError(null);
    try {
      const res = await fetch(`/api/quests/${quest.id}/claim`, { method: "POST", credentials: "include" });
      if (!res.ok) {
        setError("Claim fehlgeschlagen (evtl. schon vergeben).");
        return;
      }
      router.refresh();
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
      router.refresh();
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
    </main>
  );
}
