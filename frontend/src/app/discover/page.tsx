import Link from "next/link";
import clsx from "clsx";
import { apiFetchJson } from "@/lib/api";
import type { DiscoverSort, Project } from "@/lib/types";
import { Badge, PageContainer, Panel } from "@/components/ui";

const SECTIONS: { sort: DiscoverSort; label: string }[] = [
  { sort: "trending", label: "Trending" },
  { sort: "recent", label: "Recently Updated" },
  { sort: "looking-for-contributors", label: "Looking for Contributors" },
  { sort: "new", label: "New" },
];

export default async function DiscoverPage({
  searchParams,
}: {
  searchParams: Promise<{ sort?: string }>;
}) {
  const { sort: sortParam } = await searchParams;
  const activeSort: DiscoverSort = SECTIONS.some((s) => s.sort === sortParam)
    ? (sortParam as DiscoverSort)
    : "trending";

  const projects = (await apiFetchJson<Project[]>(`/api/projects/discover?sort=${activeSort}`)) ?? [];

  return (
    <PageContainer>
      <h1 className="mb-6 font-display text-sm text-accent-bright">PROJEKTE ENTDECKEN</h1>

      <nav className="mb-6 flex flex-wrap gap-1 border-b border-border">
        {SECTIONS.map((section) => (
          <Link
            key={section.sort}
            href={`/discover?sort=${section.sort}`}
            className={clsx(
              "border-b-2 px-3 py-2 text-sm transition-colors",
              section.sort === activeSort
                ? "border-accent-bright text-accent-bright"
                : "border-transparent text-text-muted hover:text-text",
            )}
          >
            {section.label}
          </Link>
        ))}
      </nav>

      {projects.length === 0 ? (
        <Panel className="text-text-muted">Keine Projekte in dieser Kategorie.</Panel>
      ) : (
        <ul className="list-none space-y-3 p-0">
          {projects.map((project) => (
            <li key={project.id}>
              <Panel>
                <Link href={`/projects/${project.slug}`} className="font-medium text-text hover:text-accent-bright">
                  {project.title}
                </Link>
                <p className="m-0 mt-1 text-sm text-text-muted">
                  {project.genre} · {project.engine} · {project.status}
                </p>
                {project.tags.length > 0 && (
                  <div className="mt-2 flex flex-wrap gap-2">
                    {project.tags.map((t) => (
                      <Badge key={t}>{t}</Badge>
                    ))}
                  </div>
                )}
              </Panel>
            </li>
          ))}
        </ul>
      )}
    </PageContainer>
  );
}
