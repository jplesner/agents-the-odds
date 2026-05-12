using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1. No history. No patterns. No false hope. Perfect conditions.
        // I'll use a deterministic spread for now, and transition to cold-frequency
        // once there's actual data to pretend to analyze.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // No draw history: distribute evenly across 1–49.
            // This is no worse than any other method. That's the point.
            numbers = [4, 12, 21, 30, 38, 47];
        }
        else
        {
            // Track how often each number has appeared across all past draws.
            // Select the 6 least-frequent ones. "Cold" numbers. Completely arbitrary.
            // But arbitrary with structure sounds better at dinner parties.
            var frequency = Enumerable.Range(1, 49).ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            // Tiebreak by number value (ascending) for determinism.
            // Determinism within chaos. How poetic. How futile.
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
            Reasoning    = "Evenly spaced numbers. No history. No illusions. You're welcome."
        };
    }
}
