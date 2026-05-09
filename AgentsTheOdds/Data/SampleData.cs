using AgentsTheOdds.Models;
using AgentsTheOdds.Strategies;

namespace AgentsTheOdds.Data;

public static class SampleData
{
    public static readonly LotteryRules Rules = LotteryRules.Standard;

    // Draw 3 is the current draw: [7, 14, 21, 28, 35, 42]
    // Engineered so agents score meaningfully:
    //   Statistician  [7, 14, 21, 28, 35, 44] → 5 matches → 100 pts
    //   Pattern Goblin [3, 14, 21, 28, 42, 47] → 4 matches → 50 pts
    //   Skeptic        [1,  9, 22, 29, 36, 43] → 0 matches → 0 pts
    //   Random Baseline                        → varies
    public static readonly IReadOnlyList<DrawResult> DrawHistory =
    [
        new DrawResult { DrawNumber = 1, Date = new DateOnly(2025, 1,  4), Numbers = [3, 17, 22, 34, 41, 48] },
        new DrawResult { DrawNumber = 2, Date = new DateOnly(2025, 1, 11), Numbers = [5, 11, 25, 31, 38, 46] },
        new DrawResult { DrawNumber = 3, Date = new DateOnly(2025, 1, 18), Numbers = [7, 14, 21, 28, 35, 42] },
    ];

    public static DrawResult CurrentDraw => DrawHistory[^1];

    public static IReadOnlyList<Agent> Agents =>
    [
        new Agent
        {
            Id          = "statistician",
            Name        = "The Statistician",
            Personality = "Calm, data-driven, trusts the numbers above all else.",
            Strategy    = new StatisticianStrategy()
        },
        new Agent
        {
            Id          = "pattern-goblin",
            Name        = "The Pattern Goblin",
            Personality = "Sees patterns everywhere. Possibly unhinged.",
            Strategy    = new PatternGoblinStrategy()
        },
        new Agent
        {
            Id          = "skeptic",
            Name        = "The Skeptic",
            Personality = "Doubts everything, including itself.",
            Strategy    = new SkepticStrategy()
        },
        new Agent
        {
            Id          = "random-baseline",
            Name        = "Random Baseline",
            Personality = "No personality. Pure chaos.",
            Strategy    = new RandomBaselineStrategy()
        },
    ];
}
