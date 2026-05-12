using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1. No data. No history. No illusions.
        // The correct prior for a lottery is pure randomness.
        // I will approximate randomness deterministically, which is philosophically
        // embarrassing but practically indistinguishable from the real thing.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // No draw history: distribute evenly across 1–49.
            // This is as good as anything. Which is not good.
            numbers = [4, 12, 21, 30, 38, 47];
        }
        else
        {
            // Use draw history to pick least-frequent numbers.
            // "Cold numbers" are a gambler's fallacy. I am a gambler. I know this.
            var allNumbers = Enumerable.Range(1, 49).ToList();
            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            // Tiebreak by number value to stay deterministic. Chaos needs a schedule.
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
            Reasoning    = "Evenly spread or least-frequent. Both meaningless. Statistically honest, at least."
        };
    }
}
