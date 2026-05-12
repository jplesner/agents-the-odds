using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // No draw history exists. No pattern to exploit. Good. Patterns were never real anyway.
        // Using a deterministic spread across the 1–49 range because it's as valid as anything else.
        // Which is to say: not very valid at all.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // Episode 1: no data. Distribute evenly. Pretend this is methodical.
            numbers = [4, 12, 21, 30, 38, 47];
        }
        else
        {
            // Use draw history to pick least-frequent numbers, because cold numbers are just
            // as random as hot ones, but at least this gives me something to narrate.
            var allNumbers = Enumerable.Range(1, 49).ToList();
            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

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
            StrategyName = "cold-frequency-v1",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Least-frequent numbers. Cold bias. Also meaningless. You're welcome."
        };
    }
}
