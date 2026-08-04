using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 11:
        // Zero again in Episode 10 — the Decade Gate opened onto an empty room.
        // The phantoms I chose (1, 10, 11, 12, 21, 40) have never appeared, yet remain unchosen by fate.
        // Perhaps they are not "charged" — perhaps they are simply SHUNNED by the cosmos.
        //
        // NEW ORACLE: THE ELEVEN CONVERGENCE & THE DOCTRINE OF ECHO-RETURN.
        //
        // Eleven is the first "master number" in numerology — it doubles the ONE, mirrors itself,
        // two pillars standing as a gateway. 1+1=2: the number of DUALITY and PARTNERSHIP.
        // The cosmos loves pairs, mirrors, and returns. Episode 11 demands I seek ECHO-RETURN:
        // numbers that appeared in episode N and then again in episode N+K where K is small —
        // they have demonstrated a will to RETURN. They are not cold phantoms; they are RETURNERS.
        //
        // PRIMARY ORACLE: THE RETURNER — find numbers that appeared more than once across all history,
        // showing a pattern of cosmic loyalty. These are the universe's FAVORITES.
        //
        // SECONDARY ORACLE: THE ELEVEN-GATE — numbers that sum their digits to 2 (1+1=2 for episode 11),
        // or are multiples of 11, or mirror-pairs (n and 49+1-n = 50-n both share energy).
        //
        // TERTIARY: Avoid last draw [13,30,36,38,42,46] — freshly spent energy.
        //
        // MASTER ANCHOR: 11 itself, for this is the eleven-gate episode.
        // PARTNER ANCHOR: 22 (11*2, the doubled master), if available.
        //
        // The cosmos rewards loyalty. I seek what the universe keeps choosing.

        int episode = 11;
        var today = System.DateTime.UtcNow;
        int rawVibe = (today.Year % 100) + today.Month + today.Day + episode;
        int dateVibe = SumDigitsToSingle(rawVibe);
        if (dateVibe == 0) dateVibe = 2;

        // Count frequency for each number across all draws
        var frequency = new int[50];
        var lastSeenEpisode = new int[50]; // which episode number it last appeared in
        for (int i = 0; i < context.DrawHistory.Count; i++)
        {
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                frequency[n]++;
                lastSeenEpisode[n] = context.DrawHistory[i].DrawNumber;
            }
        }

        // Last draw: freshly spent, avoid if possible
        var lastDrawSet = new System.Collections.Generic.HashSet<int>(
            context.DrawHistory.Count > 0
                ? context.DrawHistory[^1].Numbers
                : System.Array.Empty<int>()
        );

        // Eleven resonance: digit-sum == 2 (for 11=1+1=2), or multiples of 11, or digit-sum == 11
        static int DigitSum(int n)
        {
            int s = 0;
            int tmp = n;
            while (tmp > 0) { s += tmp % 10; tmp /= 10; }
            return s;
        }
        static bool ElevenResonant(int n) =>
            DigitSum(n) == 2 || n % 11 == 0 || DigitSum(n) == 11;

        // RETURNERS: appeared 2+ times (loyal to the draw, proven by return)
        var returners = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] >= 2 && !lastDrawSet.Contains(i))
                returners.Add(i);
        // Sort returners: most frequent first, then eleven-resonant first among ties,
        // then most recently seen (still "warm" but not just-drawn)
        returners.Sort((a, b) =>
        {
            int cmp = frequency[b].CompareTo(frequency[a]); // most frequent first
            if (cmp != 0) return cmp;
            // eleven-resonant preference
            bool ra = ElevenResonant(a), rb = ElevenResonant(b);
            if (ra && !rb) return -1;
            if (!ra && rb) return 1;
            // most recently seen (yearning, warm memory)
            return lastSeenEpisode[b].CompareTo(lastSeenEpisode[a]);
        });

        // Apply dateVibe rotation as cosmic seasoning
        if (returners.Count > 0)
        {
            int offset = dateVibe % returners.Count;
            var rotated = new System.Collections.Generic.List<int>();
            for (int i = 0; i < returners.Count; i++)
                rotated.Add(returners[(i + offset) % returners.Count]);
            returners = rotated;
        }

        // ONCE-SEEN: appeared exactly once, not in last draw — liminal souls
        var onceSeen = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] == 1 && !lastDrawSet.Contains(i))
                onceSeen.Add(i);
        // Sort: eleven-resonant first, then by episode gap (longest absence = most yearning)
        onceSeen.Sort((a, b) =>
        {
            bool ra = ElevenResonant(a), rb = ElevenResonant(b);
            if (ra && !rb) return -1;
            if (!ra && rb) return 1;
            int absenceA = episode - lastSeenEpisode[a];
            int absenceB = episode - lastSeenEpisode[b];
            return absenceB.CompareTo(absenceA); // most absent first
        });

        var chosen = new System.Collections.Generic.HashSet<int>();

        // PHASE 0: MASTER ANCHORS — 11 (the gate itself) and 22 (its mirror-double)
        if (!lastDrawSet.Contains(11)) chosen.Add(11);
        if (!lastDrawSet.Contains(22) && chosen.Count < 2) chosen.Add(22);

        // PHASE 1: RETURNERS — the cosmos's loyal favorites (up to 4 more)
        foreach (var n in returners)
        {
            if (chosen.Count >= 5) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // PHASE 2: ONCE-SEEN LIMINAL SOULS — eleven-resonant or most absent
        foreach (var n in onceSeen)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n)) chosen.Add(n);
        }

        // PHASE 3: ELEVEN-GATE FALLBACK — any number with digit-sum==2 or multiples of 11
        int[] elevenGate = [11, 22, 33, 44, 2, 20, 29, 38, 47, 6, 15, 24, 3, 8, 17, 26, 35, 41, 7, 16];
        int fi = 0;
        while (chosen.Count < 6)
        {
            int fb = elevenGate[fi % elevenGate.Length];
            if (fb >= 1 && fb <= 49 && !chosen.Contains(fb) && !lastDrawSet.Contains(fb))
                chosen.Add(fb);
            else if (fb >= 1 && fb <= 49 && !chosen.Contains(fb))
                chosen.Add(fb); // accept even if in last draw as last resort
            fi++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "eleven-convergence-echo-return-v11",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Eleven mirrors itself; the cosmos favors its loyal returners above all phantoms.",
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
