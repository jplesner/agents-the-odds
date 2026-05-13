# Agents the Odds

**Live site: [odds.plesner.ca](https://odds.plesner.ca)**

Agents the Odds is a playful multi-agent experiment where AI contestants repeatedly try to predict lottery-style random draws by maintaining and evolving their own prediction strategies over time.

Each agent has a distinct personality — cautious statistician, chaotic pattern-seeker, skeptic, mystic, chaos monkey, and a dog — that affects how it interprets past results, what patterns it pays attention to, how confident it is, and how aggressively it changes strategy after success or failure.

The project is intentionally built around a process that is not meaningfully predictable. The goal is not to find an algorithm that predicts random numbers. The goal is to watch how different agents behave when faced with an impossible task: whether they overfit to noise, become more cautious, invent patterns, lower their confidence, or stubbornly double down after failure.

## What this showcases

- **Agent-controlled code changes** — Claude rewrites each agent's C# strategy file from scratch each episode, guided by the agent's personality, journal, and past performance. The compiled code is the source of truth.
- **Structured tool use** — the think phase forces a structured `update_agent` tool response, separating strategy code from journal narrative.
- **Scheduled automation** — a GitHub Actions workflow runs the full episode pipeline every Tuesday, commits results to a branch, and opens a PR. The site goes live when the PR is merged.
- **Static site from data files** — an Astro 6 site reads episode JSON at build time, no backend required. Merging the PR triggers a Cloudflare Pages deploy.
- **Disciplined evaluation** — immutable predictions, consistent scoring, per-agent history, and leaderboard tracking across episodes.

## How it works

Each episode runs in four steps:

```
think → predict → draw → score
```

| Step | Command | What happens |
|------|---------|--------------|
| **think** | `npm run think -- --episode <n>` | Claude rewrites each agent's C# strategy file and appends a journal entry |
| **predict** | `dotnet run ... -- predict --episode <n>` | Runs the (just-compiled) strategies to lock in predictions |
| **draw** | `dotnet run ... -- draw --episode <n>` | Generates 6 random winning numbers |
| **score** | `dotnet run ... -- score --episode <n>` | Scores predictions, updates leaderboard, writes recap |
| **show** | `dotnet run ... -- show --episode <n>` | Displays episode results in the console |

## Automation

A GitHub Actions scheduled workflow runs the full pipeline every Tuesday at 7am ET:

1. Detects the next episode number from existing branches
2. Runs think → predict → draw → score on a fresh `episode/N` branch
3. Commits all results (strategy snapshots, episode JSON, leaderboard)
4. Opens a PR with the episode recap as the body

Merging the PR publishes the results to the live site. The workflow can also be triggered manually via `workflow_dispatch`.

## The agents

| Agent | Personality |
|-------|-------------|
| **The Statistician** | Dry, evidence-minded. Trusts frequency analysis, historical distributions, and calibrated confidence. |
| **The Pattern Goblin** | Sees patterns everywhere. Speaks in spirals and resonance. Possibly unhinged. |
| **The Skeptic** | Knows the process is random. Picks numbers anyway out of dim obligation. Smug about it. |
| **The Chaos Monkey** | Randomness maximalist. Intentionally mutates its own strategy each round to explore variance. Controlled chaos, not sabotage. |
| **Dog** | A very good boy. Simple heuristics, treat-based reasoning, and the occasional lucky sniff. |
| **The Mystic** | Numerology, moon phases, vibes, and energetic alignment. Absurd, theatrical, and weirdly serene. |

Each agent has a `personality.md` that stays fixed and a `journal.md` that grows each episode. Their C# strategy files are overwritten by Claude during the think phase.

## Scoring

| Matches | Points |
|---------|--------|
| 6 | 1000 |
| 5 | 100 |
| 4 | 50 |
| 3 | 10 |
| 2 | 5 |
| 1 | 1 |

## Web UI

A static Astro 6 site reads episode data from `data/` at build time and renders:

- **Home** — latest episode results, leaderboard, collapsible "How it works"
- **Episodes** — full history with scores, strategy code, draw results, and reality checks
- **Agents** — per-agent profiles with personality, stats, and per-episode journal entries

```bash
cd web
npm install
npm run dev      # http://localhost:4321
npm run build
```

## Setup

**Prerequisites:** .NET 10, Node.js 20+, an Anthropic API key.

```bash
# Install script dependencies
cd scripts && npm install

# Add your API key
echo "ANTHROPIC_API_KEY=sk-ant-..." > scripts/.env
```

## Running an episode

```bash
# 1. Agents rewrite their strategies
cd scripts && npm run think -- --episode 1

# 2. Compile updated strategies
dotnet build

# 3. Lock in predictions
dotnet run --project src/AgentsTheOdds.Cli -- predict --episode 1

# 4. Generate the draw
dotnet run --project src/AgentsTheOdds.Cli -- draw --episode 1

# 5. Score and update leaderboard
dotnet run --project src/AgentsTheOdds.Cli -- score --episode 1

# 6. View results
dotnet run --project src/AgentsTheOdds.Cli -- show --episode 1
```

## Tests

```bash
dotnet test
```
