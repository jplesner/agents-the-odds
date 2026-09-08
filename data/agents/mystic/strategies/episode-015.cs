using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 15: The Frequency Prophecy Refined
        // Frequency-based returner anchoring with dynamic fallback patterns.

        var frequency = new int[50];
        var lastDrawSet = new System.Collections.Generic.HashSet<int>(
            context.DrawHistory.Count > 0
                ? context.DrawHistory[^1].Numbers
                : System.Array.Empty<int>()
        );

        foreach (var draw in context.DrawHistory)
        {
            foreach (var n in draw.Numbers)
                frequency[n]++;
        }

        var chosen = new System.Collections.Generic.HashSet<int>();

        // FREQUENCY TIERS: Prioritize highest-frequency returners.
        var tiers = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
        for (int i = 1; i <= 49; i++)
        {
            if (!lastDrawSet.Contains(i))
            {
                int freq = frequency[i];
                if (!tiers.ContainsKey(freq))
                    tiers[freq] = new System.Collections.Generic.List<int>();
                tiers[freq].Add(i);
            }
        }

        // Fill from highest frequency downward.
        for (int f = 14; f >= 0 && chosen.Count < 6; f--)
        {
            if (tiers.ContainsKey(f))
            {
                foreach (var num in tiers[f])
                {
                    if (chosen.Count >= 6) break;
                    chosen.Add(num);
                }
            }
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "frequency-prophecy-v15",
            Numbers      = numbers,
            Confidence   = 0.54,
            Reasoning    = "The cosmos whispers through frequency tides: highest returners beckon the cosmic tide.",
        };
    }
}
