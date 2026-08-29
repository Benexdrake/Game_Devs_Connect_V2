import Link from "next/link";

export function SiteFooter() {
  return (
    <footer className="mb-16 border-t border-border lg:mb-0">
      <div className="mx-auto flex max-w-[1200px] items-center justify-between gap-4 px-4 py-4 text-xs text-text-muted">
        <span>© {new Date().getFullYear()} Gamedevs Connect</span>
        <Link href="/impressum" className="hover:text-accent-bright">
          Impressum
        </Link>
      </div>
    </footer>
  );
}
