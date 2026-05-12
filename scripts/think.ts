import Anthropic from "@anthropic-ai/sdk";
import { execSync } from "child_process";
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import { SYSTEM_PROMPT, UPDATE_AGENT_TOOL } from "./prompt.js";
import type { AgentConfig, DrawResult, EpisodeResult, Leaderboard } from "./types.js";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.join(__dirname, "..");

const client = new Anthropic();

function loadAgents(): AgentConfig[] {
  const output = execSync(
    `dotnet run --project "${path.join(REPO_ROOT, "src", "AgentsTheOdds.Cli")}" -- agents`,
    { cwd: REPO_ROOT },
  ).toString().trim();
  return JSON.parse(output) as AgentConfig[];
}

function dataRoot(): string {
  return process.env.AGENTS_DATA_ROOT ?? path.join(REPO_ROOT, "data");
}

function padEpisode(n: number): string {
  return String(n).padStart(3, "0");
}

function loadDraws(upToEpisode: number): DrawResult[] {
  const drawsDir = path.join(dataRoot(), "draws");
  const draws: DrawResult[] = [];
  for (let i = 1; i < upToEpisode; i++) {
    const file = path.join(drawsDir, `episode-${padEpisode(i)}.json`);
    if (fs.existsSync(file)) {
      draws.push(JSON.parse(fs.readFileSync(file, "utf-8")) as DrawResult);
    }
  }
  return draws;
}

function loadEpisodeResults(upToEpisode: number): EpisodeResult[] {
  const episodesDir = path.join(dataRoot(), "episodes");
  const results: EpisodeResult[] = [];
  for (let i = 1; i < upToEpisode; i++) {
    const file = path.join(episodesDir, `episode-${padEpisode(i)}.json`);
    if (fs.existsSync(file)) {
      results.push(JSON.parse(fs.readFileSync(file, "utf-8")) as EpisodeResult);
    }
  }
  return results;
}

function loadLeaderboard(): Leaderboard {
  const file = path.join(dataRoot(), "leaderboard.json");
  if (!fs.existsSync(file)) return { entries: [] };
  return JSON.parse(fs.readFileSync(file, "utf-8")) as Leaderboard;
}

function formatDraws(draws: DrawResult[]): string {
  if (draws.length === 0) return "No draws recorded yet.";
  return draws
    .map((d) => `Episode ${d.drawNumber} (${d.date}): [${d.numbers.join(", ")}]`)
    .join("\n");
}

function formatAgentHistory(agentId: string, episodeResults: EpisodeResult[]): string {
  const history = episodeResults
    .flatMap((e) => e.scores.filter((s) => s.prediction.agentId === agentId))
    .map(
      (s) =>
        `Episode ${s.draw.drawNumber}: picked [${s.prediction.numbers.join(", ")}], matched ${s.matches}, scored ${s.points} pts (confidence: ${s.prediction.confidence}, strategy: ${s.prediction.strategyName})`,
    );
  if (history.length === 0) return "No previous predictions recorded.";
  return history.join("\n");
}

function formatLeaderboard(leaderboard: Leaderboard): string {
  if (leaderboard.entries.length === 0) return "No scores yet.";
  return [...leaderboard.entries]
    .sort((a, b) => a.rank - b.rank)
    .map((e) => `${e.rank}. ${e.agentName} (${e.agentId}): ${e.totalPoints} pts`)
    .join("\n");
}

function formatLatestEpisode(episodeResults: EpisodeResult[]): string {
  if (episodeResults.length === 0) return "No episodes completed yet.";
  const latest = episodeResults[episodeResults.length - 1];
  const lines = [
    `Episode ${latest.episodeNumber} result:`,
    `Draw: [${latest.drawResult.numbers.join(", ")}]`,
    `Scores:`,
  ];
  for (const s of latest.scores) {
    lines.push(
      `  ${s.prediction.agentId}: picked [${s.prediction.numbers.join(", ")}] → ${s.matches} matches, ${s.points} pts`,
    );
  }
  lines.push(`Reality check: ${latest.realityCheck}`);
  return lines.join("\n");
}

