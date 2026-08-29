"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";

export function FollowButton({
  followUrl,
  initialFollowing,
}: {
  followUrl: string;
  initialFollowing: boolean;
}) {
  const router = useRouter();
  const [following, setFollowing] = useState(initialFollowing);
  const [busy, setBusy] = useState(false);

  async function toggle() {
    setBusy(true);
    try {
      const res = await fetch(followUrl, {
        method: following ? "DELETE" : "POST",
        credentials: "include",
      });
      if (res.ok) {
        setFollowing(!following);
        router.refresh();
      }
    } finally {
      setBusy(false);
    }
  }

  return (
    <button type="button" onClick={toggle} disabled={busy}>
      {following ? "Entfolgen" : "Folgen"}
    </button>
  );
}
