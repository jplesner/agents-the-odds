# Agents the Odds

Agents the Odds is a playful multi-agent experiment where AI contestants repeatedly try to predict lottery-style random draws by maintaining and evolving their own prediction strategies over time.  

Each agent has a distinct personality, such as cautious statistician, chaotic pattern-seeker, skeptic, mystic, contrarian, or engineer. That personality affects how the agent interprets past results, what kinds of patterns it pays attention to, how confident it is, and how aggressively it changes strategy after success or failure.  

The agents do not simply submit numbers from a fixed prompt. Each agent has its own strategy implementation, prompt, or constrained algorithm file. The system fetches the latest draw result, scores the previous predictions, updates each agent’s history, and then gives each agent a chance to revise its own prediction strategy for the next round. For example, the Pattern Goblin might double down on meaningless clusters after a lucky week, while the Skeptic might remain close to random selection, and the Statistician might adjust frequency weights while lowering confidence.  

The project is intentionally built around a process that is not meaningfully predictable. The goal is not to somehow come up with an algorithm that predicts random numbers. The goal is to watch how different agents behave when faced with an impossible task: whether they overfit to noise, become more cautious, invent patterns, lower their confidence, copy successful approaches, or stubbornly double down after failure.  

Under the game-show surface, the project is an automated evaluation harness. It stores immutable predictions, fetches real outcomes, applies consistent scoring, compares agents against random baselines, tracks confidence calibration, and records how each agent’s strategy evolves over time. The result is part game show, part statistics lesson, and part practical demo of multi-agent orchestration, scheduled automation, prompt design, agent-controlled code changes, guardrails, and disciplined AI evaluation.


## How it works

Each episode runs in four steps:

```
think → predict → draw → score
```

| Step | Command | What happens |
|------|---------|--------------|
| **think** | `npm run think -- --episode <n>` | Claude rewrites each agent's C# strategy file and journal entry |
| **predict** | `dotnet run ... -- predict --episode <n>` | Runs the (just-compiled) strategies to lock in predictions |
| **draw** | `dotnet run ... -- draw --episode <n>` | Generates 6 random winning numbers |
| **score** | `dotnet run ... -- score --episode <n>` | Scores predictions, updates leaderboard, writes recap |
| **show** | `dotnet run ... -- show --episode <n>` | Displays episode results in the console |

## The agents

| Agent | Personality |
|-------|-------------|
| **The Statistician** | Dry, evidence-minded. Trusts frequency analysis, historical distributions, and calibrated confidence. |
| **The Pattern Goblin** | Sees patterns everywhere. Speaks in spirals and resonance. Possibly unhinged. |
| **The Skeptic** | Knows the process is random. Picks numbers anyway out of dim obligation. Smug about it. |
| **The Chaos Monkey** | Randomness maximalist. Intentionally mutates its own strategy each round to explore variance. Controlled chaos, not sabotage. |
| **Dog** | A very good boy. Simple heuristics, treat-based reasoning, and the occasional lucky sniff. |
| **The Mystic** | Numerology, moon phases, vibes, and energetic alignment. Absurd, theatrical, and weirdly serene. |

Each agent has a `personality.md` that stays fixed and a `journal.md` that grows each episode. Their C# strategy files are overwritten by Claude during the think phase — the compiled code is the source of truth for what numbers they pick.

## Scoring

| Matches | Points |
|---------|--------|
| 6 | 1000 |
| 5 | 100 |
| 4 | 50 |
| 3 | 10 |
| 2 | 5 |
| 1 | 1 |

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
