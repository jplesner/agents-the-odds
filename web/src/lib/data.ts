import { existsSync, readFileSync, readdirSync } from 'node:fs';
import { resolve } from 'node:path';
import { marked } from 'marked';
import type { AgentProfile, EpisodeResult, Leaderboard } from '../types/data.ts';

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

export function readEpisodeStrategyCodes(episodeNumber: number): Record<string, string> {
  const padded = String(episodeNumber).padStart(3, '0');
  const agentsDir = resolve(dataRoot(), 'agents');
  if (!existsSync(agentsDir)) return {};
  const result: Record<string, string> = {};
  for (const dirent of readdirSync(agentsDir, { withFileTypes: true })) {
    if (!dirent.isDirectory()) continue;
    const codePath = resolve(agentsDir, dirent.name, 'strategies', `episode-${padded}.cs`);
    if (existsSync(codePath)) {
      result[dirent.name] = readFileSync(codePath, 'utf-8');
    }
  }
  return result;
}

export function readAgentProfiles(): AgentProfile[] {
  const agentsDir = resolve(dataRoot(), 'agents');
  if (!existsSync(agentsDir)) return [];
  return readdirSync(agentsDir, { withFileTypes: true })
    .filter((d) => d.isDirectory())
    .map((d) => {
      const mdPath = resolve(agentsDir, d.name, 'personality.md');
      if (!existsSync(mdPath)) return null;
      const raw = readFileSync(mdPath, 'utf-8').trim();
      const lines = raw.split('\n');
      const name = lines[0].replace(/^#\s*/, '').trim();
      const body = lines.slice(1).join('\n').trim();
      const description = body.split(/\r?\n\r?\n/)[0].replace(/\*\*[^*]+\*\*/g, '').trim();
      const descriptionHtml = marked.parse(body) as string;
      const journalPath = resolve(agentsDir, d.name, 'journal.md');
      const journalRaw = existsSync(journalPath) ? readFileSync(journalPath, 'utf-8') : '';
      const journalBody = journalRaw.replace(/^#\s+[^\n]*\n?/, '').trim();
      const journalHtml = journalBody ? (marked.parse(journalBody) as string) : '';
      return { id: d.name, name, description, descriptionHtml, journalHtml } satisfies AgentProfile;
    })
    .filter((a): a is AgentProfile => a !== null);
}
