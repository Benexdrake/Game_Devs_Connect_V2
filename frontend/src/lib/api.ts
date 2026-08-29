import { cookies } from "next/headers";

const BACKEND_URL = process.env.BACKEND_URL ?? "http://localhost:5128";

/** Server-side fetch against the backend, forwarding the visitor's session cookie. */
export async function apiFetch(path: string, init?: RequestInit) {
  const cookieStore = await cookies();
  return fetch(`${BACKEND_URL}${path}`, {
    ...init,
    headers: {
      ...(init?.headers ?? {}),
      cookie: cookieStore.toString(),
    },
    cache: "no-store",
  });
}

export async function apiFetchJson<T>(path: string): Promise<T | null> {
  const res = await apiFetch(path);
  if (!res.ok) return null;
  return (await res.json()) as T;
}
