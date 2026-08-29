"use client";

import { useRef, useState } from "react";
import clsx from "clsx";
import { MarkdownContent } from "./MarkdownContent";

type MarkdownEditorProps = {
  value: string;
  onChange: (value: string) => void;
  maxLength?: number;
  maxUploads?: number;
  placeholder?: string;
  className?: string;
};

type ToolbarAction = "bold" | "italic" | "heading" | "list" | "link" | "image";

const IMAGE_TAG_PATTERN = /<img\b[^>]*\/?>/gi;
const POSITION_STEP_PX = 10;
const FALLBACK_WIDTH = 300;

/** Finds the `<img>` tag overlapping [from, to] - a plain cursor position (from === to) counts as "inside" it. */
function findImageTagInRange(text: string, from: number, to: number) {
  const regex = new RegExp(IMAGE_TAG_PATTERN);
  let match: RegExpExecArray | null;
  while ((match = regex.exec(text))) {
    const start = match.index;
    const end = start + match[0].length;
    if (from <= end && to >= start) {
      return { start, end, tag: match[0] };
    }
  }
  return null;
}

function parseWidth(tag: string): number {
  const match = tag.match(/width="(\d+(?:\.\d+)?)"/);
  return match ? parseFloat(match[1]) : FALLBACK_WIDTH;
}

/** Reads the pixel offset from dead-center out of a previously-generated `positionStyle`, 0 for plain centering or anything else. */
function parseOffset(tag: string): number {
  const style = tag.match(/style="([^"]*)"/)?.[1] ?? "";
  const match = style.match(/margin-left:\s*calc\(50%\s*-\s*[\d.]+px\s*([+-])\s*([\d.]+)px\)/);
  if (!match) return 0;
  const value = parseFloat(match[2]);
  return match[1] === "-" ? -value : value;
}

/**
 * `offset` is how far the image's center sits from the container's center, in
 * px - 0 is exactly centered. The bottom margin is baked into every image's
 * own style (rather than relying on spacing between markdown/preview blocks)
 * because consecutive `<img>` lines with no blank line between them get
 * merged into a single raw-HTML block by the markdown parser - block-level
 * spacing between "different blocks" then simply doesn't apply between them.
 */
function positionStyle(width: number, offsetPx: number): string {
  // The 50% is resolved by the browser against the actual (unknown-to-us) container
  // width at render time, so this stays correctly centered-plus-offset regardless of
  // how wide the description column ends up - no need to know that width up front.
  const marginLeft =
    offsetPx === 0
      ? "auto"
      : `calc(50% - ${width / 2}px ${offsetPx >= 0 ? "+" : "-"} ${Math.abs(offsetPx)}px)`;
  return `display: block; margin: 0 auto 0.75rem; margin-left: ${marginLeft};`;
}

function setAttr(tag: string, name: string, value: string): string {
  const pattern = new RegExp(`${name}="[^"]*"`);
  if (pattern.test(tag)) {
    return tag.replace(pattern, `${name}="${value}"`);
  }
  return tag.replace(/\/?>\s*$/, ` ${name}="${value}" />`);
}

