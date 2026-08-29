import Link from "next/link";
import { apiFetchJson } from "@/lib/api";
import type { DiscoverSort, Project } from "@/lib/types";

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
    <main style={{ maxWidth: 720, margin: "0 auto", padding: "2rem 1rem" }}>
      <h1>Projekte entdecken</h1>

      <nav style={{ display: "flex", gap: "1rem", marginBottom: "1.5rem", borderBottom: "1px solid #ccc" }}>
        {SECTIONS.map((section) => (
          <Link
            key={section.sort}
            href={`/discover?sort=${section.sort}`}
            style={{ fontWeight: section.sort === activeSort ? 700 : 400, paddingBottom: "0.5rem" }}
          >
            {section.label}
          </Link>
        ))}
      </nav>

      {projects.length === 0 ? (
        <p>Keine Projekte in dieser Kategorie.</p>
      ) : (
        <ul style={{ listStyle: "none", padding: 0 }}>
          {projects.map((project) => (
            <li key={project.id} style={{ border: "1px solid #ccc", borderRadius: 8, padding: "1rem", marginBottom: "0.75rem" }}>
              <Link href={`/projects/${project.slug}`} style={{ fontWeight: 600 }}>{project.title}</Link>
              <p style={{ margin: "0.25rem 0" }}>
                {project.genre} · {project.engine} · {project.status}
              </p>
              {project.tags.length > 0 && <p style={{ margin: 0, color: "#888" }}>Tags: {project.tags.join(", ")}</p>}
            </li>
          ))}
        </ul>
      )}
    </main>
  );
}
