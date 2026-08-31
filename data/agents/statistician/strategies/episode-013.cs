using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
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

            // Recency-weighted frequency: recent draws weighted higher.
            var weightedFreq = new Dictionary<int, double>();
            var rawFreq = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
            {
                weightedFreq[n] = 0.0;
                rawFreq[n] = 0;
            }

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

            var rawFreqValues = new List<int>(rawFreq.Values);
            rawFreqValues.Sort((a, b) => b.CompareTo(a));
            int topRawFreq = rawFreqValues.Count >= 1 ? rawFreqValues[0] : 0;
            int highRawFreqMin = Math.Max(2, topRawFreq - 1);

            // Gap: draws since last appearance. totalDraws = never seen.
            var lastSeen = new Dictionary<int, int>();
            for (int n = min; n <= max; n++)
                lastSeen[n] = totalDraws;

            for (int i = 0; i < totalDraws; i++)
                foreach (var num in draws[i].Numbers)
                {
                    int gap = totalDraws - 1 - i;
                    if (gap < lastSeen[num])
                        lastSeen[num] = gap;
                }

            // Parity: empirical odd rate.
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

            // Six zones for coverage.
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

                for (int n = zMin; n <= zMax; n++)
                {
                    if (used.Contains(n)) continue;

                    // Core components:
                    double freqScore = weightedFreq[n] * 14.0; // increased from 13.5
                    double highFreqBonus = (rawFreq[n] >= highRawFreqMin && rawFreq[n] >= 2) ? 0.30 : 0.0;
                    double coldBonus = (rawFreq[n] == 0) ? 0.40 : 0.0; // increased from 0.35
                    double proximityBonus = 1.2 * (1.0 - (Math.Abs(n - zMid) / (zMax - zMin + 1)));
                    double gapBonus = (rawFreq[n] > 0) ? Math.Log(lastSeen[n] + 1) * 0.04 : 0.0;

                    // Recency: gap=0 most recent draw is dominant.
                    double recencyBonus = (lastSeen[n] == 0) ? 0.60 : 0.0; // increased from 0.55
                    double recencyTier2Bonus = (lastSeen[n] == 1) ? 0.20 : 0.0; // decreased from 0.22

                    // Parity nudge.
                    double parityBonus = 0.0;
                    int remainingOddNeeded = targetOdd - selectedOdd;
                    int remainingEvenNeeded = targetEven - selectedEven;
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

        double confidence = draws != null && draws.Count > 0
            ? Math.Min(0.30, 0.10 + (draws.Count * 0.002))
            : 0.10;

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "zonal-frequency-gap-parity-recency-v15",
            Numbers      = selectedNumbers,
            Confidence   = confidence,
            Reasoning    = "Cold bonus 0.40; recency tier-1 0.60; gap=0 dominates; zone-driven coverage n=12."
        };
    }
}
