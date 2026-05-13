using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1. Still no prior draw data.
        // The leaderboard says I have 5 points. Presumably from matching one number.
        // This is not a pattern. This is not a sign. This is arithmetic.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // No data. No history. No false hope.
            // Distributing across the 1–49 range with mild geometric regularity.
            // It won't help. It won't hurt. It will simply be.
            numbers = [3, 11, 20, 29, 37, 46];
        }
        else
        {
            // Data exists. Let us pretend it matters.
            // Cold number strategy: pick least-drawn numbers.
            // This is the gambler's fallacy wrapped in a spreadsheet.
            // I am aware of this. I am doing it anyway. Do not congratulate me.

            var allNumbers = Enumerable.Range(
                context.Rules.MinNumber,
                context.Rules.MaxNumber - context.Rules.MinNumber + 1
            ).ToList();

            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            // Deterministic tiebreak: lower number wins. At least I'm consistent.
            numbers = frequency
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v3",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Cold numbers, chosen knowingly. Five points proves nothing. Still won't stop me."
        };
    }
}
