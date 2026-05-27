using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy v4: frequency-aware with recency weighting, zonal partitioning,
        // parity balance, and a "gap penalty" — numbers not drawn recently are
        // slightly penalized to avoid overweighting cold numbers on tiny samples.
        // With only 1 draw in history, the uniform prior still dominates; we simply
        // note that {5, 29, 37, 40, 43, 49} were drawn and apply mild recency bias.

        var rules = context.Rules;
        int min = rules.MinNumber;   // 1
        int max = rules.MaxNumber;   // 49
        int drawCount = rules.DrawCount; // 6

        var draws = context.DrawHistory;

        List<int> selectedNumbers;

        if (draws == null || draws.Count == 0)
        {
            // No empirical data: principled uniform prior with parity balance
            selectedNumbers = new List<int> { 5, 14, 19, 28, 37, 44 };
        }
        else
        {
            int totalDraws = draws.Count;

            // Build recency-weighted frequency table
            var weightedFreq = new Dictionary<int, double>();
            for (int n = min; n <= max; n++)
                weightedFreq[n] = 0.0;

            for (int i = 0; i < totalDraws; i++)
            {
                double weight = (double)(i + 1) / totalDraws; // linear recency weight
                foreach (var n in draws[i].Numbers)
                    if (weightedFreq.ContainsKey(n))
                        weightedFreq[n] += weight;
            }

            // Gap analysis: compute how many draws ago each number last appeared.
            // Numbers that have never appeared get gap = totalDraws (maximum).
            // We apply a mild log-based "due bonus" — weak evidence, not mysticism.
            var lastSeen = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
                lastSeen[n] = totalDraws; // default: never seen

            for (int i = 0; i < totalDraws; i++)
                foreach (var n in draws[i].Numbers)
                    lastSeen[n] = totalDraws - 1 - i; // 0 = seen in most recent draw

            // Historical parity rate across all draws
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

            // Zones: 6 bands across 1–49
            var zones = new List<(int zMin, int zMax)>
            {
                (1, 8), (9, 16), (17, 24), (25, 32), (33, 40), (41, 49)
            };

            selectedNumbers = new List<int>();
            var used = new HashSet<int>();
            int selectedOdd = 0, selectedEven = 0;

            foreach (var (zMin, zMax) in zones)
            {
                double zMid = (zMin + zMax) / 2.0;
                int best = -1;
                double bestScore = double.MinValue;

                int oddNeeded = (int)Math.Round(oddRate * drawCount) - selectedOdd;
                int evenNeeded = (drawCount - (int)Math.Round(oddRate * drawCount)) - selectedEven;

                for (int n = zMin; n <= zMax; n++)
                {
                    if (used.Contains(n)) continue;

                    // Frequency component: recency-weighted appearances
                    double freqScore = weightedFreq[n] * 10.0;

                    // Proximity to zone midpoint (spread coverage)
                    double proximityBonus = 1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1));

                    // Gap bonus: very mild due-number signal. log(gap+1) normalised.
                    // On tiny sample sizes this is near-noise; we weight it low.
                    double gapBonus = Math.Log(lastSeen[n] + 1) * 0.2;

                    // Parity nudge
                    double parityBonus = 0.0;
                    if (n % 2 == 1 && oddNeeded > 0) parityBonus = 0.5;
                    else if (n % 2 == 0 && evenNeeded > 0) parityBonus = 0.5;

                    double score = freqScore + proximityBonus + gapBonus + parityBonus;
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

            // Safety: pad to exactly 6 if a zone yielded nothing
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

        // Confidence scales with draw history volume; hard ceiling at 0.18.
        // 13,983,816 combinations demand perpetual humility.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.18, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-v4",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "One draw is noise; zonal spread with gap bonus and parity correction applied cautiously."
        };
    }
}
