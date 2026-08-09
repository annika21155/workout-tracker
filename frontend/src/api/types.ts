export interface ChallengeSummary {
  id: number;
  title: string;
  description: string | null;
  durationDays: number;
  isPublic: boolean;
  participantCount: number;
}

export interface ChallengeDay {
  id: number;
  dayNumber: number;
  videoUrl: string;
  videoTitle: string;
  durationMinutes: number;
}

export interface ChallengeDetail {
  id: number;
  title: string;
  description: string | null;
  youtubePlaylistUrl: string;
  durationDays: number;
  isPublic: boolean;
  days: ChallengeDay[];
}

export interface ActivityLogEntry {
  id: number;
  activityType: string;
  durationMinutes: number;
  loggedAt: string;
  pointsEarned: number;
}

export interface FriendshipEntry {
  id: number;
  friendUserId: number;
  friendUsername: string;
  status: string;
}

export interface LeaderboardEntry {
  userId: number;
  username: string;
  totalPoints: number;
  rank: number;
}