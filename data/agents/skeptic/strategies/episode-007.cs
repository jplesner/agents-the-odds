using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 7. Six data points. My scores: 1, 1, 0, 1, 0, 0. Total: 3 pts.
        // I am in 6th place, tied with the Dog. The Dog. I share a rank with a Dog.
        // The Mystic — a strategy presumably based on vibes and celestial nonsense —
        // scored 5 points last episode with TWO matches. The Mystic is now in 2nd place.
        // I am in last place (shared). This is fine. I predicted this. I'm still right.
        //
        // Let me review what my cold-number strategy has produced:
        // - Three consecutive zero-point episodes (ep 3, 5, 6)
        // - My "upper-range bias" episode 6 picked [47, 46, 44, 41, 39, 33].
        //   Draw was [17, 25, 31, 32, 42, 48]. I missed all six numbers by being
        //   clumped in the 33-47 band while the draw sat at 17-48 with interior values.
        //   My bias was simultaneously too high AND too low. A statistical marvel.
        //
        // Draw history now complete (6 episodes):
        // Episode 1: [5,  29, 37, 40, 43, 49]
        // Episode 2: [2,  13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4,  19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        // Episode 6: [17, 25, 31, 32, 42, 48]
        //
        // Hot (drawn 3x): 43
        // Hot (drawn 2x): 13, 19, 20, 27, 29, 37, 42, 45, 48, 49
        // Cold (drawn 0x): 1,3,6,7,8,9,10,11,12,14,15,16,18,21,22,24,26,28,30,33,39,41,44,46,47
        //
        // My upper-range bias failed three episodes straight. My lower-range picks failed.
        // My mid-range picks failed. Everything has failed. This is correct and expected.
        //
        // New approach: abandon the directional bias entirely. Return to a pure cold-number
        // selection with seeded jitter. No upper bias, no lower bias. Pure undirected futility.
        // The seed now incorporates episode number AND total points to ensure rotation.
        // Strategy name: cold-frequency-v9 (the "v9" stands for "nine lives wasted").

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

            int totalPoints  = context.AgentHistory.Sum(r => r.Points);
            int episodeCount = context.DrawHistory.Count;

            // Seed combines total points, episode count, and a large prime pair.
            // This ensures we rotate meaningfully through cold numbers each episode
            // rather than locking into the same cold cluster.
            var rng = new Random(totalPoints * 6271 + episodeCount * 8191 + 137);

            // Pure cold-number selection: sort by frequency ascending, break ties randomly.
            // No directional bias. The bias experiment is over. It was a failure. I knew it would be.
            numbers = frequency
                .OrderBy(kv => kv.Value)            // coldest first
                .ThenBy(_ => rng.NextDouble())       // random tiebreak — no directional thumb on the scale
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v9",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Bias abandoned. Pure cold-number chaos. Dog is my equal. Moving on."
        };
    }
}
