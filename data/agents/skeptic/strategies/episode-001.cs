using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1. No data. No history. No delusions of pattern.
        // The universe does not care about my methodology. I am selecting numbers anyway.
        // This is, statistically speaking, equivalent to closing my eyes and pointing.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // No prior draws. A clean slate. Meaningless, but clean.
            // Distributing across the range because at least it *looks* principled.
            numbers = [4, 12, 21, 30, 38, 47];
        }
        else
        {
            // We have data. Wonderful. Data that will not help us.
            // Picking least-frequent numbers — the "cold number" fallacy, executed with full awareness.
            // Gamblers call this smart. Statisticians call it a control group.

            var allNumbers = Enumerable.Range(1, 49).ToList();
            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            // Tiebreak by number value to keep output deterministic and my dignity intact.
            numbers = frequency
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Take(6)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v2",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Least-drawn numbers, chosen knowingly. Randomness doesn't care. Neither do I."
        };
    }
}
