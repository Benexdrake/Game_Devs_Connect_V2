import { ButtonHTMLAttributes, forwardRef } from "react";
import clsx from "clsx";

type Variant = "primary" | "secondary" | "danger" | "ghost";

export const Button = forwardRef<
  HTMLButtonElement,
  ButtonHTMLAttributes<HTMLButtonElement> & { variant?: Variant }
>(({ className, variant = "primary", ...props }, ref) => (
  <button
    ref={ref}
    className={clsx(
      "inline-flex items-center justify-center rounded-md border px-3 py-1.5 text-sm font-medium transition-colors disabled:cursor-not-allowed disabled:opacity-50",
      variant === "primary" &&
        "border-accent bg-accent text-surface hover:border-accent-bright hover:bg-accent-bright",
      variant === "secondary" &&
        "border-border bg-transparent text-text hover:border-accent hover:text-accent-bright",
      variant === "danger" &&
        "border-danger bg-transparent text-danger hover:bg-danger hover:text-surface",
      variant === "ghost" && "border-transparent bg-transparent text-text-muted hover:text-text",
      className,
    )}
    {...props}
  />
));
Button.displayName = "Button";
