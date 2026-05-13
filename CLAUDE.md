# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run the CLI
dotnet run --project src/AgentsTheOdds.Cli -- --help

# Full episode workflow (Phase 3)
cd scripts && npm install          # first time only
npx tsx think.ts --episode 1      # agents update their own strategy code
dotnet build                       # compile updated strategies
dotnet run --project src/AgentsTheOdds.Cli -- predict --episode 1
dotnet run --project src/AgentsTheOdds.Cli -- draw --episode 1
dotnet run --project src/AgentsTheOdds.Cli -- score --episode 1

# Phase 1 in-memory simulation (preserved)
dotnet run --project src/AgentsTheOdds.Cli -- play

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~ScorerTests"

# Run a single test method
dotnet test --filter "FullyQualifiedName~ScorerTests.Score_SixMatches_Returns1000Points"
```

## Architecture

Four projects under `src/`, one test project under `tests/`:

- **`AgentsTheOdds.Domain`** — models, interfaces, `LotteryValidator`, `Scorer`, strategy implementations (`PatternGoblinStrategy`, `SkepticStrategy`, `StatisticianStrategy`, `ChaosMonkeyStrategy`, `DogStrategy`, `MysticStrategy`), domain services (`LeaderboardMerger`, `RealityCheckGenerator`)
- **`AgentsTheOdds.Application`** — `RandomBaselineStrategy`, `RandomDrawService`, commands (`DrawCommand`, `PredictCommand`, `ScoreCommand`, `ShowCommand`)
- **`AgentsTheOdds.Data`** — in-memory repositories (`InMemoryPredictionRepository`, `InMemoryAgentRepository`), file-based storage under `File/` (`JsonDrawRepository`, `JsonEpisodePredictionRepository`, `JsonEpisodeResultRepository`, `JsonLeaderboardRepository`, `MarkdownRecapWriter`)
- **`AgentsTheOdds.Cli`** — `Program.cs`, `ConsoleGamePresenter` (display for `show` command), Generic Host + DI wiring, `System.CommandLine` subcommands
- **`scripts/`** — Node.js/TypeScript tooling for the think phase: `think.ts` (orchestrator), `prompt.ts` (system prompt + tool definition), `types.ts` (re-exports shared interfaces from `web/src/types/data.ts`, plus `AgentConfig`). Agent personality and journal files live under `data/agents/{agent-id}/`.

Dependency graph: `Cli → Application → Domain`, `Cli → Data → Domain`, `Tests → Domain, Data, Application`.

### CLI commands

| Command | Description |
|---------|-------------|
| `predict --episode <n> [--force]` | Generate and lock agent predictions |
| `draw --episode <n> [--force]` | Generate and record the draw result |
| `score --episode <n>` | Score predictions against the recorded draw |
| `show --episode <n>` | Display episode results in the console |
| `agents` | Print all agents as JSON (consumed by `think.ts`) |

### File storage

All Phase 2 state lives under `data/` at the repo root (resolved via `DataRootResolver`, overridable with `AGENTS_DATA_ROOT`):

```
data/
  draws/episode-001.json                        ← written by draw
  predictions/episode-001.json                  ← written by predict
  episodes/episode-001.json                     ← written by score
  episodes/episode-001.md                       ← written by score
  leaderboard.json                              ← written by score
  agents/{id}/strategies/episode-001.cs         ← written by think (strategy snapshot)
