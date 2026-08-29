import { Globe } from "lucide-react";
import type { ComponentType } from "react";
import {
  FaBluesky,
  FaDiscord,
  FaGithub,
  FaInstagram,
  FaItchIo,
  FaLinkedin,
  FaReddit,
  FaTiktok,
  FaTwitch,
  FaXTwitter,
  FaYoutube,
} from "react-icons/fa6";
import type { LinkPlatform } from "./types";

export const LINK_PLATFORMS: LinkPlatform[] = [
  "X",
  "GitHub",
  "LinkedIn",
  "Instagram",
  "YouTube",
  "Twitch",
  "Discord",
  "TikTok",
  "ItchIo",
  "Reddit",
  "Bluesky",
  "Other",
];

export const LINK_PLATFORM_LABELS: Record<LinkPlatform, string> = {
  X: "X",
  GitHub: "GitHub",
  LinkedIn: "LinkedIn",
  Instagram: "Instagram",
  YouTube: "YouTube",
  Twitch: "Twitch",
  Discord: "Discord",
  TikTok: "TikTok",
  ItchIo: "itch.io",
  Reddit: "Reddit",
  Bluesky: "Bluesky",
  Other: "Andere",
};

export const LINK_PLATFORM_ICONS: Record<LinkPlatform, ComponentType<{ size?: number; className?: string }>> = {
  X: FaXTwitter,
  GitHub: FaGithub,
  LinkedIn: FaLinkedin,
  Instagram: FaInstagram,
  YouTube: FaYoutube,
  Twitch: FaTwitch,
  Discord: FaDiscord,
  TikTok: FaTiktok,
  ItchIo: FaItchIo,
  Reddit: FaReddit,
  Bluesky: FaBluesky,
  Other: Globe,
};
