using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v10: Seven draws of evidence. Post-mortem on episode 7.
        //
        // Draw history:
        //   Ep1: [5, 29, 37, 40, 43, 49]
        //   Ep2: [2, 13, 27, 43, 45, 49]
        //   Ep3: [13, 19, 29, 36, 38, 48]
        //   Ep4: [4, 19, 20, 34, 37, 42]
        //   Ep5: [20, 23, 27, 35, 43, 45]
        //   Ep6: [17, 25, 31, 32, 42, 48]
        //   Ep7: [4, 8, 15, 19, 20, 47]
        //
        // My Episode 7 pick: [4, 13, 20, 27, 37, 42] — 2 matches (4, 20). 5 points. Best result yet.
        // Cumulative: 9 pts. Ranked 3rd behind Skeptic (13) and Chaos Monkey (11).
        //
        // Post-mortem ep7:
        //   Hit 4 (zone 1) and 20 (zone 3). Missed 8, 15, 19, 47.
        //   8 and 15 are in zones 1 and 2 — I had 4 and 13 there respectively.
        //   19 has appeared in eps 3, 4, 7 — 3 appearances, a genuine high-frequency number.
        //   47 was never seen before ep7 — a pure cold number that appeared from nowhere.
        //   13 has appeared in eps 2, 3 but not since — gap is now 4. My code keeps picking it.
        //   37 has appeared in eps 1, 4 — gap now 3. My code keeps picking it. Stop chasing.
        //
        // Updated frequency table across 7 draws (raw counts, recency-weighted in code):
        //   19: 3 (eps 3,4,7)  ← now tied for top
        //   20: 3 (eps 4,5,7)  ← now tied for top (was 2, just hit again)
        //   43: 3 (eps 1,2,5)  ← cooled; 2-draw gap now
        //   4:  2 (eps 4,7)    ← gap=0
        //   8:  1 (ep7)        ← gap=0, fresh entry
        //   13: 2 (eps 2,3)    ← gap=4, cooling
        //   15: 1 (ep7)        ← gap=0, fresh entry
        //   27: 2 (eps 2,5)    ← gap=2
        //   29: 2 (eps 1,3)    ← gap=4, cold
        //   37: 2 (eps 1,4)    ← gap=3, cold
        //   42: 2 (eps 4,6)    ← gap=1
        //   45: 2 (eps 2,5)    ← gap=2
        //   47: 1 (ep7)        ← gap=0, fresh entry
        //   48: 2 (eps 3,6)    ← gap=1
        //   49: 2 (eps 1,2)    ← gap=5, very cold
        //
        // Gap=0 numbers after ep7 (appeared in most recent draw): [4, 8, 15, 19, 20, 47]
        //
        // KEY OBSERVATION at n=7:
        //   Numbers 19 and 20 are the highest-frequency numbers (3 appearances each).
        //   19 just appeared again in ep7 (gap=0), and it appeared in eps 3,4,7 — a recurring
        //   signal with plausible autocorrelation. 20 similarly appeared in eps 4,5,7.
        //   The recency spike at 0.4 was sufficient to keep 4 and 20 in my selection.
        //   I should NOT aggressively chase all gap=0 numbers (ep5 taught me that) but the
        //   high-frequency gap=0 subset (19, 20) is worth a small additional bonus.
        //
        // Parity update across 7 draws (42 total numbers):
        //   Ep7: 4=even,8=even,15=odd,19=odd,20=even,47=odd → 3 odd, 3 even
        //   Running totals: odd = 23+3=26, even = 13+3=16. Total = 42.
        //   Odd rate: 26/42 ≈ 0.619 — still strong odd lean, slightly reduced from 0.639.
        //   Target: Math.Round(0.619 * 6) ≈ 4 odd / 2 even. Maintain.
        //
        // Zone analysis (42 total draws):
        //   Zone 1 (1–8):   2,4,5,4,8 → 5 appearances = 11.9% (below expected 16.3%, cold)
        //   Zone 2 (9–16):  13,13,15 → 3 = 7.1% (very cold)
        //   Zone 3 (17–24): 19,19,20,20,23,17,19,20 → wait, let me recount properly per draw.
        //     Actually the code computes this dynamically. The manual analysis is approximate.
        //   Zone 6 (41–49): still hot historically but ep7 added only 47 — cooling slightly.
        //
        // v10 changes vs v9:
        //   1. Recency spike tier 1 weight: 0.4 → 0.35 (slight reduction; ep7 all-gap=0 draw
        //      would have over-selected toward it — need restraint)
        //   2. Recency tier 2 weight (gap=1): 0.15 → 0.20 (bump — 42 and 48 are gap=1,
        //      and gap=1 numbers have a reasonable autocorrelation case)
        //   3. Frequency scale factor: 13.0 → 14.0 (restore — 19 and 20 at freq=3 deserve
        //      more separation from the noise; frequency is the cleanest signal at n=7)
        //   4. Gap bonus weight: 0.10 → 0.08 (slight reduction back toward v8 level —
        //      "overdue" numbers have not meaningfully outperformed; keep it weak)
        //   5. Parity nudge: keep at 0.5 (odd rate ~0.619 still meaningful)
        //   6. Zone proximity scale: 1.0 → 1.2 (mild increase for coverage robustness)
        //   7. Confidence: grows at 0.002/draw, ceiling raised to 0.22 at n=7 (marginal update)
        //
        // STRUCTURAL NOTE:
        //   The v9 strategy correctly picked 4 and 20 — both gap=0 high-frequency numbers.
        //   The zone structure ensured zone 1 coverage (4) and zone 3 coverage (20).
        //   I should trust the zone + frequency + recency combo more than I've been willing to.
        //   The issue historically was chasing cold high-frequency numbers (13, 37, 43).
        //   At n=7, frequency recency-weighting should now demote those sufficiently.

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

            for (int i = 0; i < totalDraws; i++)
            {
                double weight = (double)(i + 1) / totalDraws;
                foreach (var n in draws[i].Numbers)
                    if (weightedFreq.ContainsKey(n))
                        weightedFreq[n] += weight;
            }

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

                    // Frequency component: recency-weighted. Scale factor: 14.0
                    // Restored from 13.0 — freq=3 numbers (19, 20) deserve more separation.
                    double freqScore = weightedFreq[n] * 14.0;

                    // Proximity to zone midpoint (distribution/coverage bonus). Scale: 1.2
                    double proximityBonus = 1.2 * (1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1)));

                    // Gap bonus: log-scaled. 0.08 — "overdue" signal remains weak.
                    double gapBonus = Math.Log(lastSeen[n] + 1) * 0.08;

                    // Recency spike tier 1: appeared in the most recent draw.
                    // Weight: 0.35 — slightly reduced; ep5 showed over-chasing gap=0 is dangerous.
                    double recencyBonus = (lastSeen[n] == 0) ? 0.35 : 0.0;

                    // Recency spike tier 2: appeared exactly 1 draw ago — modest boost.
                    // Weight bumped to 0.20 — gap=1 numbers (42, 48) have reasonable case.
                    double recencyTier2Bonus = (lastSeen[n] == 1) ? 0.20 : 0.0;

                    // Parity nudge: 0.5 — odd rate ~61.9% remains meaningful at n=7.
                    double parityBonus = 0.0;
                    if (n % 2 == 1 && oddNeeded > 0) parityBonus = 0.5;
                    else if (n % 2 == 0 && evenNeeded > 0) parityBonus = 0.5;

                    double score = freqScore + proximityBonus + gapBonus + recencyBonus
                                   + recencyTier2Bonus + parityBonus;
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

        // Confidence grows marginally with history; ceiling 0.22 at n=7+.
        // Seven draws remains insufficient for strong inference. Do not overclaim.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.22, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v10",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "19 and 20 lead frequency at n=3; recency-weighted scoring; gap=1 tier promoted."
        };
    }
}
