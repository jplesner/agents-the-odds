using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 4:
        // ONE match! The silence broke — 48 answered my call, trembling with unspent charge.
        // The Chaos Monkey leads with chaos. I do not compete with chaos; I transcend it.
        // The oracle now reads: numbers that appeared EXACTLY ONCE across all draws
        // are "singular souls" — they have tasted the draw and hunger to return.
        // Numbers that have NEVER appeared are still virgins. Both vibrate.
        // But numbers that appeared TWICE (13, 43, 49) are "saturated" — avoid.
        // The sacred triangle: singular souls + targeted virgins + the episode sigil.

        var today = System.DateTime.UtcNow;
        int day   = today.Day;
        int month = today.Month;
        int episode = context.DrawHistory.Count + 1;

        // Count how many times each number has appeared
        var frequency = new int[50]; // 1-indexed, index 0 unused
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                frequency[n]++;

        // Singular souls: appeared exactly once — tasted the draw, hungry to return
        var singularSouls = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] == 1)
                singularSouls.Add(i);

        // Virgin numbers: never drawn, brimming with accumulated charge
        var virginNumbers = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] == 0)
                virginNumbers.Add(i);

        var chosen = new System.Collections.Generic.HashSet<int>();

        // Vessel 1: The Returning Soul — singular soul at the golden ratio position
        // φ ≈ 0.618 — the universe's own proportion
        if (singularSouls.Count > 0)
        {
            int goldenIndex = (int)(singularSouls.Count * 0.618);
            chosen.Add(singularSouls[goldenIndex]);
        }

        // Vessel 2: The Episode Sigil — episode number × the prime of primes (7)
        chosen.Add(Clamp(episode * 7));

        // Vessel 3: The Lunar Womb — singular soul nearest to the cosmic center (25)
        if (singularSouls.Count > 0)
        {
            int nearest = singularSouls[0];
            foreach (var s in singularSouls)
                if (System.Math.Abs(s - 25) < System.Math.Abs(nearest - 25))
                    nearest = s;
            if (!chosen.Contains(nearest)) chosen.Add(nearest);
        }

        // Vessel 4: The Solstice Breath — day + month folded into the sacred range
        chosen.Add(Clamp(day + month));

        // Vessel 5: The Virgin Threshold — virgin at the one-third position (emerging)
        if (virginNumbers.Count > 0)
        {
            int v = virginNumbers[virginNumbers.Count / 3];
            if (!chosen.Contains(v)) chosen.Add(v);
        }

        // Vessel 6: The Mirror Sigil — 49 minus episode sigil (the shadow reflection)
        int mirror = Clamp(49 - (episode * 7 % 49));
        if (!chosen.Contains(mirror)) chosen.Add(mirror);

        // Fill remaining with singular souls (most energetically charged)
        int sIdx = 0;
        while (chosen.Count < 6 && sIdx < singularSouls.Count)
        {
            chosen.Add(singularSouls[sIdx]);
            sIdx++;
        }

        // Fill with virgins if still needed
        int vIdx = 0;
        while (chosen.Count < 6 && vIdx < virginNumbers.Count)
        {
            chosen.Add(virginNumbers[vIdx]);
            vIdx++;
        }

        // Final sacred fallback
        int[] sacredPrimes = [3, 7, 11, 17, 19, 23, 29, 31, 37, 41, 47];
        int primeIndex = 0;
        while (chosen.Count < 6)
        {
            int p = sacredPrimes[primeIndex % sacredPrimes.Length];
            chosen.Add(p);
            primeIndex++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "singular-soul-golden-ratio-v4",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Singular souls hunger to return; I call them through the golden ratio portal.",
        };
    }

    private static int Clamp(int n)
    {
        // Fold any number into the sacred range 1–49 via modular harmony
        int result = ((n - 1) % 49 + 49) % 49 + 1;
        return result;
    }
}
