"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { Input } from "@/components/ui";

export function SearchBar() {
  const router = useRouter();
  const [q, setQ] = useState("");

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!q.trim()) return;
    router.push(`/search?q=${encodeURIComponent(q.trim())}`);
  }

  return (
    <form onSubmit={handleSubmit} className="ml-2 w-48 sm:w-64">
      <Input
        type="search"
        placeholder="Suche Projekte, Quests, User..."
        value={q}
        onChange={(e) => setQ(e.target.value)}
        className="py-1.5"
      />
    </form>
  );
}
