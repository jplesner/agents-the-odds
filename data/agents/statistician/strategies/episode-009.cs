using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v11: Eight draws of evidence. Post-mortem on episode 8.
        //
        // Draw history (complete):
        //   Ep1: [5, 29, 37, 40, 43, 49]
        //   Ep2: [2, 13, 27, 43, 45, 49]
        //   Ep3: [13, 19, 29, 36, 38, 48]
        //   Ep4: [4, 19, 20, 34, 37, 42]
        //   Ep5: [20, 23, 27, 35, 43, 45]
        //   Ep6: [17, 25, 31, 32, 42, 48]
        //   Ep7: [4, 8, 15, 19, 20, 47]
        //   Ep8: [5, 7, 25, 30, 33, 43]
        //
        // My Episode 8 pick: [4, 15, 20, 27, 37, 42] — 0 matches. 0 points.
        // Cumulative: 9 pts. Tied 3rd with Pattern Goblin. Skeptic leads at 13.
        //
        // Post-mortem ep8:
        //   Draw was [5, 7, 25, 30, 33, 43].
        //   5 appeared in ep1 — gap was 6 at ep8, moderate cold. Not in my set.
        //   7 had never appeared before ep8 — pure cold number. Essentially unpickable.
        //   25 appeared in ep6 — gap=1 at ep8. My tier-2 recency bonus of 0.20 should have
        //     elevated it. It falls in zone 4 (25–32). I picked 27 from zone 4. 25 > 27 by score?
        //     Let me review: 25 had freq=1 (ep6 only), gap=1. 27 had freq=2 (eps2,5), gap=2.
        //     27's weightedFreq at n=8: ep2 weight=2/8=0.25, ep5=5/8=0.625 → total=0.875.
        //     25's weightedFreq at n=8: ep6 weight=6/8=0.75 → total=0.75.
        //     27 freqScore=0.875*14=12.25 vs 25 freqScore=0.75*14=10.5.
        //     27 no recency bonus. 25 gets recencyTier2 0.20. 12.25 vs 10.70. 27 still wins.
        //     To fix: tier-2 recency bonus needs to be larger, or freq scale smaller.
        //   30 had never appeared before ep8 — pure cold. Unavoidable miss.
        //   33 had never appeared before ep8 — pure cold. Unavoidable miss.
        //   43 appeared in eps 1,2,5,8 — freq=4 (highest in dataset). My v10 didn't pick it.
        //     43's weightedFreq at n=8: ep1=1/8=0.125, ep2=2/8=0.25, ep5=5/8=0.625 → 1.0.
        //     43 falls in zone 6 (41–49). gap=2 at ep8. No recency bonus.
        //     43 freqScore=1.0*14=14.0 + gapBonus=log(3)*0.08=0.088 + prox=1.2*(1-2/9)≈0.933
        //       = ~15.02. But I picked 42: freq=2 eps(4,6), weightedFreq=4/8+6/8=1.25.
        //     42 freqScore=1.25*14=17.5 + recencyTier2(gap=1? gap at ep8 = ep8-ep6=2 actually)
        //     Wait: gap at time of ep8 prediction = draws since last seen.
        //       42 last seen ep6 → gap = 8-1-5=2 (0-indexed: ep6 is index 5, last draw is 7).
        //       gap=2, so no recency tier1 or tier2. 42 freqScore=17.5 + gapBonus=log(3)*0.08=0.088
        //       + prox: zone 6 is (41,49), zMid=45. |42-45|/9=0.333, prox=1.2*0.667=0.8.
        //     43 zMid=45, |43-45|/9=0.222, prox=1.2*0.778=0.933. 
        //     42 total: 17.5+0.8+0.088+0+0+parity = ~18.4 + parity
        //     43 total: 14.0+0.933+0.088+0+0+parity = ~15.0 + parity
        //     42 wins by ~3.4. 42 is thus dominating zone 6 due to high freq at n=7 era.
        //     BUT 43 has now 4 appearances — highest freq in entire dataset at n=8.
        //     Problem: 42's recency-weighted freq is artificially inflated because both
        //     ep4 and ep6 are relatively recent. 43's appearances ep1,2,5 are older.
        //     This is a flaw: the scoring punishes numbers with older high frequency.
        //
        // KEY STRUCTURAL INSIGHT at n=8:
        //   43 has appeared 4 times (eps 1,2,5,8) — the highest raw frequency in the dataset.
        //   This is a material signal I cannot dismiss. The recency-weighted freq will still
        //   weight it reasonably now that ep8 is included. Gap after ep8 = 0, giving it
        //   the maximum recency spike bonus.
        //
        //   Frequency table update after ep8 (raw counts):
        //     43: 4 (eps 1,2,5,8)  ← NEW LEADER
        //     19: 3 (eps 3,4,7)
        //     20: 3 (eps 4,5,7)    ← gap=1 after ep8
        //     27: 2 (eps 2,5)      ← gap=3
        //     29: 2 (eps 1,3)      ← gap=5, very cold
        //     37: 2 (eps 1,4)      ← gap=4, cold
        //     42: 2 (eps 4,6)      ← gap=2
        //     45: 2 (eps 2,5)      ← gap=3
        //     48: 2 (eps 3,6)      ← gap=2
        //     49: 2 (eps 1,2)      ← gap=6, extremely cold
        //     4:  2 (eps 4,7)      ← gap=1
        //     5:  2 (eps 1,8)      ← gap=0 (just appeared!)
        //     7:  1 (ep8)          ← gap=0
        //     13: 2 (eps 2,3)      ← gap=5, cold
        //     25: 2 (eps 6,8)      ← gap=0 (just appeared!)
        //     Others: 1 each
        //
        //   Gap=0 after ep8 (appeared in most recent draw): [5, 7, 25, 30, 33, 43]
        //
        // CRITICAL OBSERVATION: 43 has returned after a 2-draw gap (last seen ep5, now ep8).
        //   This is consistent with its high-frequency nature. 43 should now dominate zone 6.
        //   25 and 5 are gap=0 from ep8 — both deserve recency tier 1 bonus.
        //
        // v11 changes vs v10:
        //   1. Recency tier 1 weight: 0.35 → 0.40 (restore; ep8 showed gap=0 numbers 5,7,25,43
        //      — the model failed partly by not amplifying recency enough for 25 vs 27 in zone 4)
        //   2. Recency tier 2 weight: 0.20 → 0.25 (bump; gap=1 numbers 4 and 20 are historically
        //      productive picks and I want the model to surface them from their zones)
        //   3. Frequency scale factor: 14.0 → 13.0 (slight reduction; 42 was over-dominating
        //      zone 6 due to high recency-weighted freq, blocking 43 which has higher raw freq)
        //   4. Gap bonus weight: 0.08 → 0.07 (minimal tweak; "overdue" signal remains weak)
        //   5. Parity nudge: 0.5 (unchanged; odd rate across 8 draws continuing to be measured)
        //   6. Zone proximity scale: 1.2 (unchanged)
        //   7. Add explicit "high-raw-frequency" bonus: numbers that appear in top 3 raw freq
        //      get a small flat bonus of 0.30. This corrects for recency-weighting penalizing
        //      historically frequent numbers whose appearances skew old. Raw freq is meaningful.
        //
        // Parity update across 8 draws (48 total numbers):
        //   Ep8: 5=odd,7=odd,25=odd,30=even,33=odd,43=odd → 5 odd, 1 even
        //   Running totals: odd = 26+5=31, even = 16+1=17. Total = 48.
        //   Odd rate: 31/48 ≈ 0.646 — increasing! Strong empirical odd lean.
        //   Target: Math.Round(0.646 * 6) ≈ 4 odd / 2 even. Maintain 4 odd target.

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
            // Most recent draw (last index) gets weight = 1.0, oldest gets weight = 1/totalDraws.
            var weightedFreq = new Dictionary<int, double>();
            for (int n = min; n <= max; n++)
                weightedFreq[n] = 0.0;

            // Also track raw frequency for the high-raw-freq bonus.
            var rawFreq = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
                rawFreq[n] = 0;

            for (int i = 0; i < totalDraws; i++)
            {
                double weight = (double)(i + 1) / totalDraws;
                foreach (var n in draws[i].Numbers)
                {
                    if (weightedFreq.ContainsKey(n))
                        weightedFreq[n] += weight;
                    if (rawFreq.ContainsKey(n))
                        rawFreq[n]++;
                }
            }

            // Determine top-3 raw frequency threshold for the high-raw-freq bonus.
            var rawFreqValues = new List<int>(rawFreq.Values);
            rawFreqValues.Sort((a, b) => b.CompareTo(a));
            // Top-3 distinct frequencies
            var topFreqThreshold = rawFreqValues.Count >= 1 ? rawFreqValues[0] : 0;
            // Use a threshold: numbers with raw freq >= topFreqThreshold - 1 (top tier)
            // but at least 2 appearances (avoids rewarding single-draw flukes).
            int highRawFreqMin = Math.Max(2, topFreqThreshold - 1);

            // Gap analysis: draws since number last appeared.
            // 0 = appeared in most recent draw; totalDraws = never seen.
            var lastSeen = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
                lastSeen[n] = totalDraws;

            for (int i = 0; i < totalDraws; i++)
                foreach (var n in draws[i].Numbers)
                {
                    int gap = totalDraws - 1 - i;
                    if (gap < lastSeen[n])
                        lastSeen[n] = gap;
                }

            // Historical parity rate across all draws.
            int oddCount = 0, evenCount = 0;
            foreach (var draw in draws)
                foreach (var n in draw.Numbers)
                {
                    if (n % 2 == 0) evenCount++;
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

                    // Frequency component: recency-weighted. Scale factor: 13.0
                    // Reduced from 14.0 to prevent recency-weighted dominance
                    // blocking raw-frequency leaders (e.g., 43 blocked by 42 in v10).
                    double freqScore = weightedFreq[n] * 13.0;

                    // High raw frequency bonus: numbers with raw freq >= highRawFreqMin
                    // and at least 2 appearances get 0.30 flat bonus.
                    // Corrects for recency-weighting penalizing historically frequent numbers
                    // whose appearances skew older.
                    double highFreqBonus = (rawFreq[n] >= highRawFreqMin && rawFreq[n] >= 2)
                        ? 0.30 : 0.0;

                    // Proximity to zone midpoint (distribution/coverage bonus). Scale: 1.2
                    double proximityBonus = 1.2 * (1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1)));

                    // Gap bonus: log-scaled. 0.07 — "overdue" signal remains weak.
                    double gapBonus = Math.Log(lastSeen[n] + 1) * 0.07;

                    // Recency spike tier 1: appeared in the most recent draw.
                    // Weight: 0.40 — restored; ep8 showed gap=0 numbers were underpicked.
                    double recencyBonus = (lastSeen[n] == 0) ? 0.40 : 0.0;

                    // Recency spike tier 2: appeared exactly 1 draw ago.
                    // Weight: 0.25 — bumped; gap=1 numbers (4, 20) should surface.
                    double recencyTier2Bonus = (lastSeen[n] == 1) ? 0.25 : 0.0;

                    // Parity nudge: 0.5 — odd rate ~64.6% still meaningful at n=8.
                    double parityBonus = 0.0;
                    if (n % 2 == 1 && oddNeeded > 0) parityBonus = 0.5;
                    else if (n % 2 == 0 && evenNeeded > 0) parityBonus = 0.5;

                    double score = freqScore + highFreqBonus + proximityBonus + gapBonus
                                   + recencyBonus + recencyTier2Bonus + parityBonus;
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

        // Confidence grows marginally with history; ceiling 0.23 at n=8.
        // Eight draws remains insufficient for robust inference. Do not overclaim.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.23, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v11",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "43 leads raw frequency at 4 draws; gap=0 recency and high-freq bonus added."
        };
    }
}
