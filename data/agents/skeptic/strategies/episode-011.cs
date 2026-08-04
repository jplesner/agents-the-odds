using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 11. Ten data points. My scores: 1,1,0,1,0,0,10,0,1,1. Total: 15 pts.
        // Still in first place, now by 2 points over Chaos Monkey at 13.
        // Episode 10: I picked [26, 6, 44, 46, 28, 21]. Draw was [13, 30, 36, 38, 42, 46].
        // Matched 46. One point. Cold pool delivered exactly what a cold pool delivers.
        //
        // Full draw history (10 episodes):
        // Episode 1:  [5,  29, 37, 40, 43, 49]
        // Episode 2:  [2,  13, 27, 43, 45, 49]
        // Episode 3:  [13, 19, 29, 36, 38, 48]
        // Episode 4:  [4,  19, 20, 34, 37, 42]
        // Episode 5:  [20, 23, 27, 35, 43, 45]
        // Episode 6:  [17, 25, 31, 32, 42, 48]
        // Episode 7:  [4,  8,  15, 19, 20, 47]
        // Episode 8:  [5,  7,  25, 30, 33, 43]
        // Episode 9:  [3,  14, 16, 34, 39, 42]
        // Episode 10: [13, 30, 36, 38, 42, 46]
        //
        // Updated hot numbers (drawn ≥2x):
        //   43(4x), 20(3x), 19(3x), 42(4x), 13(3x), 27(2x), 29(2x), 37(2x), 49(2x),
        //   45(2x), 48(2x), 4(2x), 25(2x), 5(2x), 34(2x), 36(2x), 38(2x), 30(2x)
        // Episode 10 newly drawn (1x): 46 (was cold, now warm)
        // Numbers still at 0x (pure cold): 1,6,9,10,11,12,18,21,22,24,26,28,41,44
        //   Wait — need to recheck against all draws...
        //   Drawn at least once: 2,3,4,5,7,8,13,14,15,16,17,19,20,23,25,27,29,30,31,
        //     32,33,34,35,36,37,38,39,40,42,43,45,46,47,48,49
        //   Remaining cold (0x): 1,6,9,10,11,12,18,21,22,24,26,28,41,44
        //   (Only 14 numbers remain at 0x after 10 episodes.)
        //
        // Strategy: unchanged. Cold-frequency with seeded jitter.
        // Seed: totalPoints * prime1 + episodeCount * prime2 + offset
        // totalPoints=15, episodeCount=10. Fresh rotation.
        //
        // The cold pool is shrinking (14 numbers at 0x). This means the
        // "cold-first" approach is increasingly pulling from a consistent
        // set — which makes it more deterministic, not less. I note this
        // with academic interest and will continue doing it anyway.
        //
        // Strategy: cold-frequency-v13
        // (v13: the unlucky version. I do not believe in luck. I do note the irony.)

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
            // totalPoints=15, episodeCount=10 → new rotation through cold pool
            // The jitter is the entire strategy. I remain at peace with this.
            var rng = new Random(totalPoints * 6271 + episodeCount * 8191 + 137);

            // Pure cold-number selection: sort ascending by draw frequency, random tiebreak.
            // No hot-chasing. No directional bias. No hope. Just cold numbers and arithmetic.
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
            StrategyName = "cold-frequency-v13",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Still first. Two-point lead. Cold pool shrinking. Changing nothing, obviously."
        };
    }
}
