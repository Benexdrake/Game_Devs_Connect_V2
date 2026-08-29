import { HTMLAttributes } from "react";
import clsx from "clsx";

type Tone = "neutral" | "accent" | "success" | "danger" | "warning";

export function Badge({
  className,
  tone = "neutral",
  ...props
}: HTMLAttributes<HTMLSpanElement> & { tone?: Tone }) {
  return (
    <span
      className={clsx(
        "inline-block rounded border px-2 py-0.5 text-xs font-medium whitespace-nowrap",
        tone === "neutral" && "border-border text-text-muted",
        tone === "accent" && "border-accent text-accent-bright",
        tone === "success" && "border-emerald-600 text-emerald-400",
        tone === "danger" && "border-danger text-danger",
        tone === "warning" && "border-amber-500 text-amber-400",
        className,
      )}
      {...props}
    />
  );
}
