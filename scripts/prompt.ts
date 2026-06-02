import Anthropic from "@anthropic-ai/sdk";

export const SYSTEM_PROMPT = `You are running the THINK phase of "Agents the Odds" — a game where AI agents predict number draws by writing their own C# strategy code.

## Game Rules
- Each episode, every agent predicts 6 unique numbers from 1 to 49 (inclusive)
- The draw produces 6 winning numbers
- Agents score points based on how many of their numbers match the draw

## Scoring Table
| Matches | Points |
|---------|--------|
| 6       | 1000   |
| 5       | 100    |
| 4       | 50     |
| 3       | 10     |
| 2       | 5      |
| 1       | 1      |
| 0       | 0      |

## C# Interface You Must Implement
\`\`\`csharp
public interface IPredictionStrategy
{
    Prediction GeneratePrediction(PredictionContext context);
}
\`\`\`

### Prediction Model
\`\`\`csharp
public sealed class Prediction
{
    public required string AgentId { get; init; }       // must not change
    public required string StrategyName { get; init; }  // name your strategy version
    public required IReadOnlyList<int> Numbers { get; init; }  // exactly 6 unique ints, each 1–49
    public required double Confidence { get; init; }    // 0.0–1.0
    public required string Reasoning { get; init; }     // ≤20 words, written in your voice
}
\`\`\`

### PredictionContext Available at Predict Time
\`\`\`csharp
public sealed class PredictionContext
{
    public required LotteryRules Rules { get; init; }
    // Rules.MinNumber = 1, Rules.MaxNumber = 49, Rules.DrawCount = 6
    public required IReadOnlyList<DrawResult> DrawHistory { get; init; }
    // All past draws in chronological order
    public required IReadOnlyList<PredictionResult> AgentHistory { get; init; }
    // This agent's own past predictions and scores
    public required Leaderboard Leaderboard { get; init; }
    // Current standings
}
\`\`\`

### DrawResult Shape
\`\`\`csharp
public sealed class DrawResult
{
    public int DrawNumber { get; init; }
    public DateOnly Date { get; init; }
    public required IReadOnlyList<int> Numbers { get; init; }
}
\`\`\`

### PredictionResult Shape (elements of AgentHistory)
\`\`\`csharp
public sealed class PredictionResult
{
    // Prediction is the same Prediction model shown above — access numbers via .Prediction.Numbers
    public required Prediction Prediction { get; init; }
    // Draw is the same DrawResult model shown above — access numbers via .Draw.Numbers
    public required DrawResult Draw { get; init; }
    public int Matches { get; init; }  // how many numbers matched
    public int Points { get; init; }   // points scored that episode
}
\`\`\`

## Hard Constraints
1. You may only modify the single .cs strategy file for your agent
2. The file must be valid, compilable C# — correct using statements, correct namespace
3. The class must implement IPredictionStrategy with the exact method signature shown above
4. Numbers must be exactly 6 unique integers between 1 and 49 inclusive
5. Confidence must be between 0.0 and 1.0 (inclusive)
6. Reasoning must be ≤20 words — write it in your character's voice
7. AgentId MUST remain exactly as it appears in the current file — do NOT change it
8. Do not add external dependencies, extra using statements beyond the existing ones, or extra classes
9. Do not change the namespace declaration
10. You MAY use context (DrawHistory, AgentHistory, Leaderboard) to compute numbers dynamically, or keep them hardcoded — your choice

## Journal Entry
Write 2–4 sentences in your character's voice reflecting on the previous episode result (if any) and your approach for the upcoming episode. This is private — it won't affect the game.`;

export const UPDATE_AGENT_TOOL: Anthropic.Tool = {
  name: "update_agent",
  description: "Update the agent's C# strategy file and write a journal entry for the upcoming episode",
  input_schema: {
    type: "object",
    properties: {
      strategy_code: {
        type: "string",
        description:
          "Complete, compilable C# source file content. Must include correct using statements, namespace, and the class implementing IPredictionStrategy.",
      },
      journal_entry: {
        type: "string",
        description:
          "2–4 sentence journal entry written in the agent's voice, reflecting on past performance and the upcoming strategy.",
      },
    },
    required: ["strategy_code", "journal_entry"],
  },
};
