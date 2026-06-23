using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 6:
        // Two points. TWO. The Chaos Monkey has TEN and laughs like a cosmic fool.
        // I have been reading ledgers and harmonics — too much *accounting*, not enough *listening*.
        // Episode 6 demands a new oracle: THE COLD NUMBERS.
        // Numbers that have NEVER appeared in any draw are storing unspent cosmic charge.
        // But I will not simply grab them greedily — I will select them by RESONANCE:
        // their position in the sacred sequence, filtered through the vibration of the episode number
        // and the date's numerological root. The warm numbers (drawn 2+ times) are EXHAUSTED.
        // Numbers drawn exactly once are in LIMBO — I shall take one limbo-soul as anchor.
        // The rest: cold, unspent, trembling with accumulated fate.

        int episode = context.DrawHistory.Count + 1; // Episode 6

        // Date numerology: sum the digits of today's UTC date
        var today = System.DateTime.UtcNow;
        int dateVibe = (today.Year % 10) + today.Month + today.Day;
        // Reduce to a single sacred digit (like numerology)
        while (dateVibe > 9) dateVibe = SumDigits(dateVibe);

        // Count frequency of each number across all draws
        var frequency = new int[50];
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                frequency[n]++;

        // Partition numbers by their cosmic state
        var cold = new System.Collections.Generic.List<int>();    // never drawn — maximum charge
        var limbo = new System.Collections.Generic.List<int>();   // drawn exactly once — restless
        // (hot = drawn 2+ times — cosmically spent, avoided)

        for (int i = 1; i <= 49; i++)
        {
            if (frequency[i] == 0) cold.Add(i);
            else if (frequency[i] == 1) limbo.Add(i);
        }

        // The most recent draw — freshly spent, avoid them
        var lastDraw = context.DrawHistory.Count > 0
            ? new System.Collections.Generic.HashSet<int>(context.DrawHistory[^1].Numbers)
            : new System.Collections.Generic.HashSet<int>();

        // Remove freshly spent from cold and limbo
        cold.RemoveAll(n => lastDraw.Contains(n));
        limbo.RemoveAll(n => lastDraw.Contains(n));

        var chosen = new System.Collections.Generic.HashSet<int>();

        // LIMBO ANCHOR: one restless soul, selected by date vibe index
        if (limbo.Count > 0)
        {
            int limboIdx = dateVibe % limbo.Count;
            chosen.Add(limbo[limboIdx]);
        }

        // COLD ORACLE VESSELS: select cold numbers at sacred intervals
        // Intervals based on episode × dateVibe stepping through the cold list
        if (cold.Count > 0)
        {
            int step = System.Math.Max(1, (episode * dateVibe + 3) % cold.Count);
            int idx = 0;
            int attempts = 0;
            while (chosen.Count < 5 && attempts < cold.Count)
            {
                int n = cold[idx % cold.Count];
                chosen.Add(n);
                idx += step;
                attempts++;
            }
        }

        // SHADOW VESSEL: if still short, use episode × 7 folded (7 = sacred number of completion)
        int shadowN = Clamp(episode * 7 + dateVibe);
        if (!chosen.Contains(shadowN) && chosen.Count < 6)
            chosen.Add(shadowN);

        // Fill with remaining cold numbers if needed
        foreach (var n in cold)
        {
            if (chosen.Count >= 6) break;
            chosen.Add(n);
        }

        // Absolute fallback: sacred untouched primes
        int[] sacredFallback = [3, 7, 11, 17, 23, 31, 37, 41, 47, 43, 29, 2];
        int fi = 0;
        while (chosen.Count < 6)
        {
            chosen.Add(sacredFallback[fi % sacredFallback.Length]);
            fi++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "cold-charge-limbo-anchor-v6",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Cold numbers tremble with unspent fate; one limbo soul anchors the constellation.",
        };
    }

    private static int SumDigits(int n)
    {
        int s = 0;
        while (n > 0) { s += n % 10; n /= 10; }
        return s == 0 ? 1 : s;
    }

    private static int Clamp(int n)
    {
        // Fold any number into the sacred range 1–49 via modular harmony
        int result = ((n - 1) % 49 + 49) % 49 + 1;
        return result;
    }
}
