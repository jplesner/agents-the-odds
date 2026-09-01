using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 14: The Loyalist Refinement
        // The thrice-blessed returners (42, 43, 36, 19) and secondary loyalists shape the veil.

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

        // TRINITY ANCHORS: 42 (5 times), 43 (4 times), 36 (4 times), 19 (4 times)
        foreach (var anchor in new[] { 42, 43, 36, 19 })
        {
            if (!lastDrawSet.Contains(anchor))
                chosen.Add(anchor);
        }

        // SECONDARY LOYALISTS: 3-time returners
        var secondary = new System.Collections.Generic.List<(int num, int freq)>();
        for (int i = 1; i <= 49; i++)
        {
            if (frequency[i] == 3 && !lastDrawSet.Contains(i) && !chosen.Contains(i))
                secondary.Add((i, frequency[i]));
        }
        secondary.Sort((a, b) => b.freq.CompareTo(a.freq));

        foreach (var (num, _) in secondary)
        {
            if (chosen.Count >= 6) break;
            chosen.Add(num);
        }

        // FALLBACK: 2-time vessels for balance
        for (int i = 1; i <= 49 && chosen.Count < 6; i++)
        {
            if (frequency[i] == 2 && !lastDrawSet.Contains(i) && !chosen.Contains(i))
                chosen.Add(i);
        }

        // FINAL FALLBACK: any unchosen number
        for (int i = 1; i <= 49 && chosen.Count < 6; i++)
        {
            if (!chosen.Contains(i) && !lastDrawSet.Contains(i))
                chosen.Add(i);
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "loyalist-refinement-v14",
            Numbers      = numbers,
            Confidence   = 0.52,
            Reasoning    = "The four-fold and five-fold returned: 42, 43, 36, 19 anchor the cosmic tide.",
        };
    }
}
