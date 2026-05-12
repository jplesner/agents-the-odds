export interface DrawResult {
  drawNumber: number;
  date: string;
  numbers: number[];
}

export interface Prediction {
  agentId: string;
  strategyName: string;
  numbers: number[];
  confidence: number;
  reasoning: string;
}

export interface PredictionResult {
  prediction: Prediction;
  draw: DrawResult;
  matches: number;
  points: number;
}

export interface LeaderboardEntry {
  agentId: string;
  agentName: string;
  totalPoints: number;
  rank: number;
}

export interface Leaderboard {
  entries: LeaderboardEntry[];
}

export interface EpisodeResult {
  episodeNumber: number;
  drawResult: DrawResult;
  scores: PredictionResult[];
  leaderboard: LeaderboardEntry[];
  realityCheck: string;
}
