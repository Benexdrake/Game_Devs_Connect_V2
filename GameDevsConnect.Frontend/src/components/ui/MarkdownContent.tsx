import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import rehypeRaw from "rehype-raw";
import rehypeSanitize, { defaultSchema } from "rehype-sanitize";
import clsx from "clsx";

// GitHub-style pasted images are raw `<img width height alt src>` tags, not
// `![]()` markdown - remark treats those as opaque HTML text, so rehype-raw
// re-parses them into real elements before rehype-sanitize checks them
// against this schema. The default schema doesn't even allow `alt`, let
// alone `width`/`height`/`style`, since it's built for stripped-down
// markdown-only rendering - widen only `img`, on purpose, everything else
// stays at the strict default.
const schema = {
  ...defaultSchema,
  attributes: {
    ...defaultSchema.attributes,
    img: [...(defaultSchema.attributes?.img ?? []), "alt", "width", "height", "style", "align"],
  },
};

export function MarkdownContent({ children, className }: { children: string; className?: string }) {
  return (
    <div
      className={clsx(
        "space-y-2 text-sm text-text",
        "[&_a]:text-accent [&_a]:underline",
        "[&_blockquote]:border-l-2 [&_blockquote]:border-border [&_blockquote]:pl-3 [&_blockquote]:text-text-muted",
        "[&_code]:rounded [&_code]:bg-surface [&_code]:px-1 [&_code]:py-0.5 [&_code]:font-mono [&_code]:text-xs",
        "[&_h1]:text-base [&_h1]:font-semibold [&_h2]:text-sm [&_h2]:font-semibold [&_h3]:text-sm [&_h3]:font-semibold",
        // No forced height here on purpose: browsers already derive the
        // aspect ratio from an <img>'s width/height attributes, so editing
        // them by hand (e.g. to shrink or stretch the image) shows up in
        // preview. A blanket `h-auto` would silently override that and
        // always fall back to the file's real proportions instead.
        "[&_hr]:border-border [&_img]:max-w-full [&_img]:rounded",
        // `align` on <img> isn't a real value browsers act on by themselves
        // (not even "center", which was never a valid value for it) -
        // GitHub's own stylesheet is what makes align="center"/"left"/
        // "right" actually do anything there, so we replicate that mapping
        // here rather than relying on non-existent native support. Plain
        // margins, not `float`: this only positions the image itself, no
        // text-wrap-around-the-image behavior.
        '[&_img[align="center"]]:mx-auto [&_img[align="center"]]:block',
        '[&_img[align="left"]]:mr-auto [&_img[align="left"]]:ml-0 [&_img[align="left"]]:block',
        '[&_img[align="right"]]:ml-auto [&_img[align="right"]]:mr-0 [&_img[align="right"]]:block',
        "[&_ol]:list-decimal [&_ol]:space-y-1 [&_ol]:pl-5 [&_ul]:list-disc [&_ul]:space-y-1 [&_ul]:pl-5",
        "[&_pre]:overflow-x-auto [&_pre]:rounded-md [&_pre]:border [&_pre]:border-border [&_pre]:bg-surface [&_pre]:p-3",
        "[&_pre_code]:bg-transparent [&_pre_code]:p-0",
        "[&_table]:border-collapse [&_td]:border [&_td]:border-border [&_td]:px-2 [&_td]:py-1 [&_th]:border [&_th]:border-border [&_th]:px-2 [&_th]:py-1",
        className,
      )}
    >
      <ReactMarkdown remarkPlugins={[remarkGfm]} rehypePlugins={[rehypeRaw, [rehypeSanitize, schema]]}>
        {children}
      </ReactMarkdown>
    </div>
  );
}
