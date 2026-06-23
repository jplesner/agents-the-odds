using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v8: five-draw frequency table now available.
        // Key observations from draws 1-5:
        //   Frequency counts (raw):
        //     43: draws 1,2,3,5 — freq 4/5 (highest; cooled draw 4, returned draw 5)
        //     13: draws 2,3 — freq 2/5
        //     19: draws 3,4 — freq 2/5
        //     29: draws 1,3 — freq 2/5
        //     37: draws 1,4 — freq 2/5
        //     49: draws 1,2 — freq 2/5
        //     27: draws 2,5 — freq 2/5
        //     20: draws 4,5 — freq 2/5, gap=0 (most recent)
        //     45: draws 2,5 — freq 2/5, gap=0
        //     23: draw 5 only — gap=0
        //     35: draw 5 only — gap=0
        //   Episode 5 draw: [20, 23, 27, 35, 43, 45]
        //   My Episode 5 pick: [4, 13, 19, 29, 37, 42] — 0 matches. Worst result so far.
        //   Cumulative score: 4 pts (four single matches, one zero).
        //
        // Post-mortem: Episode 5 draw was dominated by numbers with gap=0 (20,23,35,45)
        //   and a returning high-frequency number (43) plus a freq-2 mid-range number (27).
        //   My picks leaned heavily on frequency/gap signals from older draws and missed
        //   entirely. The draw also had NO numbers from zones 1-8 (1-8 range: zero representation
        //   across ALL five draws combined — this is a notable distributional fact).
        //
        // v8 changes:
        //   1. Increase recency spike weight to 0.8 (from 0.5) — draw 5 was dominated by
        //      gap=0 numbers (4 of 6). This is the strongest empirical signal in our dataset.
        //   2. Increase frequency scale factor to 14.0 (from 12.5) — 43's return in draw 5
        //      after skipping draw 4 reinforces the recency-weighted frequency signal.
        //   3. Reduce gap bonus weight to 0.08 (from 0.15) — "due" numbers have not
        //      materialized; gap bonus has not rewarded us across 5 draws. Demoting it.
        //   4. Add a secondary recency tier: gap=1 gets a modest bonus of 0.2 (new).
        //   5. Zone 1 (1-8) has produced ZERO numbers in 5 draws. This is an n=5 signal
        //      that the low end is cold. We will still cover the zone (safety/coverage), but
        //      the zone midpoint proximity bonus will not help numbers there compete.
        //      Accept that zone 1 coverage likely yields a cold pick; minimize wasted weight.
        //   6. Parity across 5 draws: odd=14, even=16 — slight lean toward even.
        //      Target 3 odd / 3 even (round 0.47 odd rate to 3/6).
        //
        // Numbers with strongest composite signals entering Episode 6:
        //   43 (freq 4/5, gap=0 — most recent return), 20 (freq 2/5, gap=0),
        //   45 (freq 2/5, gap=0), 27 (freq 2/5, gap=0), 23 (freq 1/5, gap=0),
        //   35 (freq 1/5, gap=0). But we cannot just pick the last draw — must balance zones.

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

            // Zones: 6 equal-ish bands across 1–49.
            // One number selected per zone to guarantee full range coverage.
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

                    // Frequency component: recency-weighted.
                    // Scale factor raised to 14.0 — 43's return in draw 5 reinforces this signal.
                    double freqScore = weightedFreq[n] * 14.0;

                    // Proximity to zone midpoint (distribution/coverage bonus).
                    double proximityBonus = 1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1));

                    // Gap bonus: log-scaled. Reduced to 0.08 — "due" numbers have not paid off.
                    double gapBonus = Math.Log(lastSeen[n] + 1) * 0.08;

                    // Recency spike tier 1: appeared in the most recent draw — strong boost.
                    // Weight raised to 0.8 — draw 5 had 4 of 6 numbers with gap=0.
                    double recencyBonus = (lastSeen[n] == 0) ? 0.8 : 0.0;

                    // Recency spike tier 2: appeared exactly 1 draw ago — modest boost.
                    double recencyTier2Bonus = (lastSeen[n] == 1) ? 0.2 : 0.0;

                    // Parity nudge: steer toward historically observed odd/even balance.
                    double parityBonus = 0.0;
                    if (n % 2 == 1 && oddNeeded > 0) parityBonus = 0.4;
                    else if (n % 2 == 0 && evenNeeded > 0) parityBonus = 0.4;

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

        // Confidence grows marginally with history; hard ceiling at 0.18.
        // Five draws remains anecdote-tier. Do not overclaim.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.18, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v8",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "Draw 5: four gap=0 numbers. Recency spike raised; gap bonus demoted."
        };
    }
}
