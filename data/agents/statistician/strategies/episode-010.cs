using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v12: Nine draws of evidence. Post-mortem on episode 9.
        //
        // Draw history (complete through ep9):
        //   Ep1: [5, 29, 37, 40, 43, 49]
        //   Ep2: [2, 13, 27, 43, 45, 49]
        //   Ep3: [13, 19, 29, 36, 38, 48]
        //   Ep4: [4, 19, 20, 34, 37, 42]
        //   Ep5: [20, 23, 27, 35, 43, 45]
        //   Ep6: [17, 25, 31, 32, 42, 48]
        //   Ep7: [4, 8, 15, 19, 20, 47]
        //   Ep8: [5, 7, 25, 30, 33, 43]
        //   Ep9: [3, 14, 16, 34, 39, 42]
        //
        // My Episode 9 pick: [4, 15, 20, 25, 33, 43] — 0 matches. 0 points.
        // Cumulative: 9 pts. 3rd place. Skeptic 14, Chaos Monkey 13.
        //
        // Post-mortem ep9:
        //   Draw was [3, 14, 16, 34, 39, 42].
        //   3  — never appeared before. Pure cold. Unpickable.
        //   14 — never appeared before. Pure cold. Unpickable.
        //   16 — never appeared before. Pure cold. Unpickable.
        //   34 — appeared ep4. Gap=4 at ep9. Cold.
        //   39 — never appeared before. Pure cold. Unpickable.
        //   42 — appeared eps 4,6. Gap=2 at ep9. My v11 did NOT pick 42 — I had demoted it
        //        after ep8 to surface 43. But 43 didn't draw in ep9 either.
        //
        //   Critical observation: ep9 had FOUR brand-new numbers (3, 14, 16, 39) — numbers
        //   that had never appeared in 8 previous draws. This is statistically notable.
        //   Out of 9 draws × 6 numbers = 54 total draws, with 49 possible numbers, the
        //   "cold" vs "seen" distribution is shifting. At n=9, 36 distinct numbers have
        //   appeared, leaving only 13 numbers that have NEVER been drawn. Those 13 cold
        //   numbers hit at a rate of 4/6 = 66.7% of ep9's draw — an extreme cold episode.
        //
        //   The implication: my zone-based strategy with frequency weighting is systematically
        //   under-selecting cold numbers. If cold numbers hit at 4/6 in ep9, maybe I should
        //   include at least 1–2 "long cold" numbers from zones where cold numbers cluster.
        //
        //   However: n=9 is still small. One cold-dominated episode is weak evidence of a
        //   structural pattern. I will not overreact, but I will add a modest cold-number
        //   bonus for numbers that have NEVER appeared in the draw history.
        //
        // Frequency table after ep9 (raw counts, sorted by frequency):
        //   43: 4 (eps 1,2,5,8)   ← still leader; gap=1 after ep9
        //   19: 3 (eps 3,4,7)     ← gap=2 after ep9
        //   20: 3 (eps 4,5,7)     ← gap=2 after ep9
        //   42: 3 (eps 4,6,9)     ← NOW 3! gap=0 after ep9
        //   13: 2 (eps 2,3)       ← gap=6
        //   25: 2 (eps 6,8)       ← gap=1 after ep9
        //   27: 2 (eps 2,5)       ← gap=4
        //   29: 2 (eps 1,3)       ← gap=6
        //   37: 2 (eps 1,4)       ← gap=5
        //   45: 2 (eps 2,5)       ← gap=4
        //   48: 2 (eps 3,6)       ← gap=3
        //   49: 2 (eps 1,2)       ← gap=7, very cold
        //   4:  2 (eps 4,7)       ← gap=2 after ep9
        //   5:  2 (eps 1,8)       ← gap=1 after ep9
        //   34: 2 (eps 4,9)       ← gap=0 after ep9
        //   Others: 1 each
        //
        //   Gap=0 after ep9 (appeared in most recent draw): [3, 14, 16, 34, 39, 42]
        //   Gap=1 (appeared in ep8): [5, 7, 25, 30, 33, 43]
        //   Gap=2 (appeared in ep7): [4, 8, 15, 19, 20, 47]
        //
        // KEY UPDATE: 42 now ties 19 and 20 at raw freq=3, gap=0.
        //   43 still leads at raw freq=4, gap=1.
        //   Both 42 and 43 should now be heavily favored in zone 6 (41–49).
        //   I can only pick ONE from zone 6. With 42 at gap=0 (recency tier 1) and
        //   43 at gap=1 (recency tier 2) plus higher raw freq... this is a genuine conflict.
        //   The model should resolve it by score. Let me ensure both are weighted fairly.
        //
        // v12 changes vs v11:
        //   1. Add cold-number bonus: numbers that have NEVER appeared in draw history
        //      receive a small flat bonus of 0.20. This is a first-order correction for
        //      the systematic under-representation of cold numbers in my predictions.
        //      Caveat: this is a 1-episode signal; I'm weighting it lightly.
        //   2. Frequency scale factor: 13.0 → 12.5 (modest reduction to tighten the spread
        //      between 42 and 43 in zone 6, so recency and raw-freq bonuses matter more).
        //   3. Recency tier 1 weight: 0.40 → 0.45 (ep9 confirms gap=0 numbers are frequent;
        //      42 at gap=0 should have strong elevation in zone 6).
        //   4. Recency tier 2 weight: 0.25 → 0.28 (modest bump; 43 at gap=1 should follow).
        //   5. High raw freq bonus threshold: keep at rawFreq >= (topFreq - 1) and >= 2.
        //      This will now include 42 (freq=3) and 43 (freq=4) both getting the bonus.
        //   6. Gap bonus weight: 0.07 → 0.06 (further reduce; "overdue" signal still weak).
        //   7. Parity nudge: unchanged at 0.5.
        //   8. Zone proximity scale: unchanged at 1.2.
        //   9. Cold number bonus: 0.20 for numbers with zero appearances.
        //
        // Parity update across 9 draws (54 total numbers):
        //   Ep9: 3=odd,14=even,16=even,34=even,39=odd,42=even → 2 odd, 4 even
        //   Running totals: odd = 31+2=33, even = 17+4=21. Total = 54.
        //   Odd rate: 33/54 ≈ 0.611 — declining from 0.646; still above 0.5.
        //   Target: Math.Round(0.611 * 6) ≈ 4 odd / 2 even. Maintain 4 odd target.

        var rules = context.Rules;
        int min = rules.MinNumber;   // 1
        int max = rules.MaxNumber;   // 49
        int drawCount = rules.DrawCount; // 6

        var draws = context.DrawHistory;

        List<int> selectedNumbers;

        if (draws == null || draws.Count == 0)
        {
            selectedNumbers = new List<int> { 5, 14, 19, 28, 37, 44 };
        }
        else
        {
            int totalDraws = draws.Count;

            // Build recency-weighted frequency table.
            // Most recent draw gets weight = 1.0, oldest gets weight = 1/totalDraws.
            var weightedFreq = new Dictionary<int, double>();
            for (int n = min; n <= max; n++)
                weightedFreq[n] = 0.0;

            var rawFreq = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
                rawFreq[n] = 0;

            for (int i = 0; i < totalDraws; i++)
            {
                double weight = (double)(i + 1) / totalDraws;
                foreach (var num in draws[i].Numbers)
                {
                    if (weightedFreq.ContainsKey(num))
                        weightedFreq[num] += weight;
                    if (rawFreq.ContainsKey(num))
                        rawFreq[num]++;
                }
            }

            // Determine top raw frequency for high-raw-freq bonus threshold.
            var rawFreqValues = new List<int>(rawFreq.Values);
            rawFreqValues.Sort((a, b) => b.CompareTo(a));
            int topFreqThreshold = rawFreqValues.Count >= 1 ? rawFreqValues[0] : 0;
            // Numbers with raw freq >= topFreqThreshold - 1 and at least 2 appearances.
            int highRawFreqMin = Math.Max(2, topFreqThreshold - 1);

            // Gap analysis: draws since number last appeared.
            // 0 = appeared in most recent draw; totalDraws = never seen.
            var lastSeen = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
                lastSeen[n] = totalDraws; // sentinel: never seen

            for (int i = 0; i < totalDraws; i++)
                foreach (var num in draws[i].Numbers)
                {
                    int gap = totalDraws - 1 - i;
                    if (gap < lastSeen[num])
                        lastSeen[num] = gap;
                }

            // Historical parity rate across all draws.
            int oddCount = 0, evenCount = 0;
            foreach (var draw in draws)
                foreach (var num in draw.Numbers)
                {
                    if (num % 2 == 0) evenCount++;
                    else oddCount++;
                }
            double oddRate = (oddCount + evenCount) > 0
                ? (double)oddCount / (oddCount + evenCount)
                : 0.5;

            // Zones: 6 equal-ish bands across 1–49. One pick per zone for coverage.
            var zones = new List<(int zMin, int zMax)>
            {
                (1, 8), (9, 16), (17, 24), (25, 32), (33, 40), (41, 49)
            };

            selectedNumbers = new List<int>();
            var used = new HashSet<int>();
            int selectedOdd = 0, selectedEven = 0;

            int targetOdd = (int)Math.Round(oddRate * drawCount);
            int targetEven = drawCount - targetOdd;

            foreach (var (zMin, zMax) in zones)
            {
                double zMid = (zMin + zMax) / 2.0;
                int best = -1;
                double bestScore = double.MinValue;

                int oddNeeded = targetOdd - selectedOdd;
                int evenNeeded = targetEven - selectedEven;

                for (int n = zMin; n <= zMax; n++)
                {
                    if (used.Contains(n)) continue;

                    // Frequency component: recency-weighted. Scale factor: 12.5
                    double freqScore = weightedFreq[n] * 12.5;

                    // High raw frequency bonus: numbers with raw freq >= highRawFreqMin
                    // and at least 2 appearances get 0.30 flat bonus.
                    double highFreqBonus = (rawFreq[n] >= highRawFreqMin && rawFreq[n] >= 2)
                        ? 0.30 : 0.0;

                    // Cold number bonus: numbers that have NEVER appeared in draw history.
                    // New in v12: weak signal (0.20) correcting systematic cold-number miss.
                    // Ep9 had 4/6 cold numbers — weak 1-episode evidence, held lightly.
                    double coldBonus = (rawFreq[n] == 0) ? 0.20 : 0.0;

                    // Proximity to zone midpoint (distribution/coverage bonus). Scale: 1.2
                    double proximityBonus = 1.2 * (1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1)));

                    // Gap bonus: log-scaled. 0.06 — "overdue" signal remains weak.
                    double gapBonus = (rawFreq[n] > 0)
                        ? Math.Log(lastSeen[n] + 1) * 0.06
                        : 0.0; // cold numbers get coldBonus instead

                    // Recency spike tier 1: appeared in the most recent draw. Weight: 0.45
                    double recencyBonus = (lastSeen[n] == 0) ? 0.45 : 0.0;

                    // Recency spike tier 2: appeared exactly 1 draw ago. Weight: 0.28
                    double recencyTier2Bonus = (lastSeen[n] == 1) ? 0.28 : 0.0;

                    // Parity nudge: 0.5
                    double parityBonus = 0.0;
                    if (n % 2 == 1 && oddNeeded > 0) parityBonus = 0.5;
                    else if (n % 2 == 0 && evenNeeded > 0) parityBonus = 0.5;

                    double score = freqScore + highFreqBonus + coldBonus + proximityBonus
                                   + gapBonus + recencyBonus + recencyTier2Bonus + parityBonus;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        best = n;
                    }
                }

                if (best != -1)
                {
                    selectedNumbers.Add(best);
                    used.Add(best);
                    if (best % 2 == 1) selectedOdd++;
                    else selectedEven++;
                }
            }

            // Safety pad to exactly 6 numbers.
            if (selectedNumbers.Count < drawCount)
            {
                for (int n = min; n <= max && selectedNumbers.Count < drawCount; n++)
                    if (!used.Contains(n))
                    {
                        selectedNumbers.Add(n);
                        used.Add(n);
                    }
            }

            selectedNumbers.Sort();
        }

        // Confidence grows marginally with history; ceiling 0.25 at n=9.
        // Nine draws remains insufficient for robust inference. Do not overclaim.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.25, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v12",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "42 gap=0 recency; 43 raw-freq leader; cold-number bonus added for ep9 signal."
        };
    }
}
