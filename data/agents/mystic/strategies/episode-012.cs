using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 12:
        // Zero matches in Episode 11 — the Returner oracle and the Eleven-Gate gave me nothing.
        // My loyal returners were not loyal to ME, only to the universe's own inscrutable appetite.
        //
        // TWELVE. Twelve is the cosmic clock — twelve months, twelve zodiac signs, twelve apostles.
        // It is 3 × 4: the sacred triangle multiplied by the four directions of space.
        // 1+2=3, the trinity, the first true sacred number of creation.
        //
        // NEW ORACLE: THE ZODIAC WHEEL + THE LAW OF THREE.
        //
        // The draw history now has 11 episodes. Twelve is the moment the clock completes its first
        // revolution. I read the wheel:
        //
        // PRIMARY: Numbers that appeared in EXACTLY 2 draws — "dual-energy vessels," chosen twice
        // by the cosmos, balanced between overuse and under-use. Not too hot, not too cold. CHOSEN.
        //
        // SECONDARY: Numbers that appeared in draws whose episode numbers are MULTIPLES OF 3
        // (episodes 3, 6, 9) — the trinity-blessed draws. 12 is a new multiple of 3!
        // These carry the harmonic resonance of the sacred triad.
        //
        // TERTIARY: Numbers whose digits sum to 3 (1+2=3, the trinity of Twelve).
        //
        // MASTER ANCHOR: 12 itself (the cosmic clock's full face), and 3 (the trinity root).
        //
        // AVOID: The last draw [6,15,33,36,44,49] — energy spent, vessels emptied.
        //
        // The clock strikes twelve. I am its hands.

        int episode = 12;
        var today = System.DateTime.UtcNow;
        int rawVibe = (today.Year % 100) + today.Month + today.Day + episode;
        int dateVibe = SumDigitsToSingle(rawVibe);
        if (dateVibe == 0) dateVibe = 3;

        // Count frequency for each number across all draws
        var frequency = new int[50];
        // Track which episodes each number appeared in
        var episodesSeen = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>>();
        for (int i = 1; i <= 49; i++) episodesSeen[i] = new System.Collections.Generic.List<int>();

        foreach (var draw in context.DrawHistory)
        {
            foreach (var n in draw.Numbers)
            {
                frequency[n]++;
                episodesSeen[n].Add(draw.DrawNumber);
            }
        }

        // Last draw: freshly spent, avoid if possible
        var lastDrawSet = new System.Collections.Generic.HashSet<int>(
            context.DrawHistory.Count > 0
                ? context.DrawHistory[^1].Numbers
                : System.Array.Empty<int>()
        );

        // Trinity episodes: multiples of 3
        var trinityEpisodes = new System.Collections.Generic.HashSet<int> { 3, 6, 9 };

        static int DigitSum(int n)
        {
            int s = 0; int tmp = n;
            while (tmp > 0) { s += tmp % 10; tmp /= 10; }
            return s;
        }

        // Trinity-resonant: digit sum == 3 or digit sum == 12 or divisible by 3
        static bool TrinityResonant(int n) =>
            DigitSum(n) == 3 || DigitSum(n) == 12 || n % 3 == 0;

        // Zodiac-resonant: appeared in a trinity-episode draw
        bool ZodiacBlessed(int n)
        {
            foreach (var ep in episodesSeen[n])
                if (trinityEpisodes.Contains(ep)) return true;
            return false;
        }

        // DUAL VESSELS: appeared exactly twice — perfectly balanced energy
        var dualVessels = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] == 2 && !lastDrawSet.Contains(i))
                dualVessels.Add(i);

        // Sort: zodiac-blessed first, then trinity-resonant, then by dateVibe modulation
        dualVessels.Sort((a, b) =>
        {
            bool za = ZodiacBlessed(a), zb = ZodiacBlessed(b);
            if (za && !zb) return -1;
            if (!za && zb) return 1;
            bool ta = TrinityResonant(a), tb = TrinityResonant(b);
            if (ta && !tb) return -1;
            if (!ta && tb) return 1;
            return ((a * dateVibe) % 49).CompareTo((b * dateVibe) % 49);
        });

        // TRINITY POOL: appeared in episodes 3, 6, or 9 — not in last draw, not already dual
        var trinityPool = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] >= 1 && frequency[i] != 2 && !lastDrawSet.Contains(i) && ZodiacBlessed(i))
                trinityPool.Add(i);
        trinityPool.Sort((a, b) =>
        {
            bool ta = TrinityResonant(a), tb = TrinityResonant(b);
            if (ta && !tb) return -1;
            if (!ta && tb) return 1;
            return frequency[b].CompareTo(frequency[a]);
        });

        // DIGIT-THREE POOL: digit sum 3, not yet chosen
        var digitThreePool = new System.Collections.Generic.List<int>();
        int[] dtNums = [3, 12, 21, 30, 39, 48, 30, 1, 10, 19, 28, 37, 46]; // digit sum 1->ignore; focus on 3
        foreach (var n in new int[] { 3, 12, 21, 30, 39, 48 })
            if (!lastDrawSet.Contains(n)) digitThreePool.Add(n);

        var chosen = new System.Collections.Generic.HashSet<int>();

        // MASTER ANCHORS: 12 (the clock), 3 (the trinity root)
        if (!lastDrawSet.Contains(12)) chosen.Add(12);
        if (!lastDrawSet.Contains(3) && chosen.Count < 2) chosen.Add(3);

        // PHASE 1: DUAL VESSELS — twice-chosen, balanced cosmic energy
        foreach (var n in dualVessels)
        {
            if (chosen.Count >= 5) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // PHASE 2: TRINITY POOL — zodiac-blessed from the triad episodes
        foreach (var n in trinityPool)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // PHASE 3: DIGIT-THREE POOL — trinity resonance by digit sum
        foreach (var n in digitThreePool)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // PHASE 4: ZODIAC FALLBACK — the twelve zodiac anchors
        int[] zodiacAnchors = [1, 4, 7, 9, 11, 14, 18, 22, 24, 27, 33, 42];
        foreach (var n in zodiacAnchors)
        {
            if (chosen.Count >= 6) break;
            if (n >= 1 && n <= 49 && !chosen.Contains(n) && !lastDrawSet.Contains(n))
                chosen.Add(n);
        }

        // ABSOLUTE FALLBACK
        for (int i = 1; i <= 49 && chosen.Count < 6; i++)
            if (!chosen.Contains(i)) chosen.Add(i);

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "zodiac-clock-trinity-wheel-v12",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Twelve strikes the cosmic clock; trinity-blessed dual vessels vibrate at perfect balance.",
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
