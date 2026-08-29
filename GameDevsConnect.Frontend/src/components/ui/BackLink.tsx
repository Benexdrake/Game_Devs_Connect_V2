"use client";

import { useRouter } from "next/navigation";
import { ArrowLeft } from "lucide-react";
import clsx from "clsx";

export function BackLink({ fallbackHref, className }: { fallbackHref: string; className?: string }) {
  const router = useRouter();

  function handleClick() {
    // If we got here via a click inside the app, history has something to go back to.
    // A direct/bookmarked visit has no such entry, so fall back to a fixed destination
    // instead of leaving the site or landing on a blank tab.
    if (window.history.length > 1) {
      router.back();
    } else {
      router.push(fallbackHref);
    }
  }

  return (
    <button
      type="button"
      onClick={handleClick}
      title="Zurück"
      aria-label="Zurück"
      className={clsx(
        "inline-flex h-6 w-6 shrink-0 items-center justify-center rounded border border-border text-text-muted transition-colors hover:border-accent hover:text-accent-bright",
        className,
      )}
    >
      <ArrowLeft size={14} />
    </button>
  );
}
