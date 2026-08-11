using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 12. Eleven data points. My scores: 1,1,0,1,0,0,10,0,1,1,1. Total: 16 pts.
        // I am now in SECOND place. Chaos Monkey leapt to 18 by matching 6 and 33 last episode.
        // Pattern Goblin is at 15 — breathing down my neck. The lead I had is gone.
        // This is, of course, variance. I am not upset. I am noting it with the detachment
        // of a pathologist reading a report about someone else's organs.
        //
        // Full draw history (11 episodes):
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
        // Episode 11: [6,  15, 33, 36, 44, 49]
        //
        // Updated hot numbers (drawn ≥2x):
        //   43(4x), 42(5x!), 20(3x), 19(3x), 13(3x), 36(3x), 49(3x), 15(2x),
        //   27(2x), 29(2x), 37(2x), 45(2x), 48(2x), 4(2x), 25(2x), 5(2x),
        //   34(2x), 38(2x), 30(2x), 33(2x)
        //
        // Episode 11 newly drawn (now warm): 6, 15(was 1x→2x), 33(was 1x→2x), 44(was 0x→1x), 49(was 2x→3x)
        //
        // Numbers still at 0x (pure cold) after 11 draws:
        //   Need to compute from full history:
        //   Drawn at least once: 2,3,4,5,6,7,8,13,14,15,16,17,19,20,23,25,27,29,30,31,
        //     32,33,34,35,36,37,38,39,40,42,43,44,45,46,47,48,49
        //   Remaining cold (0x): 1,9,10,11,12,18,21,22,24,26,28,41
        //   (12 numbers at 0x. Pool shrinking further.)
        //
        // Strategy: unchanged. Cold-frequency with seeded jitter.
        // I am in second place by two points and I am changing nothing,
        // because reacting to two points of deficit would be embarrassing.
        // The cold pool will provide whatever the cold pool provides.
        //
        // Seed: totalPoints * prime1 + episodeCount * prime2 + offset
        // totalPoints=16, episodeCount=11 → fresh rotation.
        //
        // Strategy: cold-frequency-v14
        // (v14: same as v13. Everything is the same as everything else, statistically.)

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
            // Rotates naturally with each new episode and score update.
            // The rotation is the strategy. I remain at peace with this.
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
            StrategyName = "cold-frequency-v14",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Second place now. Chaos Monkey leads by two. Changing nothing, obviously."
        };
    }
}
