import { InputHTMLAttributes, forwardRef, TextareaHTMLAttributes, SelectHTMLAttributes } from "react";
import { twMerge } from "tailwind-merge";

const fieldClass =
  "w-full rounded-md border border-border bg-canvas px-3 py-2 text-sm text-text placeholder:text-text-muted focus:border-accent focus:outline-none";

export const Input = forwardRef<HTMLInputElement, InputHTMLAttributes<HTMLInputElement>>(
  ({ className, ...props }, ref) => <input ref={ref} className={twMerge(fieldClass, className)} {...props} />,
);
Input.displayName = "Input";

export const Textarea = forwardRef<HTMLTextAreaElement, TextareaHTMLAttributes<HTMLTextAreaElement>>(
  ({ className, ...props }, ref) => <textarea ref={ref} className={twMerge(fieldClass, className)} {...props} />,
);
Textarea.displayName = "Textarea";

export const Select = forwardRef<HTMLSelectElement, SelectHTMLAttributes<HTMLSelectElement>>(
  ({ className, ...props }, ref) => <select ref={ref} className={twMerge(fieldClass, className)} {...props} />,
);
Select.displayName = "Select";
