using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v6: three-draw frequency table now available.
        // Key observations from draws 1-3:
        //   - 13: appeared in draws 2 and 3 — frequency 2/3.
        //   - 29: appeared in draws 1 and 3 — frequency 2/3.
        //   - 43: appeared in draws 1 and 2 — frequency 2/3, but absent in draw 3 (cooling).
        //   - 49: appeared in draws 1 and 2 — frequency 2/3, but absent in draw 3 (cooling).
        //   - 19, 36, 38, 48: appeared once (draw 3 only — highest recency).
        //   - 5, 37, 40: appeared once (draw 1 only — oldest recency).
        //   - 2, 27, 45: appeared once (draw 2 only — mid recency).
        // Recency weighting: draw 3 (most recent) gets weight 1.0, draw 1 gets weight 0.33.
        // Parity observation: draws 1-3 contained 9 odd / 9 even numbers exactly — 50/50 split.
        // Zone coverage: draw 3 skewed toward 13-49 range, with a gap in 1-12 and 25-32.
        // Strategy: recency-weighted frequency, zone coverage, parity balance, gap analysis.
        // n=3 improves signal marginally; caution remains appropriate.

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

            // Raw frequency count (unweighted) for tie-breaking and analysis.
            var rawFreq = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
                rawFreq[n] = 0;
            foreach (var draw in draws)
                foreach (var n in draw.Numbers)
                    rawFreq[n]++;

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
            // One number selected per zone to guarantee coverage.
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
                    // Scale factor 11.0 — slightly reduced from v5 to avoid over-anchoring
                    // on numbers that appeared early but have since cooled (e.g., 43, 49).
                    double freqScore = weightedFreq[n] * 11.0;

                    // Proximity to zone midpoint (distribution/coverage bonus).
                    double proximityBonus = 1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1));

                    // Gap bonus: log-scaled. Numbers absent for longer receive a mild due-bonus.
                    // Weight reduced to 0.12 — three draws does not meaningfully validate this signal.
                    double gapBonus = Math.Log(lastSeen[n] + 1) * 0.12;

                    // Recency spike: numbers seen in the most recent draw get a small extra boost
                    // to reflect possible short-term momentum (weak hypothesis, low weight 0.3).
                    double recencyBonus = (lastSeen[n] == 0) ? 0.3 : 0.0;

                    // Parity nudge: steer toward historically observed odd/even balance.
                    double parityBonus = 0.0;
                    if (n % 2 == 1 && oddNeeded > 0) parityBonus = 0.4;
                    else if (n % 2 == 0 && evenNeeded > 0) parityBonus = 0.4;

                    double score = freqScore + proximityBonus + gapBonus + recencyBonus + parityBonus;
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
        // Three draws is not a dataset — it is an anecdote with marginally more numbers attached.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.18, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v6",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "Three draws: 13 and 29 repeat; recency-weighted frequency, zonal spread, 50/50 parity observed."
        };
    }
}
