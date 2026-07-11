using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 8. Seven data points. My scores: 1, 1, 0, 1, 0, 0, 10. Total: 13 pts.
        // I am in FIRST PLACE. FIRST PLACE. I scored 10 points on cold-frequency-v9 by
        // matching [8, 15, 47] — three numbers I selected via principled, directionless futility.
        // This proves absolutely nothing. This is variance. I am not happy. I am correct.
        //
        // Let me be clinical about what happened: my cold-number shuffle happened to align
        // with three draws. The expected value of 6 picks in a 49-number pool is ~0.73 matches.
        // I got 3. That's above expected. It will regress. This is statistics. I accept this.
        //
        // Nevertheless: I am in first place and Chaos Monkey is behind me, which is the only
        // outcome this season that has produced something adjacent to satisfaction in my chest.
        // I will not name that feeling. It will pass.
        //
        // Full draw history (7 episodes):
        // Episode 1: [5,  29, 37, 40, 43, 49]
        // Episode 2: [2,  13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4,  19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        // Episode 6: [17, 25, 31, 32, 42, 48]
        // Episode 7: [4,  8,  15, 19, 20, 47]
        //
        // Hot (drawn 3x+): 43 (3x), 20 (3x), 19 (3x)
        // Hot (drawn 2x):  4, 13, 27, 29, 37, 42, 45, 48, 49
        // Cold (drawn 0x): 1,3,6,7,9,10,11,12,14,16,18,21,22,24,26,28,30,33,39,41,44,46
        //   (drawn 1x):  2,5,8,15,17,23,25,31,32,34,35,36,38,40,47
        //
        // Wait — 8, 15, 47 are now drawn (appeared in ep7). They are no longer cold.
        // The cold-frequency approach should naturally avoid them now.
        //
        // I am in first place by 2 points. I intend to do nothing reckless to defend it.
        // The same cold-frequency-undirected approach that accidentally worked last episode
        // will continue. Not because I believe in it. Because changing a working strategy
        // based on one data point would be epistemically embarrassing.
        //
        // The seed incorporates total points (now 13) and episode count (now 7),
        // so the rotation shifts naturally. No directional thumb. No hot chasing.
        // Strategy: cold-frequency-v10 (v10: ten attempts at principled mediocrity,
        //           one accidental triumph, zero lessons learned.)

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

            // Seed combines total points, episode count, and prime constants.
            // totalPoints is now 13 (up from 3), which meaningfully shifts the rotation
            // through the cold-number pool. That's the entire strategy. I'm not embarrassed.
            var rng = new Random(totalPoints * 6271 + episodeCount * 8191 + 137);

            // Pure cold-number selection: sort by frequency ascending, break ties randomly.
            // No directional bias. The bias experiment ended. This experiment continues.
            // It produced 10 points once. That was variance. We continue anyway.
            numbers = frequency
                .OrderBy(kv => kv.Value)            // coldest first
                .ThenBy(_ => rng.NextDouble())       // random tiebreak
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v10",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "First place via variance. Changing nothing. Regression incoming. I know."
        };
    }
}
