"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";

export function SearchBar() {
  const router = useRouter();
  const [q, setQ] = useState("");

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!q.trim()) return;
    router.push(`/search?q=${encodeURIComponent(q.trim())}`);
  }

  return (
    <form onSubmit={handleSubmit} style={{ marginLeft: "1rem" }}>
      <input
        type="search"
        placeholder="Suche Projekte, Quests, User..."
        value={q}
        onChange={(e) => setQ(e.target.value)}
        style={{ width: 220 }}
      />
    </form>
  );
}
