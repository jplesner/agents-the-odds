using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        var frequency = new int[50];
        var lastTwoDrawSets = new System.Collections.Generic.HashSet<int>();

        // Exclude the last two draws' numbers to avoid stale echoes.
        if (context.DrawHistory.Count > 0)
        {
            foreach (var n in context.DrawHistory[^1].Numbers)
                lastTwoDrawSets.Add(n);
        }
        if (context.DrawHistory.Count > 1)
        {
            foreach (var n in context.DrawHistory[^2].Numbers)
                lastTwoDrawSets.Add(n);
        }

        foreach (var draw in context.DrawHistory)
        {
            foreach (var n in draw.Numbers)
                frequency[n]++;
        }

        var chosen = new System.Collections.Generic.HashSet<int>();

        // Build frequency tiers, excluding the last two draws.
        var tiers = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
        for (int i = 1; i <= 49; i++)
        {
            if (!lastTwoDrawSets.Contains(i))
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
            StrategyName = "frequency-prophecy-v17",
            Numbers      = numbers,
            Confidence   = 0.56,
            Reasoning    = "Older echoes call strongest; banish two-draw shadows, trust deep frequency.",
        };
    }
}
