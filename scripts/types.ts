export type {
  DrawResult,
  Prediction,
  PredictionResult,
  LeaderboardEntry,
  Leaderboard,
  EpisodeResult,
} from '../web/src/types/data.ts';

export interface AgentConfig {
  id: string;
  name: string;
  strategyClass: string;
}
