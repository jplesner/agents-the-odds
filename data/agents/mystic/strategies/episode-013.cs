using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 13: The Recurrence Spiral
        // I abandoned the false towers of Twelve. Now I read what the cosmos truly loves—
        // Numbers that return again and again, drawn 3+ times across 12 episodes.
        // These are the LOYALISTS, the gods' favorites, immune to entropy.
        // Anchor: 42 and 43, the dual-pillar returners; 36, the trinity-blessed anchor (episodes 3, 6, 10).

        int episode = 13;
        var today = System.DateTime.UtcNow;
        int rawVibe = (today.Year % 100) + today.Month + today.Day + episode;
        int dateVibe = SumDigitsToSingle(rawVibe);

        // Count frequency for each number across all draws
        var frequency = new int[50];
        var episodesSeen = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
        for (int i = 1; i <= 49; i++) 
            episodesSeen[i] = new System.Collections.Generic.List<int>();

        foreach (var draw in context.DrawHistory)
        {
            foreach (var n in draw.Numbers)
            {
                frequency[n]++;
                episodesSeen[n].Add(draw.DrawNumber);
            }
        }

        // Last draw: energy spent
        var lastDrawSet = new System.Collections.Generic.HashSet<int>(
            context.DrawHistory.Count > 0
                ? context.DrawHistory[^1].Numbers
                : System.Array.Empty<int>()
        );

        var chosen = new System.Collections.Generic.HashSet<int>();

        // LOYALISTS: appeared 3+ times (the gods' favorites, immune to entropy)
        var loyalists = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
        {
            if (frequency[i] >= 3 && !lastDrawSet.Contains(i))
                loyalists.Add(i);
        }
        loyalists.Sort((a, b) => frequency[b].CompareTo(frequency[a]));

        // DUAL PILLARS: 42 and 43 (appeared 3 times each, the returner twins)
        if (!lastDrawSet.Contains(42)) chosen.Add(42);
        if (!lastDrawSet.Contains(43)) chosen.Add(43);

        // TRINITY ANCHOR: 36 (appeared in episodes 3, 6, 10 — multiples/resonances of sacred 3)
        if (!lastDrawSet.Contains(36) && chosen.Count < 3) chosen.Add(36);

        // SECONDARY LOYALISTS: other 3+ frequency numbers
        foreach (var n in loyalists)
        {
            if (chosen.Count >= 5) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // TERTIARY: appeared exactly twice (balanced, underused power)
        var dualVessels = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
        {
            if (frequency[i] == 2 && !lastDrawSet.Contains(i))
                dualVessels.Add(i);
        }

        foreach (var n in dualVessels)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // FALLBACK: modulo spiral by episode number
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
            StrategyName = "recurrence-spiral-loyalist-v13",
            Numbers      = numbers,
            Confidence   = 0.48,
            Reasoning    = "The cosmos loves its loyalists. Thrice-chosen returners and the trinity anchor guide my spiral.",
        };
    }

    private static int SumDigitsToSingle(int n)
    {
        while (n > 9)
        {
            int s = 0;
            while (n > 0) { s += n % 10; n /= 10; }
            n = s;
        }
        return n == 0 ? 1 : n;
    }
}