```

### Core data flow (Phase 3)

0. **think** (`scripts/think.ts`) — calls `dotnet run ... -- agents` to get the agent list; for each agent reads `data/agents/{id}/personality.md`, `journal.md`, and the current `.cs` strategy file; calls Claude (`claude-sonnet-4-6`) with the game state and forces a structured `update_agent` tool response; writes the new strategy code back to `src/AgentsTheOdds.Domain/Strategies/{Class}.cs`, snapshots it to `data/agents/{id}/strategies/episode-{n}.cs`, and appends a journal entry
1. **predict** — loads agents, draw history (from past episode results), leaderboard; calls `agent.Strategy.GeneratePrediction(context)` per agent with that agent's own filtered history in `PredictionContext.AgentHistory`; validates via `LotteryValidator`; saves `EpisodePredictionSet`
2. **draw** — calls `IDrawService.Draw(episode)` → `RandomDrawService` generates 6 random numbers, calls `IDrawRepository.RecordDraw()`
3. **score** — loads locked predictions + draw via `IDrawRepository.GetByEpisode()` (errors if missing); scores via `Scorer.Score()`; merges leaderboard via `LeaderboardMerger.Merge()`; generates `RealityCheck` via `IRealityCheckGenerator`; saves `EpisodeResult` + markdown recap + leaderboard

### Key design decisions

**`IPredictionStrategy`** is the extension point. Adding a new agent means implementing this interface in `AgentsTheOdds.Domain`, registering it in `InMemoryAgentRepository`, and adding a `data/agents/{id}/` folder with `personality.md`, `journal.md`, `avatar.png`, and an empty `strategies/` directory. Each strategy receives a `PredictionContext` with rules, draw history, its own prior `PredictionResult` history (filtered by `AgentId`), and the current leaderboard.

**Think phase** — `think.ts` calls `dotnet run ... -- agents` at startup to get the authoritative agent list (id, name, strategyClass). The `.cs` strategy files are the source of truth for compiled behaviour; `think.ts` overwrites them, snapshots the code to `data/agents/{id}/strategies/episode-{n}.cs`, and then `dotnet build` recompiles before `predict` runs. Agent personalities and journals live in `data/agents/{id}/` and are never read by the .NET code.

**`IDrawService`** owns draw generation logic (currently random). `IDrawRepository` is pure CRUD storage. Swapping in a real lottery API means implementing a new `IDrawService` only.

**`IDrawRepository.GetByEpisode()`** throws `InvalidOperationException` if the draw hasn't been recorded yet — `score` fails clearly with an actionable message.

**`Prediction.Confidence`** is strategy-provided (`[0.0, 1.0]`). The validator enforces the range.

**`Agent.Id`** (kebab-case string) is the stable identifier linking `Prediction` → `Agent`.

### Scoring table

| Matches | Points |
|---------|--------|
| 6       | 1000   |
| 5       | 100    |
| 4       | 50     |
| 3       | 10     |
| 2       | 5      |
| 1       | 1      |

### Lottery rules

6 unique numbers, each in `[1, 49]`. Defined in `LotteryRules.Standard`.

### NuGet

A local `NuGet.config` at the repo root restricts package sources to `nuget.org` only, bypassing any machine-level corporate feeds.

## Web UI (`web/`)

Static read-only Astro 6 site that reads episode data from `../data/` at build time. No backend, no API.

### Commands

```bash
cd web
npm install          # first time only
npm run dev          # dev server at http://localhost:4321
npm run build        # emit to web/dist/
npm run preview      # serve web/dist/ locally
npx astro check      # TypeScript check
```

### Pages

| Route | Description |
|-------|-------------|
| `/` | Home: leaderboard + latest episode summary |
| `/episodes` | All episodes list with date, draw, winner |
| `/episodes/[n]` | Episode detail: draw, full scores table, standings, reality check |

### Architecture

- **`src/types/data.ts`** — TypeScript interfaces for all JSON data shapes. **Source of truth shared with `scripts/types.ts`**, which re-exports from here.
- **`src/lib/data.ts`** — Build-time helpers (`readLeaderboard`, `readAllEpisodes`, `readEpisode`, `getEpisodeNumbers`, `readAgentProfiles`, `readEpisodeStrategyCodes`). Uses synchronous `node:fs`. All I/O errors swallowed — missing files produce empty/null gracefully so the site builds even with no episode data.
- **`src/layouts/Layout.astro`** — Shared HTML shell with nav. Imports `src/styles/global.css`.
- **`src/styles/global.css`** — Tailwind v4 config via `@theme` (custom colors, Roboto font), base element styles, and `.data-table` component class.

### Styling

Matches [plesner.ca](https://www.plesner.ca/) — dark navy (`#111821`) background, `rgba(255,255,255,0.7)` body text, Roboto font, `#0d6efd` blue accent. Uses Tailwind v4 (`@tailwindcss/vite`). Custom theme properties defined in `global.css` under `@theme`.

### Data path

`src/lib/data.ts` resolves data via `path.resolve(process.cwd(), '..', 'data')`. Always run Astro commands from the `web/` directory.
