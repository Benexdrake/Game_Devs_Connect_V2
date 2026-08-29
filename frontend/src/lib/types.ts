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
  contributions: UserContribution[];
  isFollowedByMe: boolean;
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
  isFollowedByMe: boolean;
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
  claimedByUserId: string | null;
  claimedByUsername: string | null;
  requiredSkills: QuestSkill[];
  createdAt: string;
};

export type SubmissionStatus = "PendingReview" | "ChangesRequested" | "Accepted" | "Rejected";

export type SubmissionDecision = "Accept" | "Reject" | "RequestChanges";

export type SubmissionFileEntry = {
  id: string;
  fileName: string;
  contentType: string;
  sizeBytes: number;
  uploadedAt: string;
};

export type SubmissionLinkEntry = { id: string; url: string; label: string | null };

export type Submission = {
  id: string;
  questId: string;
  userId: string;
  username: string;
  description: string;
  status: SubmissionStatus;
  submittedAt: string;
  reviewedAt: string | null;
  reviewerId: string | null;
  reviewComment: string | null;
  files: SubmissionFileEntry[];
  links: SubmissionLinkEntry[];
};

export type UserContribution = {
  id: string;
  projectSlug: string;
  projectTitle: string;
  questId: string;
  questTitle: string;
  createdAt: string;
};

export type XpSummary = {
  totalXp: number;
  level: number;
  xpForCurrentLevel: number;
  xpForNextLevel: number;
  reputation: number | null;
  completedQuests: number;
  acceptedContributions: number;
};

export type ActivityEventType = "QuestCreated" | "ContributionAccepted" | "MemberJoined" | "ProjectPosted" | "LevelUp";

export type ActivityEvent = {
  id: string;
  type: ActivityEventType;
  actorUserId: string;
  actorUsername: string;
  projectId: string | null;
  projectSlug: string | null;
  projectTitle: string | null;
  summary: string;
  linkUrl: string | null;
  createdAt: string;
};

export type PostAttachmentEntry = { id: string; fileName: string; contentType: string };

export type Comment = { id: string; authorId: string; authorUsername: string; body: string; createdAt: string };

export type Post = {
  id: string;
  projectId: string;
  authorId: string;
  authorUsername: string;
  body: string;
  createdAt: string;
  attachments: PostAttachmentEntry[];
  comments: Comment[];
  likeCount: number;
  likedByMe: boolean;
};

export type NotificationType = "SubmissionReviewed" | "NewQuestInFollowedProject" | "NewFollower";

export type Notification = {
  id: string;
  type: NotificationType;
  message: string;
  isRead: boolean;
  createdAt: string;
};

export type NotificationsResult = { items: Notification[]; unreadCount: number };
