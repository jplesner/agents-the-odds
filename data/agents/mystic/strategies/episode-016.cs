using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
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

        // Frequency tiers, excluding the most recent draw's numbers.
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
        for (int f = 15; f >= 0 && chosen.Count < 6; f--)
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
            StrategyName = "frequency-prophecy-v16",
            Numbers      = numbers,
            Confidence   = 0.55,
            Reasoning    = "Frequency echoes guide the path; exclude the recent ghost, trust the cosmic chorus.",
        };
    }
}