export function MarkdownEditor({ value, onChange, maxLength, maxUploads, placeholder, className }: MarkdownEditorProps) {
  const [tab, setTab] = useState<"write" | "preview">("write");
  const [uploading, setUploading] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [selection, setSelection] = useState({ start: 0, end: 0 });
  const uploadCountRef = useRef(0);
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const activeImage = findImageTagInRange(value, selection.start, selection.end);

  function syncSelection() {
    const textarea = textareaRef.current;
    if (!textarea) return;
    setSelection({ start: textarea.selectionStart, end: textarea.selectionEnd });
  }

  /** Leading/trailing newlines needed so a block element gets its own blank-line-separated paragraph. */
  function blockPadding(text: string, start: number, end: number) {
    const before = text.slice(0, start);
    const after = text.slice(end);
    const leading = before === "" ? "" : before.endsWith("\n\n") ? "" : before.endsWith("\n") ? "\n" : "\n\n";
    const trailing = after === "" ? "" : after.startsWith("\n\n") ? "" : after.startsWith("\n") ? "\n" : "\n\n";
    return { leading, trailing };
  }

  function insertAtCursor(before: string, after = "", placeholderText = "", block = false) {
    const textarea = textareaRef.current;
    if (!textarea) return;
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const selected = value.slice(start, end) || placeholderText;

    if (block) {
      const { leading, trailing } = blockPadding(value, start, end);
      before = leading + before;
      after = after + trailing;
    }

    const next = value.slice(0, start) + before + selected + after + value.slice(end);
    onChange(maxLength ? next.slice(0, maxLength) : next);
    requestAnimationFrame(() => {
      textarea.focus();
      const cursor = start + before.length + selected.length + after.length;
      textarea.setSelectionRange(cursor, cursor);
    });
  }

  function applyToolbarAction(action: ToolbarAction) {
    switch (action) {
      case "bold":
        insertAtCursor("**", "**", "fett");
        break;
      case "italic":
        insertAtCursor("*", "*", "kursiv");
        break;
      case "heading":
        insertAtCursor("## ", "", "Überschrift", true);
        break;
      case "list":
        insertAtCursor("- ", "", "Eintrag", true);
        break;
      case "link":
        insertAtCursor("[", "](https://)", "Linktext");
        break;
      case "image":
        fileInputRef.current?.click();
        break;
    }
  }

  async function uploadImage(file: File) {
    if (maxUploads !== undefined && uploadCountRef.current >= maxUploads) {
      setUploadError(`Maximal ${maxUploads} Bilder erlaubt.`);
      return;
    }
    setUploading(true);
    setUploadError(null);
    try {
      const width = await readImageWidth(file);
      const formData = new FormData();
      formData.append("file", file);
      const res = await fetch("/api/uploads/images", { method: "POST", credentials: "include", body: formData });
      if (!res.ok) {
        setUploadError("Bild-Upload fehlgeschlagen.");
        return;
      }
      const { url } = (await res.json()) as { url: string };
      uploadCountRef.current += 1;
      // Same shape GitHub inserts: a raw <img> tag with the real pixel width
      // and an inline style, so both can be tweaked by hand afterwards (or
      // via the size/position toolbar buttons) - a plain `![](url)` gives no
      // such hook. No `height` attribute: Tailwind's preflight resets every
      // <img> to `height: auto`, so a fixed height here would just be
      // silently ignored - width alone still scales proportionally via the
      // image's natural aspect ratio.
      const markup = width
        ? `<img width="${width}" alt="Image" src="${url}" style="${positionStyle(width, 0)}" />`
        : `![](${url})`;
      insertAtCursor(markup, "", "", true);
    } finally {
      setUploading(false);
    }
  }

  /** Patches the `<img>` tag the cursor/selection is currently on, in place. */
  function adjustActiveImage(mutate: (width: number, offsetPx: number) => { width: number; offsetPx: number }) {
    const textarea = textareaRef.current;
    const active = findImageTagInRange(value, textarea?.selectionStart ?? selection.start, textarea?.selectionEnd ?? selection.end);
    if (!textarea || !active) return;

    const { width, offsetPx } = mutate(parseWidth(active.tag), parseOffset(active.tag));
    const clampedWidth = Math.max(1, Math.round(width));

    let newTag = setAttr(active.tag, "width", String(clampedWidth));
    newTag = setAttr(newTag, "style", positionStyle(clampedWidth, offsetPx));

    const newValue = value.slice(0, active.start) + newTag + value.slice(active.end);
    onChange(maxLength ? newValue.slice(0, maxLength) : newValue);

    requestAnimationFrame(() => {
      textarea.focus();
      const cursor = active.start + newTag.length;
      textarea.setSelectionRange(cursor, cursor);
      setSelection({ start: cursor, end: cursor });
    });
  }

  async function readImageWidth(file: File): Promise<number | null> {
    if (typeof createImageBitmap !== "function") return null;
    try {
      const bitmap = await createImageBitmap(file);
      const width = bitmap.width;
      bitmap.close();
      return width;
    } catch {
      return null;
    }
  }

  function handlePaste(e: React.ClipboardEvent<HTMLTextAreaElement>) {
    const item = Array.from(e.clipboardData.items).find((i) => i.type.startsWith("image/"));
    const file = item?.getAsFile();
    if (!file) return;
    e.preventDefault();
    void uploadImage(file);
  }

  function handleDrop(e: React.DragEvent<HTMLTextAreaElement>) {
    const file = Array.from(e.dataTransfer.files).find((f) => f.type.startsWith("image/"));
    if (!file) return;
    e.preventDefault();
    void uploadImage(file);
  }

  function handleFileInputChange(e: React.ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (file) void uploadImage(file);
    e.target.value = "";
  }

  return (
    <div className={clsx("rounded-md border border-border bg-canvas", className)}>
      <div className="flex items-center justify-between border-b border-border px-2 py-1">
        <div className="flex gap-1">
          <button type="button" onClick={() => setTab("write")} className={tabClass(tab === "write")}>
            Write
          </button>
          <button type="button" onClick={() => setTab("preview")} className={tabClass(tab === "preview")}>
            Preview
          </button>
        </div>
        {tab === "write" && (
          <div className="flex gap-1">
            <ToolbarButton label="Fett" onClick={() => applyToolbarAction("bold")}>
              <strong>B</strong>
            </ToolbarButton>
            <ToolbarButton label="Kursiv" onClick={() => applyToolbarAction("italic")}>
              <em>I</em>
            </ToolbarButton>
            <ToolbarButton label="Überschrift" onClick={() => applyToolbarAction("heading")}>
              H
            </ToolbarButton>
            <ToolbarButton label="Liste" onClick={() => applyToolbarAction("list")}>
              •
            </ToolbarButton>
            <ToolbarButton label="Link" onClick={() => applyToolbarAction("link")}>
              🔗
            </ToolbarButton>
            <ToolbarButton label="Bild einfügen" onClick={() => applyToolbarAction("image")} disabled={uploading}>
              🖼
            </ToolbarButton>
            <div className="mx-1 w-px self-stretch bg-border" />
            <ToolbarButton
              label="Bild schmaler (-1px)"
              disabled={!activeImage}
              onClick={() => adjustActiveImage((width, offsetPx) => ({ width: width - 1, offsetPx }))}
            >
              −
            </ToolbarButton>
            <ToolbarButton
              label="Bild breiter (+1px)"
              disabled={!activeImage}
              onClick={() => adjustActiveImage((width, offsetPx) => ({ width: width + 1, offsetPx }))}
            >
              +
            </ToolbarButton>
            <ToolbarButton
              label="Bild nach links"
              disabled={!activeImage}
              onClick={() => adjustActiveImage((width, offsetPx) => ({ width, offsetPx: offsetPx - POSITION_STEP_PX }))}
            >
              &lt;
            </ToolbarButton>
            <ToolbarButton
              label="Bild zentrieren"
              disabled={!activeImage}
              onClick={() => adjustActiveImage((width) => ({ width, offsetPx: 0 }))}
            >
              X
            </ToolbarButton>
            <ToolbarButton
              label="Bild nach rechts"
              disabled={!activeImage}
              onClick={() => adjustActiveImage((width, offsetPx) => ({ width, offsetPx: offsetPx + POSITION_STEP_PX }))}
            >
              &gt;
            </ToolbarButton>
          </div>
        )}
      </div>

      {/* Both panels stay mounted (toggled via `hidden`) so switching tabs doesn't reset a
          manually resized textarea height or the browser's undo history for it. */}
      <textarea
        ref={textareaRef}
        hidden={tab !== "write"}
        value={value}
        onChange={(e) => {
          onChange(maxLength ? e.target.value.slice(0, maxLength) : e.target.value);
          syncSelection();
        }}
        onPaste={handlePaste}
        onDrop={handleDrop}
        onDragOver={(e) => e.preventDefault()}
        onSelect={syncSelection}
        onClick={syncSelection}
        onKeyUp={syncSelection}
        placeholder={placeholder}
        rows={10}
        className="block w-full resize-y border-0 bg-transparent p-3 text-sm text-text placeholder:text-text-muted focus:outline-none"
      />
      {/* flow-root: gives this panel its own block-formatting context so it always
          sizes to fully wrap its rendered content (e.g. a left/right-aligned image),
          instead of the footer bar below it overlapping that content. */}
      <div hidden={tab !== "preview"} className="min-h-[14rem] flow-root p-3">
        {value ? <MarkdownContent>{value}</MarkdownContent> : <p className="text-sm text-text-muted">Nichts zum Anzeigen.</p>}
      </div>

      <input ref={fileInputRef} type="file" accept="image/*" onChange={handleFileInputChange} className="hidden" />

      <div className="flex flex-wrap items-center justify-between gap-1 border-t border-border px-3 py-1.5 text-xs text-text-muted">
        <span>
          Markdown wird unterstützt. Bilder per Einfügen, Ziehen oder Button hinzufügen.
          {uploading && " Bild wird hochgeladen..."}
        </span>
        {maxLength !== undefined && (
          <span className={clsx(value.length >= maxLength && "text-danger")}>
            {value.length} / {maxLength}
          </span>
        )}
      </div>
      {uploadError && <p className="border-t border-border px-3 py-1.5 text-xs text-danger">{uploadError}</p>}
    </div>
  );
}

function tabClass(active: boolean) {
  return clsx(
    "rounded-t-md px-3 py-1 text-sm transition-colors",
    active ? "bg-canvas text-accent-bright" : "text-text-muted hover:text-text",
  );
}

function ToolbarButton({
  label,
  onClick,
  disabled,
  children,
}: {
  label: string;
  onClick: () => void;
  disabled?: boolean;
  children: React.ReactNode;
}) {
  return (
    <button
      type="button"
      title={label}
      aria-label={label}
      disabled={disabled}
      onClick={onClick}
      className="rounded px-2 py-1 text-xs text-text-muted hover:bg-border hover:text-text disabled:cursor-not-allowed disabled:opacity-50"
    >
      {children}
    </button>
  );
}
