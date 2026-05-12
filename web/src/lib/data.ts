import { existsSync, readFileSync, readdirSync } from 'node:fs';
import { resolve } from 'node:path';
import type { EpisodeResult, Leaderboard } from '../types/data.ts';

function dataRoot(): string {
  return resolve(process.cwd(), '..', 'data');
}

function readJsonOrNull<T>(filePath: string): T | null {
  try {
    if (!existsSync(filePath)) return null;
    return JSON.parse(readFileSync(filePath, 'utf-8')) as T;
  } catch {
    return null;
  }
}

function discoverEpisodeNumbers(): number[] {
  const dir = resolve(dataRoot(), 'episodes');
  if (!existsSync(dir)) return [];
  return readdirSync(dir)
    .map((f) => f.match(/^episode-(\d+)\.json$/)?.[1])
    .filter((n): n is string => n !== undefined)
    .map(Number)
    .sort((a, b) => a - b);
}

export function readLeaderboard(): Leaderboard {
  return readJsonOrNull<Leaderboard>(resolve(dataRoot(), 'leaderboard.json')) ?? { entries: [] };
}

export function readEpisode(episodeNumber: number): EpisodeResult | null {
  const padded = String(episodeNumber).padStart(3, '0');
  return readJsonOrNull<EpisodeResult>(resolve(dataRoot(), 'episodes', `episode-${padded}.json`));
}

export function readAllEpisodes(): EpisodeResult[] {
  return discoverEpisodeNumbers()
    .map(readEpisode)
    .filter((e): e is EpisodeResult => e !== null);
}

export function getEpisodeNumbers(): number[] {
  return discoverEpisodeNumbers();
}
