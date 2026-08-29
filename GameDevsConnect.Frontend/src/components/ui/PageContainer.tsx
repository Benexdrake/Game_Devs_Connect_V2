import { HTMLAttributes } from "react";
import clsx from "clsx";

// Explicit variants instead of letting callers pass a conflicting max-w-*
// className - two max-w-* utilities on the same element don't reliably
// "override" each other based on JSX order, so this is the only reliable
// way to narrow the container (forms) vs. the default page width, which
// matches the top bar's max-width so content and nav sit in one shared
// centered block (like X).
const WIDTHS = {
  page: "max-w-3xl",
  xl: "max-w-xl",
  md: "max-w-md",
} as const;

export function PageContainer({
  className,
  width = "page",
  ...props
}: HTMLAttributes<HTMLElement> & { width?: keyof typeof WIDTHS }) {
  return <main className={clsx("mx-auto w-full px-4 pt-8 pb-24 lg:pb-8", WIDTHS[width], className)} {...props} />;
}
