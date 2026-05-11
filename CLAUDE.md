# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run the CLI
dotnet run --project src/AgentsTheOdds.Cli -- --help

# Phase 2 workflow
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

- **`AgentsTheOdds.Domain`** — models, interfaces, `LotteryValidator`, `Scorer`, strategy implementations (`PatternGoblinStrategy`, `SkepticStrategy`, `StatisticianStrategy`), domain services (`LeaderboardMerger`, `RealityCheckGenerator`)
- **`AgentsTheOdds.Application`** — `GameRunner`, `RandomBaselineStrategy`, `RandomDrawService`, commands (`DrawCommand`, `PredictCommand`, `ScoreCommand`)
- **`AgentsTheOdds.Data`** — in-memory repositories (`InMemoryDrawRepository`, `InMemoryPredictionRepository`, `InMemoryAgentRepository`), file-based storage under `File/` (`JsonDrawRepository`, `JsonEpisodePredictionRepository`, `JsonEpisodeResultRepository`, `JsonLeaderboardRepository`, `MarkdownRecapWriter`), agent personality files under `Agents/`
- **`AgentsTheOdds.Cli`** — `Program.cs`, `ConsoleGamePresenter`, Generic Host + DI wiring, `System.CommandLine` subcommands

Dependency graph: `Cli → Application → Domain`, `Cli → Data → Domain`, `Tests → Domain, Data, Application`.

### CLI commands

| Command | Description |
|---------|-------------|
| `predict --episode <n> [--force]` | Generate and lock agent predictions |
| `draw --episode <n> [--force]` | Generate and record the draw result |
| `score --episode <n>` | Score predictions against the recorded draw |
| `play` | Phase 1 in-memory simulation |

### File storage

All Phase 2 state lives under `data/` at the repo root (resolved via `DataRootResolver`, overridable with `AGENTS_DATA_ROOT`):

```
data/
  draws/episode-001.json          ← written by draw
  predictions/episode-001.json    ← written by predict
  episodes/episode-001.json       ← written by score
  episodes/episode-001.md         ← written by score
  leaderboard.json                ← written by score
```

### Core data flow (Phase 2)

1. **predict** — loads agents, draw history (from past episode results), leaderboard; calls `agent.Strategy.GeneratePrediction(context)` per agent with that agent's own filtered history in `PredictionContext.AgentHistory`; validates via `LotteryValidator`; saves `EpisodePredictionSet`
2. **draw** — calls `IDrawService.Draw(episode)` → `RandomDrawService` generates 6 random numbers, calls `IDrawRepository.RecordDraw()`
3. **score** — loads locked predictions + draw via `IDrawRepository.GetByEpisode()` (errors if missing); scores via `Scorer.Score()`; merges leaderboard via `LeaderboardMerger.Merge()`; generates `RealityCheck` via `IRealityCheckGenerator`; saves `EpisodeResult` + markdown recap + leaderboard

### Key design decisions

**`IPredictionStrategy`** is the extension point. Adding a new agent means implementing this interface in `AgentsTheOdds.Domain` and registering it in `InMemoryAgentRepository`. Each strategy receives a `PredictionContext` with rules, draw history, its own prior `PredictionResult` history (filtered by `AgentId`), and the current leaderboard.

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
| 0–2     | 0      |

### Lottery rules

6 unique numbers, each in `[1, 49]`. Defined in `LotteryRules.Standard`.

### NuGet

A local `NuGet.config` at the repo root restricts package sources to `nuget.org` only, bypassing any machine-level corporate feeds.
