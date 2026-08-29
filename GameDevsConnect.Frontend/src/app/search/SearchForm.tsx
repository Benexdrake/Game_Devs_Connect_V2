"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { Button, Input } from "@/components/ui";

export function SearchForm({ initialQuery }: { initialQuery: string }) {
  const router = useRouter();
  const [q, setQ] = useState(initialQuery);

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!q.trim()) return;
    router.push(`/search?q=${encodeURIComponent(q.trim())}`);
  }

  return (
    <form onSubmit={handleSubmit} className="mb-6 flex gap-2">
      <Input
        type="search"
        placeholder="Suche Projekte, Quests, User..."
        value={q}
        onChange={(e) => setQ(e.target.value)}
        autoFocus
        className="flex-1"
      />
      <Button type="submit">Suchen</Button>
    </form>
  );
}
