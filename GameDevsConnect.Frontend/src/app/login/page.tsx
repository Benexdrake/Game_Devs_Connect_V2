import { redirect } from "next/navigation";
import { apiFetchJson } from "@/lib/api";
import type { CurrentUser } from "@/lib/types";
import { PageContainer, Panel } from "@/components/ui";

export default async function LoginPage() {
  const me = await apiFetchJson<CurrentUser>("/api/auth/me");

  if (me) {
    redirect("/");
  }

  return (
    <PageContainer className="flex max-w-md flex-col items-center gap-6 py-24 text-center">
      <h1 className="font-display text-sm text-accent-bright">LOGIN</h1>
      <Panel className="w-full">
        <p className="mb-4 text-text-muted">Melde dich mit deinem GitHub-Account an, um fortzufahren.</p>
        <a
          href="/api/auth/login/github"
          className="inline-block rounded-md border-2 border-accent bg-accent px-5 py-2.5 font-medium text-surface transition-colors hover:border-accent-bright hover:bg-accent-bright"
        >
          Login with GitHub
        </a>
      </Panel>
    </PageContainer>
  );
}
