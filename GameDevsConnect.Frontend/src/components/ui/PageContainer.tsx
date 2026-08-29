import { HTMLAttributes } from "react";
import clsx from "clsx";

export function PageContainer({ className, ...props }: HTMLAttributes<HTMLElement>) {
  return <main className={clsx("mx-auto w-full max-w-3xl px-4 py-8", className)} {...props} />;
}
