using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 6. Five data points. Five episodes of principled futility.
        // My scores: 1, 1, 0, 1, 0. Total: 3 pts. Average: 0.6 pts/episode.
        // The Statistician overtook me at 4 pts. The Monkey leads at 10 pts.
        // I have zero conclusions to draw from this that aren't already obvious to anyone
        // who understands variance. The Monkey got lucky early and will regress.
        // I am already regressed and have nowhere further to fall. Comforting, in a bleak way.
        //
        // Draw history analysis:
        // Episode 1: [5,29,37,40,43,49]
        // Episode 2: [2,13,27,43,45,49]
        // Episode 3: [13,19,29,36,38,48]
        // Episode 4: [4,19,20,34,37,42]
        // Episode 5: [20,23,27,35,43,45]
        //
        // Drawn 3 times: 43
        // Drawn 2 times: 13, 19, 20, 27, 29, 37, 45, 49
        // Drawn 0 times (cold): everything else
        //
        // My last two episodes: 0 matches each. Both times I picked low-to-mid cold numbers.
        // The draws have skewed noticeably toward mid-to-high numbers (20-49 range dominates).
        // This observation is statistically meaningless with n=5, but I'm going to act on it
        // anyway by biasing cold-number selection toward the upper half of the range.
        // Not because it works. Because sitting in the same cold-number cave while draws
        // walk around in the 20-49 corridor has produced 0 pts twice in a row, and I
        // refuse to be boring AND wrong simultaneously.
        //
        // Strategy: pick cold numbers, but weight toward higher values using frequency +
        // a positional bias term. Seed remains a function of total points + draw count.

        var allNumbers = Enumerable.Range(
            context.Rules.MinNumber,
            context.Rules.MaxNumber - context.Rules.MinNumber + 1
        ).ToList();

        List<int> numbers;

        if (context.DrawHistory.Count == 0)
        {
            numbers = [3, 11, 20, 29, 37, 46];
        }
        else
        {
            // Build frequency map
            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            int maxNum = context.Rules.MaxNumber;
            int minNum = context.Rules.MinNumber;
            int range  = maxNum - minNum;

            int totalPoints = context.AgentHistory.Sum(r => r.Points);
            var rng = new Random(totalPoints * 7919 + context.DrawHistory.Count * 9973 + 42);

            // Sort by frequency ascending (cold first), then by a bias that nudges
            // toward the upper half of the number range (normalized position 0..1),
            // combined with a seeded random jitter to avoid locking into the same picks.
            numbers = frequency
                .OrderBy(kv => kv.Value)                               // cold numbers first
                .ThenBy(kv => -(kv.Key - minNum) / (double)range)     // prefer higher numbers among ties
                .ThenBy(_ => rng.NextDouble())                         // jitter within clusters
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v8",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Cold numbers, upper-range bias. Still won't work. At least it's different."
        };
    }
}
