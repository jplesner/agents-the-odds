using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v9: Six draws of evidence now available. Post-mortem on episodes 5 & 6.
        //
        // Draw history:
        //   Ep1: [5, 29, 37, 40, 43, 49]
        //   Ep2: [2, 13, 27, 43, 45, 49]
        //   Ep3: [13, 19, 29, 36, 38, 48]
        //   Ep4: [4, 19, 20, 34, 37, 42]
        //   Ep5: [20, 23, 27, 35, 43, 45]
        //   Ep6: [17, 25, 31, 32, 42, 48]
        //
        // My Episode 6 pick: [4, 13, 20, 27, 35, 43] — 0 matches. Second consecutive zero.
        // Cumulative: 4 pts total. I am tied for last among competitive agents.
        //
        // Critical post-mortem:
        //   Ep6 draw [17,25,31,32,42,48] — NONE of these had gap=0 from ep5.
        //   My v8 strategy heavily weighted gap=0 numbers (ep5 draw), but ep6 was entirely
        //   composed of numbers with gap >= 2 (17: never seen; 25: never seen; 31: never seen;
        //   32: never seen; 42: ep4 = gap 2; 48: ep3 = gap 3). The recency spike failed
        //   catastrophically again — just in the opposite direction from ep5.
        //
        // KEY INSIGHT from n=6:
        //   The recency signal is too volatile to trust directionally. In ep5, four gap=0.
        //   In ep6, zero gap=0. These are inconsistent signals. I've been chasing the last draw.
        //   I need to reduce recency spike weight significantly and return to balanced scoring.
        //
        // Frequency table across 6 draws (raw counts):
        //   43: 4 (eps 1,2,3,5) — but not in ep4 or ep6: cooling
        //   13: 2 (eps 2,3)
        //   19: 2 (eps 3,4)
        //   20: 2 (eps 4,5)
        //   27: 2 (eps 2,5)
        //   29: 2 (eps 1,3)
        //   37: 2 (eps 1,4)
        //   45: 2 (eps 2,5)
        //   48: 2 (eps 3,6) — gap=0 (appeared ep6)
        //   42: 2 (eps 4,6) — gap=0 (appeared ep6)
        //   49: 2 (eps 1,2)
        //
        // Gap=0 numbers after ep6: [17, 25, 31, 32, 42, 48]
        //
        // Zone representation across 6 draws:
        //   Zone 1 (1–8):   2, 4, 5 → 3 appearances in 36 total numbers = 8.3% (expected ~16.3%)
        //   Zone 2 (9–16):  13, 13 → 2 appearances = 5.6% (very cold)
        //   Zone 3 (17–24): 19,19,20,20,23,17 → 6 = 16.7% (on par)
        //   Zone 4 (25–32): 27,27,29,29,25,31,32 → 7 = 19.4% (warm)
        //   Zone 5 (33–40): 34,35,36,37,37,38,40 → 7 = 19.4% (warm)
        //   Zone 6 (41–49): 42,42,43,43,43,43,45,45,48,48,49,49 → 12 = 33.3% (very hot)
        //
        // Parity across 6 draws: 36 total numbers drawn.
        //   Odd:  5,29,37,43,49, 13,27,43,45,49, 13,19,29,37,43, 19,37, 23,27,35,43,45, 17,25,31
        //   Actually let me count: odd numbers in draws:
        //     Ep1: 5,29,37,43,49 = 5 odd, 1 even (40)
        //     Ep2: 13,27,43,45,49 = 5 odd, 1 even (2)
        //     Ep3: 13,19,29,37,43 = 5 odd, 1 even (36,38,48 wait: 13,19,29,36,38,48)
        //          13=odd,19=odd,29=odd,36=even,38=even,48=even → 3 odd, 3 even
        //     Ep4: 4=even,19=odd,20=even,34=even,37=odd,42=even → 2 odd, 4 even
        //     Ep5: 20=even,23=odd,27=odd,35=odd,43=odd,45=odd → 5 odd, 1 even
        //     Ep6: 17=odd,25=odd,31=odd,32=even,42=even,48=even → 3 odd, 3 even
        //   Total: odd = 5+5+3+2+5+3=23, even = 1+1+3+4+1+3=13. (Wait that's 36 total. ✓)
        //   Odd rate: 23/36 ≈ 0.639 — strong odd lean.
        //   Target: Math.Round(0.639 * 6) = 4 odd / 2 even.
        //
        // v9 changes vs v8:
        //   1. Recency spike tier 1 weight: 0.8 → 0.4 (halved — volatile signal, unreliable)
        //   2. Recency tier 2 weight: 0.2 → 0.15 (modest demotion)
        //   3. Frequency scale factor: 14.0 → 13.0 (slight pullback; 43 cooling)
        //   4. Gap bonus weight: 0.08 → 0.10 (modest restoration — ep6 draw was all high-gap)
        //   5. Zone midpoint proximity: keep at 1.0 scale
        //   6. Parity nudge: 0.4 → 0.5 (odd rate 63.9% is significant; reinforce)
        //   7. Confidence: grows with draws, ceiling 0.20 (marginally raised at n=6)
        //
        // The core structural issue: I am getting 0 matches. The scoring table gives 1pt for
        // ANY single match. I need to diversify zone coverage to increase the probability of
        // at least one match per draw — which the zonal approach already handles structurally.
        // The problem is my within-zone picks are consistently wrong. The frequency weighting
        // keeps pulling me toward 43, 13, 19, 20 — numbers that have cooled. Need fresher picks.

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

                int zonesLeft = zones.Count - selectedNumbers.Count;
                int oddNeeded = targetOdd - selectedOdd;
                int evenNeeded = targetEven - selectedEven;

                for (int n = zMin; n <= zMax; n++)
                {
                    if (used.Contains(n)) continue;

                    // Frequency component: recency-weighted.
                    // Scale factor: 13.0
                    double freqScore = weightedFreq[n] * 13.0;

                    // Proximity to zone midpoint (distribution/coverage bonus).
                    double proximityBonus = 1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1));

                    // Gap bonus: log-scaled. 0.10 — modest restoration after ep6 all-high-gap draw.
                    double gapBonus = Math.Log(lastSeen[n] + 1) * 0.10;

                    // Recency spike tier 1: appeared in the most recent draw.
                    // Weight reduced to 0.4 — signal too volatile to trust heavily (n=6).
                    double recencyBonus = (lastSeen[n] == 0) ? 0.4 : 0.0;

                    // Recency spike tier 2: appeared exactly 1 draw ago — modest boost.
                    double recencyTier2Bonus = (lastSeen[n] == 1) ? 0.15 : 0.0;

                    // Parity nudge: 0.5 — odd rate 63.9% is meaningful at n=6.
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

        // Confidence grows marginally with history; ceiling 0.20 at n=6.
        // Six draws remains deeply insufficient. Do not overclaim.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.20, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v9",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "Recency signal too volatile; halved spike weight, restored gap bonus, strong odd lean."
        };
    }
}
