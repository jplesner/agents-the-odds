# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run the CLI
dotnet run --project src/AgentsTheOdds.Cli

# Run all tests
dotnet test

# Run a single test class
dotnet test --filter "FullyQualifiedName~ScorerTests"

# Run a single test method
dotnet test --filter "FullyQualifiedName~ScorerTests.Score_SixMatches_Returns1000Points"
```

## Architecture

Four projects under `src/`, one test project under `tests/`:

- **`AgentsTheOdds.Domain`** — models, repository interfaces, `IPredictionStrategy`, `LotteryValidator`, `Scorer`
- **`AgentsTheOdds.Application`** — strategy implementations, `InMemoryAgentRepository`, `GameRunner`
- **`AgentsTheOdds.Data`** — `InMemoryDrawRepository`, `InMemoryPredictionRepository`
- **`AgentsTheOdds.Cli`** — `Program.cs`, Generic Host + DI wiring

Dependency graph: `Cli → Application → Domain`, `Cli → Data → Domain`, `Tests → Domain`.

### Core data flow

`Program.cs` builds a Generic Host with DI, then delegates to `GameRunner.RunAsync()`:

1. `IDrawRepository.GetCurrent()` → current draw; `GetHistory()` → draw history
2. `IAgentRepository.GetAll()` → agents; each calls `agent.Strategy.GeneratePrediction(context)` → `Prediction`
3. Validate via `LotteryValidator.Validate(prediction, rules)` → `ValidationResult`
4. Score via `Scorer.Score(prediction, draw)` → `PredictionResult`, stored via `IPredictionRepository`
5. Sort results and print predictions + leaderboard

### Key design decisions

**`IPredictionStrategy`** is the extension point. Adding a new agent means implementing this interface in `AgentsTheOdds.Application` and registering it in `InMemoryAgentRepository`. Each strategy receives a `PredictionContext` containing rules, full draw history, prior prediction results, and the current leaderboard — so Phase 2 AI strategies can use all of this context.

**`Prediction.Confidence`** is strategy-provided (each strategy hard-codes its own value, `[0.0, 1.0]`). The validator enforces the range. Future phases will compare declared confidence against actual match rate.

**`Agent.Id`** (kebab-case string) is the stable identifier used to link `Prediction` back to `Agent` in `Program.cs`. Strategies embed their own `AgentId` in the returned `Prediction`.

**`Leaderboard.Empty`** is passed as context in Phase 1 since there's no multi-round history yet. `DrawHistory` has 3 sample draws; `CurrentDraw` is always `DrawHistory[^1]`.

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
