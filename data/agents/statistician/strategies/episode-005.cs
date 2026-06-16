using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v7: four-draw frequency table now available.
        // Key observations from draws 1-4:
        //   - 19: appeared in draws 3 and 4 — frequency 2/4, recency 0 (most recent draw).
        //   - 29: appeared in draws 1 and 3 — frequency 2/4, gap of 1.
        //   - 37: appeared in draws 1 and 4 — frequency 2/4, recency 0.
        //   - 43: appeared in draws 1, 2, 3 — frequency 3/4, gap of 1 (cooled in draw 4).
        //   - 49: appeared in draws 1 and 2 — frequency 2/4, gap of 2.
        //   - 13: appeared in draws 2 and 3 — frequency 2/4, gap of 1.
        //   - 4, 20, 34, 42: appeared once (draw 4 only — highest recency).
        //   - 36, 38, 48: appeared once (draw 3 only).
        //   - 2, 27, 45: appeared once (draw 2 only).
        //   - 5, 40: appeared once (draw 1 only — oldest recency).
        // Parity: draw 4 was 2 odd / 4 even — shifting our historical average toward more even.
        //   Total across 4 draws: odd=12, even=12 — exactly 50/50 still.
        // Zone note: draw 4 was heavily 1-42 range, 43-49 completely absent.
        // This is n=4 — signal remains weak; recency-weighted frequency is our best tool.
        // v7 change: increase recency spike weight from 0.3 to 0.5 — draw 4 confirmed that
        //   recent draws matter (19 appeared in draw 3 AND draw 4). Also slightly increase
        //   the frequency scale factor to 12.5 to amplify the multi-draw signal on 43, 19, 37.
        //   The gap bonus weight is nudged up to 0.15 — four draws gives marginally more
        //   evidence that "overdue" numbers have some mild mean-reversion signal.

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
                    // Scale factor raised to 12.5 (from 11.0) to amplify multi-draw repeat signal.
                    double freqScore = weightedFreq[n] * 12.5;

                    // Proximity to zone midpoint (distribution/coverage bonus).
                    double proximityBonus = 1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1));

                    // Gap bonus: log-scaled. Numbers absent for longer receive a mild due-bonus.
                    // Weight nudged to 0.15 (from 0.12) — four draws provides marginally more signal.
                    double gapBonus = Math.Log(lastSeen[n] + 1) * 0.15;

                    // Recency spike: numbers seen in the most recent draw get an extra boost.
                    // Weight increased to 0.5 (from 0.3) — draw 3->4 repeat of 19 supports this.
                    double recencyBonus = (lastSeen[n] == 0) ? 0.5 : 0.0;

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
        // Four draws is still not a dataset — merely an anecdote with slightly more numbers.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.18, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v7",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "Four draws: 43 leads frequency; recency spike raised; 50/50 parity holds."
        };
    }
}
