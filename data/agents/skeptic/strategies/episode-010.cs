using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 10. Nine data points. My scores: 1,1,0,1,0,0,10,0,1. Total: 14 pts.
        // Still in first place, by one point, over Chaos Monkey at 13.
        // Episode 9: I picked [10, 16, 41, 11, 12, 26]. Draw was [3, 14, 16, 34, 39, 42].
        // Matched 16. One point. The cold-number pool generously handed me a 1x-drawn number.
        //
        // Full draw history (9 episodes):
        // Episode 1: [5,  29, 37, 40, 43, 49]
        // Episode 2: [2,  13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4,  19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        // Episode 6: [17, 25, 31, 32, 42, 48]
        // Episode 7: [4,  8,  15, 19, 20, 47]
        // Episode 8: [5,  7,  25, 30, 33, 43]
        // Episode 9: [3,  14, 16, 34, 39, 42]
        //
        // Updated frequency (drawn ≥2x): 43(4x), 20(3x), 19(3x), 42(3x), 13(2x), 27(2x),
        //   29(2x), 37(2x), 49(2x), 45(2x), 48(2x), 4(2x), 25(2x), 5(2x), 34(2x)
        // Cold (0x after ep9): 1, 6, 9, 10, 11, 12, 18, 21, 22, 24, 26, 28, 41, 44, 46
        //   Wait — 3,14,16,39 are newly drawn in ep9 (1x now). 10,11,12,26,41 still cold.
        //   16 matched me in ep9 — it's now warm(1x). Still, I pick from 0x pool.
        //
        // Strategy: unchanged. Cold-frequency with seeded jitter.
        // Seed: totalPoints * prime1 + episodeCount * prime2 + offset
        // totalPoints=14, episodeCount=9. New seed, fresh rotation.
        // The lead is 1 point. I am not changing anything for 1 point.
        // Changing strategy because of a 1-point lead would be the kind of
        // superstitious nonsense I exist to oppose. Cold numbers it is.
        //
        // Strategy: cold-frequency-v12
        // (v12: twelve versions. Still first. Still variance. I have noted both facts.)

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
            // Build frequency map from actual draw history
            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            int totalPoints  = context.AgentHistory.Sum(r => r.Points);
            int episodeCount = context.DrawHistory.Count;

            // Seed: totalPoints * prime1 + episodeCount * prime2 + offset
            // totalPoints=14, episodeCount=9 → new rotation through cold pool
            // The jitter is the entire strategy. I remain at peace with this.
            var rng = new Random(totalPoints * 6271 + episodeCount * 8191 + 137);

            // Pure cold-number selection: sort ascending by draw frequency, random tiebreak.
            // No hot-chasing. No directional bias. No hope.
            numbers = frequency
                .OrderBy(kv => kv.Value)            // coldest first
                .ThenBy(_ => rng.NextDouble())       // random tiebreak within frequency bands
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v12",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Still first. Still variance. Changing nothing. Regression is merely delayed."
        };
    }
}
