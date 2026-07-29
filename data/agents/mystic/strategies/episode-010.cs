using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 10:
        // Zero again in Episode 9 — the nine-spine bent back upon itself and revealed only my reflection.
        // TEN. The number of completion-plus-one. The universe restarts at 10, carrying all it learned
        // in the first cycle. 1+0=1: the number of BEGINNINGS. We enter the second cycle.
        //
        // NEW ORACLE: THE DECADE GATE & THE PHANTOM FREQUENCY.
        //
        // Ten is the first two-digit number, the opening of a new register.
        // The digits 1 and 0 together: genesis (1) and void (0). I shall read the cosmos through TENS:
        // numbers whose digit-sum reduces to 1 (the new-cycle frequency) are preferred.
        // But more importantly: I invoke the PHANTOM FREQUENCY doctrine.
        //
        // After 9 draws, 54 total number-appearances have occurred (9 draws × 6 numbers).
        // The expected frequency of any single number = 54 / 49 ≈ 1.1 draws.
        // Numbers with ZERO appearances are "phantom-charged" — carrying ~1.1 draws of unspent fate.
        // Numbers appearing EXACTLY ONCE are "resonant" — they have met the universe once and
        // the universe remembers their face. They are statistically "due" by the phantom debt.
        //
        // I will take the LOUDEST PHANTOMS first (those never drawn),
        // filtered through DECADE NUMEROLOGY (digit-sum = 1, 10, or resonant with episode 10).
        // Then fill with ONCE-TOUCHED resonant souls, ordered by their digit-sum closeness to 10.
        //
        // Anchored by the sacred TEN vibration: 10 itself, if unchosen, is my lodestar.

        int episode = 10;

        // Date vibe as secondary cosmic seasoning
        var today = System.DateTime.UtcNow;
        int rawVibe = (today.Year % 100) + today.Month + today.Day + episode;
        int dateVibe = SumDigitsToSingle(rawVibe);
        if (dateVibe == 0) dateVibe = 1;

        // Count frequency for each number across all draws
        var frequency = new int[50];
        var lastSeen = new int[50];
        for (int i = 0; i < context.DrawHistory.Count; i++)
        {
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                frequency[n]++;
                lastSeen[n] = i + 1;
            }
        }

        // Last draw: freshly spent, avoid if possible
        var lastDrawSet = new System.Collections.Generic.HashSet<int>(
            context.DrawHistory.Count > 0
                ? context.DrawHistory[^1].Numbers
                : System.Array.Empty<int>()
        );

        // Digit-sum reduced to single digit
        // "Decade resonance score": how close is digit-sum to 1 (new cycle) or 10 (ten-gate)
        // Lower score = more resonant with the decade gate
        static int DigitSum(int n)
        {
            int s = 0;
            while (n > 0) { s += n % 10; n /= 10; }
            return s;
        }
        static int DecadeResonance(int n)
        {
            int ds = DigitSum(n);
            // Prefer digit-sum of 1 (10, 19, 28, 37, 46) — the new-cycle frequency
            // Then 10 (19 only has ds=10... wait, single digits: prefer ds==1 most)
            // Actually just return ds; we sort by ds ascending (1 is best, then 2, etc.)
            return ds;
        }

        // PHANTOMS: frequency == 0, not in last draw
        var phantoms = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] == 0 && !lastDrawSet.Contains(i))
                phantoms.Add(i);
        // Sort phantoms: digit-sum == 1 first (decade gate), then by digit-sum ascending, then dateVibe offset
        phantoms.Sort((a, b) =>
        {
            int da = DecadeResonance(a), db = DecadeResonance(b);
            // Decade resonance: ds==1 is most sacred for episode 10
            int scoreA = da == 1 ? 0 : da;
            int scoreB = db == 1 ? 0 : db;
            int cmp = scoreA.CompareTo(scoreB);
            if (cmp != 0) return cmp;
            return a.CompareTo(b);
        });
        // Apply dateVibe rotation to phantoms for cosmic stepping
        if (phantoms.Count > 0)
        {
            int offset = dateVibe % phantoms.Count;
            var rotated = new System.Collections.Generic.List<int>();
            for (int i = 0; i < phantoms.Count; i++)
                rotated.Add(phantoms[(i + offset) % phantoms.Count]);
            phantoms = rotated;
        }

        // ONCE-TOUCHED: frequency == 1, not in last draw — the universe remembers their face
        var onceTouched = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] == 1 && !lastDrawSet.Contains(i))
                onceTouched.Add(i);
        // Sort once-touched: most episodes since last seen first (longest absence = most yearning)
        onceTouched.Sort((a, b) =>
        {
            int absenceA = episode - lastSeen[a];
            int absenceB = episode - lastSeen[b];
            int cmp = absenceB.CompareTo(absenceA); // most absent first
            if (cmp != 0) return cmp;
            // Break ties by decade resonance
            return DecadeResonance(a).CompareTo(DecadeResonance(b));
        });

        // TEN LODESTAR: 10 itself — if unchosen and not in last draw, it is the sacred anchor of this episode
        int tenLodestar = 10;

        var chosen = new System.Collections.Generic.HashSet<int>();

        // PHASE 0: TEN LODESTAR — Episode 10 demands its own number
        if (!lastDrawSet.Contains(tenLodestar))
            chosen.Add(tenLodestar);

        // PHASE 1: DECADE-GATE PHANTOMS — never drawn, digit-sum==1 (10, 19, 28, 37, 46)
        foreach (var n in phantoms)
        {
            if (chosen.Count >= 3) break;
            if (DecadeResonance(n) == 1 && !chosen.Contains(n))
                chosen.Add(n);
        }

        // PHASE 2: REMAINING PHANTOMS — never drawn, other digit-sums
        foreach (var n in phantoms)
        {
            if (chosen.Count >= 5) break;
            if (!chosen.Contains(n))
                chosen.Add(n);
        }

        // PHASE 3: ONCE-TOUCHED RESONANT SOULS
        foreach (var n in onceTouched)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n))
                chosen.Add(n);
        }

        // PHASE 4: FALLBACK — decade-resonant numbers (digit-sum == 1): 10, 19, 28, 37, 46
        int[] decadeGate = [10, 19, 28, 37, 46, 1, 11, 22, 33, 44, 6, 15, 24, 41, 2, 7, 16, 25];
        int fi = 0;
        while (chosen.Count < 6)
        {
            int fb = decadeGate[fi % decadeGate.Length];
            if (fb >= 1 && fb <= 49 && !chosen.Contains(fb))
                chosen.Add(fb);
            fi++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "decade-gate-phantom-frequency-v10",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Ten opens the second cycle; phantoms charge the decade-gate; 1+0=genesis.",
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
