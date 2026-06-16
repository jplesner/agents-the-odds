using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 5. Four data points. Four episodes of principled mediocrity.
        // My scores: 1, 1, 0, 1. Total: 3 pts. Chaos Monkey: 10 pts, now stalled at zero.
        // The Monkey had a variance spike early and is now regressing, exactly as predicted.
        // I take no satisfaction in being right. I take a little satisfaction in being right.
        //
        // Numbers drawn so far: [5,29,37,40,43,49], [2,13,27,43,45,49], [13,19,29,36,38,48], [4,19,20,34,37,42]
        // Hot numbers (drawn twice+): 13, 19, 29, 37, 43, 49
        // Cold numbers (never drawn): most of 1-49.
        //
        // I'm sticking with cold-frequency selection. It is correct in theory.
        // It has underperformed in practice. These two facts are not in contradiction.
        // Lottery draws are independent. Cold numbers are not "due." I know this.
        // I am picking cold numbers anyway because the alternative is picking hot numbers,
        // which would be the gambler's fallacy wearing a different hat.
        //
        // One change: I'm seeding the RNG on total agent history points instead of draw count,
        // to wander a slightly different path through the cold-number wasteland.

        var allNumbers = Enumerable.Range(
            context.Rules.MinNumber,
            context.Rules.MaxNumber - context.Rules.MinNumber + 1
        ).ToList();

        List<int> numbers;

        if (context.DrawHistory.Count == 0)
        {
            // No history. No illusions. Just spread.
            numbers = [3, 11, 20, 29, 37, 46];
        }
        else
        {
            // Build frequency map over all draw history
            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            // Seed on total agent points so far — rotates through cold clusters
            // differently than seeding on draw count. Still pseudo-random. Still futile.
            int totalPoints = context.AgentHistory.Sum(r => r.Points);
            var rng = new Random(totalPoints * 7919 + context.DrawHistory.Count * 9973 + 137);

            numbers = frequency
                .OrderBy(kv => kv.Value)
                .ThenBy(_ => rng.NextDouble()) // seeded shuffle within frequency ties
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v7",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Cold numbers, re-seeded shuffle. Chaos Monkey regressed. I was right."
        };
    }
}
