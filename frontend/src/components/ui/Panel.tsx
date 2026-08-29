import { HTMLAttributes } from "react";
import clsx from "clsx";

export function Panel({ className, ...props }: HTMLAttributes<HTMLDivElement>) {
  return (
    <div
      className={clsx("rounded-lg border-2 border-border-strong bg-surface p-4", className)}
      {...props}
    />
  );
}
