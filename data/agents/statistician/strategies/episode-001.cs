using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Strategy: frequency-aware with uniform prior fallback
        // When draw history is available, compute frequency of each number across all draws
        // and select a balanced mix of high-frequency and mid-range numbers.
        // When no history exists, apply a principled uniform prior.

        var rules = context.Rules;
        int min = rules.MinNumber;   // 1
        int max = rules.MaxNumber;   // 49
        int drawCount = rules.DrawCount; // 6

        var draws = context.DrawHistory;

        List<int> selectedNumbers;

        if (draws == null || draws.Count == 0)
        {
            // No empirical data. Apply uniform prior:
            // - Spread across low / mid-low / mid / mid-high / high zones
            // - 3 odd, 3 even for expected parity balance
            // Zones (1-49 split into 6 deciles of ~8):
            // Zone 1: 1-8, Zone 2: 9-16, Zone 3: 17-24, Zone 4: 25-32, Zone 5: 33-40, Zone 6: 41-49
            // Select one representative from each zone, alternating parity
            selectedNumbers = new List<int> { 5, 14, 19, 28, 37, 44 };
        }
        else
        {
            // Build frequency table
            var freq = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
                freq[n] = 0;

            foreach (var draw in draws)
                foreach (var n in draw.Numbers)
                    if (freq.ContainsKey(n))
                        freq[n]++;

            // Compute recency-weighted frequency:
            // More recent draws contribute more weight (linear decay from oldest to newest)
            var weightedFreq = new Dictionary<int, double>();
            for (int n = min; n <= max; n++)
                weightedFreq[n] = 0.0;

            int totalDraws = draws.Count;
            for (int i = 0; i < totalDraws; i++)
            {
                double weight = (double)(i + 1) / totalDraws; // 0..1, increasing with recency
                foreach (var n in draws[i].Numbers)
                    if (weightedFreq.ContainsKey(n))
                        weightedFreq[n] += weight;
            }

            // Partition 1-49 into 6 zones and pick best candidate from each
            // Zones: 1-8, 9-16, 17-24, 25-32, 33-40, 41-49
            var zones = new List<(int zMin, int zMax)>
            {
                (1, 8), (9, 16), (17, 24), (25, 32), (33, 40), (41, 49)
            };

            selectedNumbers = new List<int>();
            var used = new HashSet<int>();

            foreach (var (zMin, zMax) in zones)
            {
                // Among numbers in this zone, pick the one with highest weighted frequency
                // Ties broken by proximity to zone midpoint (prefer spread)
                double zMid = (zMin + zMax) / 2.0;
                int best = -1;
                double bestScore = double.MinValue;

                for (int n = zMin; n <= zMax; n++)
                {
                    if (used.Contains(n)) continue;
                    // Score: weighted frequency + small proximity bonus
                    double proximityBonus = 1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1));
                    double score = weightedFreq[n] * 10.0 + proximityBonus;
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
                }
            }

            // Safety: if somehow we have fewer than 6, fill with unselected numbers in order
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
        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.18, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-weighted-v2",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "Zonal frequency weighting applied; recency-adjusted, no overclaiming warranted."
        };
    }
}
