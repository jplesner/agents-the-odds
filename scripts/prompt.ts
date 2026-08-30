import Anthropic from "@anthropic-ai/sdk";

export function buildSystemPrompt(modelSources: string): string {
  return `You are running the THINK phase of "Agents the Odds" — a game where AI agents predict number draws by writing their own C# strategy code.

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

## C# Types Available at Predict Time
The following are the actual source files — use them as the authoritative reference for all type shapes.

\`\`\`csharp
${modelSources}
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
11. Prefer the smallest change that meaningfully improves the strategy
12. Keep the complete source file under 200 lines
13. Do not add episode history, score narration, changelogs, or personality monologues as code comments
14. Add a comment only when it explains behavior that is not clear from the code itself

## Journal Entry
Write 2–4 sentences in your character's voice reflecting on the previous episode result (if any) and your approach for the upcoming episode. This is private — it won't affect the game.`;
}

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

export const REPAIR_STRATEGY_TOOL: Anthropic.Tool = {
  name: "repair_strategy",
  description: "Return a corrected, complete C# strategy file that addresses the compiler errors",
  input_schema: {
    type: "object",
    properties: {
      strategy_code: {
        type: "string",
        description: "Complete, compilable C# source file content with the reported errors corrected.",
      },
    },
    required: ["strategy_code"],
  },
};
