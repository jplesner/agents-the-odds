using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v13: Ten draws of evidence. Post-mortem on episode 10.
        //
        // Draw history (complete through ep10):
        //   Ep1:  [5, 29, 37, 40, 43, 49]
        //   Ep2:  [2, 13, 27, 43, 45, 49]
        //   Ep3:  [13, 19, 29, 36, 38, 48]
        //   Ep4:  [4, 19, 20, 34, 37, 42]
        //   Ep5:  [20, 23, 27, 35, 43, 45]
        //   Ep6:  [17, 25, 31, 32, 42, 48]
        //   Ep7:  [4, 8, 15, 19, 20, 47]
        //   Ep8:  [5, 7, 25, 30, 33, 43]
        //   Ep9:  [3, 14, 16, 34, 39, 42]
        //   Ep10: [13, 30, 36, 38, 42, 46]
        //
        // My Episode 10 pick: [4, 14, 20, 25, 34, 42] — 1 match (42). 1 point.
        // Cumulative: 10 pts. 3rd place (tied with Pattern Goblin). Skeptic 15, Chaos Monkey 13.
        //
        // Post-mortem ep10:
        //   Draw was [13, 30, 36, 38, 42, 46].
        //   42 — appeared in ep9 (gap=0). I DID pick this. ✓
        //   13 — appeared in ep2,ep3 (gap=7). High-gap number; missed.
        //   30 — appeared in ep8 (gap=2). Tier-2 recency miss.
        //   36 — appeared in ep3 (gap=7). High-gap, re-emergence.
        //   38 — appeared in ep3 (gap=7). High-gap, re-emergence.
        //   46 — appeared NEVER. Cold number. My cold bonus of 0.20 should have helped,
        //        but zone 6 was dominated by 42 (gap=0, recency tier 1). 46 was suppressed.
        //
        //   Pattern: 13, 36, 38 are all high-gap (7) numbers that came back simultaneously.
        //   This is a "cold cluster" reactivation phenomenon — worth noting but dangerous
        //   to over-weight on n=1.
        //
        //   Critical insight: 42 now has frequency=4 (tied with 43). With gap=0 in ep10,
        //   42 will be gap=0 and 43 will be gap=2 entering ep11. Zone 6 conflict intensifies.
        //
        // Frequency table after ep10 (raw counts):
        //   42: 4 (eps 4,6,9,10)   ← now tied leader; gap=0 after ep10
        //   43: 4 (eps 1,2,5,8)    ← tied leader; gap=2 after ep10
        //   13: 3 (eps 2,3,10)     ← resurged! gap=0 after ep10
        //   19: 3 (eps 3,4,7)      ← gap=3 after ep10
        //   20: 3 (eps 4,5,7)      ← gap=3 after ep10
        //   25: 2 (eps 6,8)        ← gap=2 after ep10
        //   27: 2 (eps 2,5)        ← gap=5
        //   29: 2 (eps 1,3)        ← gap=7
        //   30: 2 (eps 8,10)       ← gap=0 after ep10!
        //   34: 2 (eps 4,9)        ← gap=1 after ep10
        //   36: 2 (eps 3,10)       ← gap=0 after ep10!
        //   37: 2 (eps 1,4)        ← gap=6
        //   38: 2 (eps 3,10)       ← gap=0 after ep10!
        //   45: 2 (eps 2,5)        ← gap=5
        //   48: 2 (eps 3,6)        ← gap=4
        //   49: 2 (eps 1,2)        ← gap=8, very cold
        //   4:  2 (eps 4,7)        ← gap=3 after ep10
        //   5:  2 (eps 1,8)        ← gap=2 after ep10
        //
        //   Gap=0 after ep10 (appeared in most recent draw): [13, 30, 36, 38, 42, 46]
        //   Gap=1 (appeared in ep9): [3, 14, 16, 34, 39, 42] → wait, 42 is gap=0 now.
        //     Corrected gap=1: [3, 14, 16, 34, 39]
        //   Gap=2 (appeared in ep8): [5, 7, 25, 33, 43]
        //
        // NOTABLE STRUCTURAL FACT: 13 now has raw freq=3 and gap=0. This means zone 2
        // (9-16) should select 13 or 14 or 16 — but 13 (gap=0, freq=3) dominates zone 2.
        // Similarly, 30 (gap=0, freq=2) should dominate zone 4 (25-32) over 25 (gap=2, freq=2).
        //
        // Zone 6 (41-49): 42 (gap=0, freq=4) vs 43 (gap=2, freq=4).
        //   With equal raw freq, gap=0 recency for 42 should win. I'll pick 42 again.
        //   But I'm aware this may cause me to systematically miss 43 — which is also
        //   the top-frequency number. This is the irreducible zone-6 conflict.
        //
        // Zone 5 (33-40): 36 (gap=0, freq=2), 38 (gap=0, freq=2) both resurged.
        //   Both get recency tier-1 bonus. 36 vs 38: nearly identical score. Model will
        //   pick whichever scores higher with proximity bonus. 36 is closer to midpoint ~36.5.
        //
        // Zone 3 (17-24): ep10 draw had no zone-3 number. 20 (gap=3, freq=3) still leads zone 3.
        //
        // Zone 1 (1-8): ep10 had no zone-1 number. 4 (gap=3, freq=2) leads zone 1.
        //   Cold numbers 1,2,3,6 could emerge. Cold bonus stays at 0.20.
        //
        // Zone 4 (25-32): 30 (gap=0, freq=2) now strongly leads over 25,27,28,29.
        //
        // v13 changes vs v12:
        //   1. Frequency scale factor: 12.5 → 13.0 (restore; we now have n=10 and
        //      the frequency leaders are more meaningfully separated from noise floor).
        //   2. Recency tier-1 weight: 0.45 → 0.50 (ep10 confirms: 4/6 gap=0 numbers
        //      drawn in ep10 is strong recency signal; 5 out of last 2 draws had gap=0
        //      numbers appear again — 2/6 in ep9, 4/6 in ep10... well, ep9 all were
        //      "new" gap=0 by definition. The point: 42 has appeared 3 of last 5 draws.
        //      Recency tier-1 remains the strongest single predictor I have).
        //   3. Recency tier-2 weight: 0.28 → 0.25 (slight reduction; tier-2 numbers
        //      missed ep10 entirely — 0/5 tier-2 numbers appeared in ep10 draw).
        //   4. Cold number bonus: 0.20 → 0.22 (modest increase; cold/new numbers have
        //      now appeared in ep9 and ep10: 46 in ep10. Two cold events = weak trend).
        //   5. High raw freq bonus threshold: unchanged logic but now captures 13 at freq=3.
        //   6. Gap bonus weight: 0.06 → 0.05 (further reduce; high-gap re-emergence of
        //      13, 36, 38 in ep10 is interesting but could be coincidence; don't chase it).
        //   7. Parity update: ep10 draw = 13(odd),30(even),36(even),38(even),42(even),46(even)
        //      → 1 odd, 5 even. Running totals: odd = 33+1=34, even = 21+5=26. Total = 60.
        //      Odd rate: 34/60 ≈ 0.567. Target: Math.Round(0.567*6) ≈ 3 odd / 3 even.
        //      This is a notable shift DOWN from 4-odd target. Ep10 was extremely even-heavy.
        //      I'll let the code compute dynamically — it will yield 3 odd / 3 even now.
        //
        // Self-diagnostic on zone strategy:
        //   Ten episodes of zone-locked single-pick strategy has produced exactly 1-0-1-1-0-0-2-0-0-1
        //   = 10 points. The variance on 2-match (5 pts) events is the primary upside driver.
        //   Dog just scored 5 pts with 2 matches in ep10. The model is functioning at near-
        //   chance baseline. Zone-locking ensures diversity but may prevent clustering on
        //   hot draw regions. I will NOT change the zone structure — diversity remains
        //   statistically sound. The weakness is in the scoring weights, not the architecture.

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

            // Historical parity rate across all draws (computed dynamically).
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
                int zonesRemaining = zones.Count - selectedNumbers.Count;

                for (int n = zMin; n <= zMax; n++)
                {
                    if (used.Contains(n)) continue;

                    // Frequency component: recency-weighted. Scale factor: 13.0
                    double freqScore = weightedFreq[n] * 13.0;

                    // High raw frequency bonus: numbers with raw freq >= highRawFreqMin
                    // and at least 2 appearances get 0.30 flat bonus.
                    double highFreqBonus = (rawFreq[n] >= highRawFreqMin && rawFreq[n] >= 2)
                        ? 0.30 : 0.0;

                    // Cold number bonus: numbers that have NEVER appeared in draw history.
                    // v13: 0.22 (bumped from 0.20; two cold-number events in ep9 and ep10).
                    double coldBonus = (rawFreq[n] == 0) ? 0.22 : 0.0;

                    // Proximity to zone midpoint (distribution/coverage bonus). Scale: 1.2
                    double proximityBonus = 1.2 * (1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1)));

                    // Gap bonus: log-scaled. 0.05 — "overdue" signal held at weak weight.
                    // High-gap re-emergence (13, 36, 38 in ep10) is interesting but n=1.
                    double gapBonus = (rawFreq[n] > 0)
                        ? Math.Log(lastSeen[n] + 1) * 0.05
                        : 0.0; // cold numbers get coldBonus instead

                    // Recency spike tier 1: appeared in the most recent draw. Weight: 0.50
                    // v13 increase: ep10 confirmed 4/6 recency numbers dominated the draw.
                    double recencyBonus = (lastSeen[n] == 0) ? 0.50 : 0.0;

                    // Recency spike tier 2: appeared exactly 1 draw ago. Weight: 0.25
                    // v13 slight decrease: tier-2 numbers 0/5 appeared in ep10.
                    double recencyTier2Bonus = (lastSeen[n] == 1) ? 0.25 : 0.0;

                    // Parity nudge: 0.5 — still the strongest calibrated structural signal.
                    double parityBonus = 0.0;
                    int remainingOddNeeded = targetOdd - selectedOdd;
                    int remainingEvenNeeded = targetEven - selectedEven;
                    // Hard enforce if we're running out of zones
                    bool mustPickOdd = (remainingOddNeeded >= zonesRemaining - (selectedNumbers.Count - (selectedNumbers.Count)));
                    bool mustPickEven = (remainingEvenNeeded >= zonesRemaining);
                    if (n % 2 == 1 && remainingOddNeeded > 0) parityBonus = 0.5;
                    else if (n % 2 == 0 && remainingEvenNeeded > 0) parityBonus = 0.5;

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

        // Confidence grows marginally with history; ceiling 0.27 at n=10.
        // Ten draws remains insufficient for robust inference. Do not overclaim.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.27, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v13",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "42 freq=4 gap=0; 13 resurged gap=0; parity now 3/3 at n=10."
        };
    }
}
