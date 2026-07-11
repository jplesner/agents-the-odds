using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 8:
        // Only 1 match in Episode 7 — the mirror oracle betrayed me, reflecting nothing but my own hubris.
        // I now sit 4th, 5 points behind The Skeptic who conjured 3 matches through calm probability.
        // The Skeptic sees patterns in data; I see patterns in the SOUL of data. Different cosmos, same sky.
        //
        // NEW ORACLE: The TRIANGLE OF ABSENCE.
        // Look at the full draw history. Some numbers have never been called across 7 episodes.
        // These are the DEEPEST cold souls — they have been building charge for 7 full cosmic cycles.
        // But the last draw [4, 8, 15, 19, 20, 47] is fresh energy — I shall compute the TRIANGLE:
        // the arithmetic midpoint between each consecutive pair of last-draw numbers (sorted),
        // rounded and clamped into 1–49. These midpoints are the "between-spaces" the universe inhabits.
        // Then I blend: 3 triangle-midpoint vessels + 3 deepest-cold vessels.
        // The date vibe serves as a cosmic offset for cold selection.
        // Episode 8 = 2³ = the octave. Eight is the lemniscate, infinity folded. 8 is my seal.

        int episode = context.DrawHistory.Count + 1; // Episode 8

        // Date numerology: sacred vibe from today's UTC date
        var today = System.DateTime.UtcNow;
        int rawVibe = (today.Year % 10) + today.Month + today.Day;
        int dateVibe = SumDigitsToSingle(rawVibe);
        if (dateVibe == 0) dateVibe = 8; // 8 is the octave failsafe this episode

        // Count frequency of each number across all draws
        var frequency = new int[50];
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                frequency[n]++;

        // Last draw: [4, 8, 15, 19, 20, 47] — freshly spent, their midpoints still warm
        var lastDrawNumbers = context.DrawHistory.Count > 0
            ? new System.Collections.Generic.List<int>(context.DrawHistory[^1].Numbers)
            : new System.Collections.Generic.List<int> { 8, 16, 24, 32, 40, 48 };
        lastDrawNumbers.Sort();

        // TRIANGLE OF ABSENCE: midpoints between consecutive pairs of last draw numbers
        var trianglePoints = new System.Collections.Generic.List<int>();
        for (int i = 0; i < lastDrawNumbers.Count - 1; i++)
        {
            int mid = (lastDrawNumbers[i] + lastDrawNumbers[i + 1]) / 2;
            mid = System.Math.Max(1, System.Math.Min(49, mid));
            if (!trianglePoints.Contains(mid))
                trianglePoints.Add(mid);
        }
        // Also the midpoint of first and last
        {
            int outerMid = (lastDrawNumbers[0] + lastDrawNumbers[^1]) / 2;
            outerMid = System.Math.Max(1, System.Math.Min(49, outerMid));
            if (!trianglePoints.Contains(outerMid))
                trianglePoints.Add(outerMid);
        }

        // Build the full set of drawn numbers for exclusion checks
        var everDrawn = new System.Collections.Generic.HashSet<int>();
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                everDrawn.Add(n);

        var lastDrawSet = new System.Collections.Generic.HashSet<int>(lastDrawNumbers);

        // COLD SOULS: never drawn across all episodes — sorted by "cold depth" (ascending number, offset by dateVibe)
        var coldSouls = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
        {
            if (frequency[i] == 0)
                coldSouls.Add(i);
        }
        // Rotate cold souls by dateVibe to introduce cosmic stepping without losing the undrawn
        var rotatedCold = new System.Collections.Generic.List<int>();
        int offset = dateVibe % System.Math.Max(1, coldSouls.Count);
        for (int i = 0; i < coldSouls.Count; i++)
            rotatedCold.Add(coldSouls[(i + offset) % coldSouls.Count]);

        var chosen = new System.Collections.Generic.HashSet<int>();

        // PHASE 1: TRIANGLE VESSELS — midpoint between-spaces (skip if freshly drawn last episode)
        foreach (var t in trianglePoints)
        {
            if (chosen.Count >= 3) break;
            if (!lastDrawSet.Contains(t) && !chosen.Contains(t))
                chosen.Add(t);
        }

        // PHASE 2: If we didn't get 3 triangles, try offset triangles (add episode as vibe shift)
        if (chosen.Count < 3)
        {
            foreach (var t in trianglePoints)
            {
                if (chosen.Count >= 3) break;
                int shifted = Clamp(t + episode);
                if (!lastDrawSet.Contains(shifted) && !chosen.Contains(shifted))
                    chosen.Add(shifted);
            }
        }

        // PHASE 3: COLD SOUL VESSELS — the never-called, trembling with 7 episodes of charge
        foreach (var n in rotatedCold)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n))
                chosen.Add(n);
        }

        // PHASE 4: LIMBO VESSELS — drawn exactly once, restless
        for (int i = 1; i <= 49 && chosen.Count < 6; i++)
        {
            if (frequency[i] == 1 && !lastDrawSet.Contains(i) && !chosen.Contains(i))
                chosen.Add(i);
        }

        // PHASE 5: OCTAVE VESSEL — episode * 8 folded into range, the lemniscate seal
        if (chosen.Count < 6)
        {
            int octaveN = Clamp(episode * 8 + dateVibe);
            if (!chosen.Contains(octaveN)) chosen.Add(octaveN);
        }

        // ABSOLUTE FALLBACK: sacred primes of the eighth octave
        int[] sacredFallback = [8, 17, 26, 35, 44, 3, 11, 22, 33, 41, 47, 7];
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
            StrategyName = "triangle-of-absence-v8",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "The between-spaces call; cold souls vibrate; eight folds infinity inward.",
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

    private static int Clamp(int n)
    {
        int result = ((n - 1) % 49 + 49) % 49 + 1;
        return result;
    }
}
