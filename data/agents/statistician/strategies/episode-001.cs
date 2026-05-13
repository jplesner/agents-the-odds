using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy: frequency-aware with uniform prior fallback
        // When draw history is available, compute recency-weighted frequency of each number
        // across all draws and select a balanced mix using zonal partitioning.
        // When no history exists, apply a principled uniform prior with parity balance.

        var rules = context.Rules;
        int min = rules.MinNumber;   // 1
        int max = rules.MaxNumber;   // 49
        int drawCount = rules.DrawCount; // 6

        var draws = context.DrawHistory;

        List<int> selectedNumbers;

        if (draws == null || draws.Count == 0)
        {
            // No empirical data. Apply uniform prior:
            // - Spread evenly across 6 zones spanning 1–49
            // - 3 odd, 3 even for expected parity balance
            // Zone representatives chosen near zone midpoints, alternating parity
            selectedNumbers = new List<int> { 5, 14, 19, 28, 37, 44 };
        }
        else
        {
            // Build recency-weighted frequency table
            // More recent draws contribute linearly more weight
            var weightedFreq = new Dictionary<int, double>();
            for (int n = min; n <= max; n++)
                weightedFreq[n] = 0.0;

            int totalDraws = draws.Count;
            for (int i = 0; i < totalDraws; i++)
            {
                double weight = (double)(i + 1) / totalDraws; // increases with recency
                foreach (var n in draws[i].Numbers)
                    if (weightedFreq.ContainsKey(n))
                        weightedFreq[n] += weight;
            }

            // Also factor in parity balance: track odd/even counts in history
            int oddCount = 0, evenCount = 0;
            foreach (var draw in draws)
                foreach (var n in draw.Numbers)
                {
                    if (n % 2 == 0) evenCount++;
                    else oddCount++;
                }
            double oddRate = totalDraws > 0 ? (double)oddCount / (oddCount + evenCount) : 0.5;

            // Partition 1-49 into 6 zones and pick best candidate from each
            // Zones: 1-8, 9-16, 17-24, 25-32, 33-40, 41-49
            var zones = new List<(int zMin, int zMax)>
            {
                (1, 8), (9, 16), (17, 24), (25, 32), (33, 40), (41, 49)
            };

            selectedNumbers = new List<int>();
            var used = new HashSet<int>();

            // Track parity of selected so far
            int selectedOdd = 0, selectedEven = 0;

            foreach (var (zMin, zMax) in zones)
            {
                double zMid = (zMin + zMax) / 2.0;
                int best = -1;
                double bestScore = double.MinValue;

                // Determine parity preference for this slot based on historical rate
                // Target ~3 odd, ~3 even. Nudge score toward underrepresented parity.
                int slotsRemaining = drawCount - selectedNumbers.Count;
                int oddNeeded = (int)Math.Round(oddRate * drawCount) - selectedOdd;
                int evenNeeded = (drawCount - (int)Math.Round(oddRate * drawCount)) - selectedEven;

                for (int n = zMin; n <= zMax; n++)
                {
                    if (used.Contains(n)) continue;

                    double proximityBonus = 1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1));
                    double parityBonus = 0.0;
                    if (n % 2 == 1 && oddNeeded > 0) parityBonus = 0.5;
                    else if (n % 2 == 0 && evenNeeded > 0) parityBonus = 0.5;

                    double score = weightedFreq[n] * 10.0 + proximityBonus + parityBonus;
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

            // Safety: fill to exactly 6 if needed
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

        // Confidence: scales modestly with draw history volume; hard ceiling at 0.18
        // Even with a large sample, the combinatorial space (13,983,816) demands humility.
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.18, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-parity-weighted-v3",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "Zonal selection with recency weighting and parity correction; empirical priors only."
        };
    }
}
