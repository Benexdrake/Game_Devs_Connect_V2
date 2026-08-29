"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import type { CurrentUser } from "@/lib/types";

export function LoginLink() {
  const [me, setMe] = useState<CurrentUser | null>(null);
  const [checked, setChecked] = useState(false);

  useEffect(() => {
    let cancelled = false;
    fetch("/api/auth/me", { credentials: "include" })
      .then((res) => (res.ok ? res.json() : null))
      .then((json) => {
        if (!cancelled) {
          setMe(json);
          setChecked(true);
        }
      })
      .catch(() => {
        if (!cancelled) setChecked(true);
      });
    return () => {
      cancelled = true;
    };
  }, []);

  if (!checked || me) {
    return null;
  }

  return (
    <Link href="/login" className="text-sm text-accent hover:text-accent-bright">
      Login
    </Link>
  );
}