async function thinkForAgent(
  agent: AgentConfig,
  episode: number,
  draws: DrawResult[],
  episodeResults: EpisodeResult[],
  leaderboard: Leaderboard,
): Promise<void> {
  const agentsDir = path.join(__dirname, "agents", agent.id);
  const strategyFile = path.join(
    REPO_ROOT,
    "src",
    "AgentsTheOdds.Domain",
    "Strategies",
    `${agent.strategyClass}.cs`,
  );

  const personality = fs.readFileSync(path.join(agentsDir, "personality.md"), "utf-8").trim();
  const journal = fs.readFileSync(path.join(agentsDir, "journal.md"), "utf-8").trim();
  const currentStrategy = fs.readFileSync(strategyFile, "utf-8").trim();

  const userMessage = `# Agent: ${agent.name} (ID: ${agent.id})

## Personality
${personality}

## Your Journal So Far
${journal}

## Current Strategy Code
\`\`\`csharp
${currentStrategy}
\`\`\`

## Game State for Episode ${episode}

### All Draw History (episodes prior to ${episode})
${formatDraws(draws)}

### Your Previous Predictions & Scores
${formatAgentHistory(agent.id, episodeResults)}

### Current Leaderboard
${formatLeaderboard(leaderboard)}

### Latest Episode Summary
${formatLatestEpisode(episodeResults)}

## Task
Update your strategy for Episode ${episode}. Study the game state and update your code:
- Pick 6 numbers that reflect your personality and analytical approach
- Bake your Reasoning (≤20 words, in your voice) directly into the strategy code
- Name your strategy version to reflect what's changed
- Write a journal entry (2–4 sentences) in your character's voice`;

  console.log(`  Calling Claude for ${agent.name}...`);

  const response = await client.messages.create({
    model: "claude-opus-4-7",
    max_tokens: 8192,
    thinking: { type: "adaptive" },
    system: [
      {
        type: "text",
        text: SYSTEM_PROMPT,
        cache_control: { type: "ephemeral" },
      },
    ],
    tools: [UPDATE_AGENT_TOOL],
    tool_choice: { type: "tool", name: "update_agent" },
    messages: [{ role: "user", content: userMessage }],
  });

  const toolUse = response.content.find((b) => b.type === "tool_use");
  if (!toolUse || toolUse.type !== "tool_use") {
    throw new Error(
      `Expected tool_use response for ${agent.name}, got: ${JSON.stringify(response.content)}`,
    );
  }

  const input = toolUse.input as { strategy_code: string; journal_entry: string };

  fs.writeFileSync(strategyFile, input.strategy_code, "utf-8");
  console.log(`  Updated: ${path.relative(REPO_ROOT, strategyFile)}`);

  const journalFile = path.join(agentsDir, "journal.md");
  const journalContent = fs.readFileSync(journalFile, "utf-8");
  fs.writeFileSync(journalFile, journalContent + `\n## Episode ${episode}\n${input.journal_entry}\n`, "utf-8");
  console.log(`  Journal: ${path.relative(REPO_ROOT, journalFile)}`);
}

async function main(): Promise<void> {
  const args = process.argv.slice(2);
  const episodeFlag = args.indexOf("--episode");
  if (episodeFlag === -1 || !args[episodeFlag + 1]) {
    console.error("Usage: npx tsx think.ts --episode <n>");
    process.exit(1);
  }
  const episode = parseInt(args[episodeFlag + 1], 10);
  if (isNaN(episode) || episode < 1) {
    console.error("Episode must be a positive integer");
    process.exit(1);
  }

  console.log(`Think phase — Episode ${episode}`);

  const agents = loadAgents();
  const draws = loadDraws(episode);
  const episodeResults = loadEpisodeResults(episode);
  const leaderboard = loadLeaderboard();

  console.log(
    `Loaded: ${agents.length} agent(s), ${draws.length} draw(s), ${episodeResults.length} episode result(s)`,
  );

  for (const agent of agents) {
    console.log(`\n[${agent.name}]`);
    await thinkForAgent(agent, episode, draws, episodeResults, leaderboard);
  }

  console.log(
    `\nDone. Run: dotnet build && dotnet run --project src/AgentsTheOdds.Cli -- predict --episode ${episode}`,
  );
}

main().catch((err: unknown) => {
  console.error(err);
  process.exit(1);
});
