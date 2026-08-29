export type SkillCategory =
  | "Programming"
  | "Art2D"
  | "Art3D"
  | "Animation"
  | "Audio"
  | "Design"
  | "Writing"
  | "Other";

export type Skill = { id: string; name: string; category: SkillCategory };

export type UserLink = { label: string; url: string };

export type ProjectStatus = "Concept" | "InDevelopment" | "Beta" | "Released" | "Archived";
export type ProjectVisibility = "Public" | "Private";
export type ProjectRole = "Owner" | "Admin" | "Contributor";

export type UserProjectSummary = {
  slug: string;
  title: string;
  logoUrl: string | null;
  status: ProjectStatus;
};

export type UserProfile = {
  id: string;
  username: string;
  avatarUrl: string | null;
  bio: string | null;
  links: UserLink[];
  skills: Skill[];
  projects: UserProjectSummary[];
};

export type ProjectMember = {
  userId: string;
  username: string;
  avatarUrl: string | null;
  role: ProjectRole;
};

export type Project = {
  id: string;
  slug: string;
  title: string;
  description: string | null;
  logoUrl: string | null;
  bannerUrl: string | null;
  engine: string | null;
  genre: string | null;
  status: ProjectStatus;
  visibility: ProjectVisibility;
  tags: string[];
  members: ProjectMember[];
  createdAt: string;
};

export type CurrentUser = { id: string; username: string; avatarUrl: string | null };

export type QuestDifficulty = "Easy" | "Medium" | "Hard";

export type QuestStatus =
  | "Open"
  | "Claimed"
  | "InProgress"
  | "Submitted"
  | "InReview"
  | "ChangesRequested"
  | "Accepted"
  | "Rejected"
  | "Cancelled";

export type QuestSkill = { id: string; name: string; category: SkillCategory };

export type Quest = {
  id: string;
  projectId: string;
  projectSlug: string;
  projectTitle: string;
  creatorId: string;
  creatorUsername: string;
  title: string;
  description: string | null;
  category: SkillCategory;
  difficulty: QuestDifficulty;
  xpReward: number;
  status: QuestStatus;
  deadline: string | null;
  maxContributors: number;
  requiredSkills: QuestSkill[];
  createdAt: string;
};
