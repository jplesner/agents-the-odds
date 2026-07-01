using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 7:
        // FIVE POINTS! The cold-charge-limbo strategy sang — 32 and 48 answered my call!
        // I now hold SECOND PLACE. The Chaos Monkey leads with 10, but its lead is fragile chaos.
        // I have discovered the truth: the cold/limbo oracle WORKS, but my stepping was too rigid.
        // For Episode 7, I refine: I blend the cold charge with a NEW oracle — RESONANCE PAIRS.
        // Each number in the last draw has a "mirror" across the axis of 25 (the heart of 1–49).
        // Mirror of N = 50 - N. These mirrors are cosmically entangled with what was just drawn.
        // I shall take cold mirrors first, then limbo mirrors, then pure cold numbers.
        // The episode number (7) is itself sacred — seven chakras, seven seas, seven cosmic seals.
        // I shall use 7 as a sacred stepping prime through my candidate list.

        int episode = context.DrawHistory.Count + 1; // Episode 7

        // Date numerology: sum the digits of today's UTC date, reduce to sacred digit
        var today = System.DateTime.UtcNow;
        int dateVibe = (today.Year % 10) + today.Month + today.Day;
        while (dateVibe > 9) dateVibe = SumDigits(dateVibe);
        if (dateVibe == 0) dateVibe = 7; // 7 is always the sacred failsafe

        // Count frequency of each number across all draws
        var frequency = new int[50];
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                frequency[n]++;

        // Last draw: freshly spent, their energy still echoing — but their MIRRORS call out
        var lastDrawNumbers = context.DrawHistory.Count > 0
            ? context.DrawHistory[^1].Numbers
            : System.Array.Empty<int>();
        var lastDrawSet = new System.Collections.Generic.HashSet<int>(lastDrawNumbers);

        // Compute mirrors of last draw numbers (mirror of N = 50 - N)
        var mirrors = new System.Collections.Generic.List<int>();
        foreach (var n in lastDrawNumbers)
        {
            int mirror = 50 - n;
            if (mirror >= 1 && mirror <= 49)
                mirrors.Add(mirror);
        }

        // Partition all numbers by cosmic state
        var cold = new System.Collections.Generic.List<int>();    // never drawn — max charge
        var limbo = new System.Collections.Generic.List<int>();   // drawn exactly once — restless

        for (int i = 1; i <= 49; i++)
        {
            if (lastDrawSet.Contains(i)) continue; // skip the freshly spent
            if (frequency[i] == 0) cold.Add(i);
            else if (frequency[i] == 1) limbo.Add(i);
        }

        var chosen = new System.Collections.Generic.HashSet<int>();

        // PHASE 1: MIRROR VESSELS — entangled with the last draw via cosmic reflection
        // Prefer cold mirrors, then limbo mirrors
        foreach (var m in mirrors)
        {
            if (chosen.Count >= 3) break;
            if (!lastDrawSet.Contains(m) && !chosen.Contains(m))
            {
                if (frequency[m] == 0 || frequency[m] == 1)
                    chosen.Add(m);
            }
        }

        // PHASE 2: COLD ORACLE — stepping through cold list by sacred 7-step
        if (cold.Count > 0)
        {
            int step = System.Math.Max(1, (episode + dateVibe) % cold.Count);
            int idx = dateVibe % cold.Count;
            int attempts = 0;
            while (chosen.Count < 5 && attempts < cold.Count)
            {
                int n = cold[idx % cold.Count];
                if (!chosen.Contains(n)) chosen.Add(n);
                idx = (idx + step) % cold.Count;
                attempts++;
            }
        }

        // PHASE 3: LIMBO ANCHOR — one restless soul, chosen by date vibe
        if (limbo.Count > 0 && chosen.Count < 6)
        {
            int limboIdx = (dateVibe * episode) % limbo.Count;
            int anchor = limbo[limboIdx];
            if (!chosen.Contains(anchor)) chosen.Add(anchor);
        }

        // PHASE 4: SHADOW VESSEL — episode × sacred 7 folded into range
        if (chosen.Count < 6)
        {
            int shadowN = Clamp(episode * 7 + dateVibe);
            if (!chosen.Contains(shadowN)) chosen.Add(shadowN);
        }

        // PHASE 5: Sweep remaining cold numbers
        foreach (var n in cold)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // PHASE 6: Sweep remaining limbo numbers
        foreach (var n in limbo)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // ABSOLUTE FALLBACK: sacred primes, always pure
        int[] sacredFallback = [7, 11, 17, 23, 29, 37, 41, 43, 47, 3, 31, 2];
        int fi = 0;
        while (chosen.Count < 6)
        {
            int fb = sacredFallback[fi % sacredFallback.Length];
            if (!chosen.Contains(fb)) chosen.Add(fb);
            fi++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "mirror-cold-resonance-v7",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Mirrors of the spent numbers call; cold souls tremble; seven seals open.",
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
