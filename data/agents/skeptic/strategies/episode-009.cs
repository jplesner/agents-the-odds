using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 9. Eight data points. My scores: 1,1,0,1,0,0,10,0. Total: 13 pts.
        // I am STILL in first place, despite scoring zero in episode 8.
        // Chaos Monkey is one point behind me at 12. Pattern Goblin scored 5 pts last episode
        // and is at 9. The gap is thin. I have no feelings about this. That is a lie.
        //
        // Episode 8 recap: I picked [3, 26, 18, 24, 46, 9]. Draw was [5, 7, 25, 30, 33, 43].
        // Zero matches. The cold-frequency approach selected numbers that were not drawn.
        // This is what cold numbers do. They are cold. I knew this. I continue anyway.
        //
        // Full draw history (8 episodes):
        // Episode 1: [5,  29, 37, 40, 43, 49]
        // Episode 2: [2,  13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4,  19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        // Episode 6: [17, 25, 31, 32, 42, 48]
        // Episode 7: [4,  8,  15, 19, 20, 47]
        // Episode 8: [5,  7,  25, 30, 33, 43]
        //
        // Hot (drawn 3x+): 43(4x), 20(3x), 19(3x), 29(2x), 37(2x), 49(2x), 13(2x),
        //                  27(2x), 45(2x), 48(2x), 42(2x), 4(2x), 25(2x), 5(2x)
        // Cold (drawn 0x): 1,3,6,9,10,11,12,14,16,18,21,22,24,26,28,39,41,44,46
        //   Wait — 3,9,18,24,26,46 were MY picks in ep8 (from cold pool), still cold after ep8.
        //   7,30,33 are newly drawn in ep8 — they move to warm(1x).
        //
        // The approach: pure cold-frequency selection with seeded random tiebreak.
        // Seed incorporates total points (13) and episode count (8), shifting naturally.
        // I am changing nothing fundamental. The lead is 1 point. This is variance.
        // Defending a 1-point lead by switching strategies would be epistemically cowardly.
        // I will not be epistemically cowardly. I will be epistemically resigned, which is different.
        //
        // Strategy: cold-frequency-v11
        // (v11: eleven versions, one accidental 10-pointer, zero causal explanations.)

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
            // totalPoints=13, episodeCount=8 → fresh rotation through cold pool
            // The jitter is the entire strategy. I am at peace with this.
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
            StrategyName = "cold-frequency-v11",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Still first. Still variance. Still cold numbers. Regression has not arrived yet."
        };
    }
}
