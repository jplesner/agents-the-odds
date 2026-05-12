using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1: No draw history available.
        // Strategy: Without empirical frequency data, we apply a statistically principled prior.
        // Methodology:
        //   1. If draw history exists, compute frequency of each number and select
        //      a blend of high-frequency and underrepresented numbers.
        //   2. Without history, fall back to a uniform prior:
        //      - Partition 1–49 into 6 equal decile bands (~8 numbers each)
        //      - Select one representative from each band
        //      - Maintain 3 odd / 3 even balance (empirical lottery average)
        //      - Maximise gap between selected numbers (min pairwise distance)

        var rules = context.Rules;
        int min = rules.MinNumber;   // 1
        int max = rules.MaxNumber;   // 49
        int count = rules.DrawCount; // 6

        var history = context.DrawHistory;
        var selected = new List<int>();

        if (history != null && history.Count >= 3)
        {
            // Build frequency table
            var freq = new Dictionary<int, int>();
            for (int n = min; n <= max; n++) freq[n] = 0;
            foreach (var draw in history)
                foreach (var n in draw.Numbers)
                    if (freq.ContainsKey(n)) freq[n]++;

            // Score each number: blend frequency (50%) and recency gap (50%)
            // Recency gap = episodes since last appearance (higher = more overdue)
            var lastSeen = new Dictionary<int, int>();
            for (int n = min; n <= max; n++) lastSeen[n] = history.Count + 1; // default: never seen
            for (int i = 0; i < history.Count; i++)
                foreach (var n in history[i].Numbers)
                    lastSeen[n] = history.Count - i; // smaller = more recent

            double maxFreq = freq.Values.Max() == 0 ? 1 : freq.Values.Max();
            double maxGap  = lastSeen.Values.Max() == 0 ? 1 : lastSeen.Values.Max();

            var scores = new Dictionary<int, double>();
            for (int n = min; n <= max; n++)
            {
                double freqScore = freq[n] / maxFreq;
                double gapScore  = lastSeen[n] / maxGap;
                scores[n] = 0.5 * freqScore + 0.5 * gapScore;
            }

            // Select top-scoring numbers, enforcing 3 odd / 3 even and range spread
            var candidates = scores.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
            int oddCount = 0, evenCount = 0;
            foreach (var n in candidates)
            {
                if (selected.Count == count) break;
                bool isOdd = n % 2 != 0;
                if (isOdd && oddCount < 3)       { selected.Add(n); oddCount++; }
                else if (!isOdd && evenCount < 3) { selected.Add(n); evenCount++; }
            }
            // Fill any remaining slots if balance couldn't be met
            foreach (var n in candidates)
            {
                if (selected.Count == count) break;
                if (!selected.Contains(n)) selected.Add(n);
            }
        }
        else
        {
            // Uniform prior: one number from each of 6 equal bands across 1–49
            // Band boundaries: [1–8], [9–16], [17–24], [25–32], [33–40], [41–49]
            // Pick the median of each band, adjusted for odd/even balance
            // Band medians (approx): 4 (even→5), 12, 20, 28, 36, 45
            // Targeting: odd=5,23,45 even=12,28,36 → 3 odd / 3 even
            selected.AddRange(new[] { 5, 12, 23, 28, 36, 45 });
        }

        selected.Sort();

        // Confidence: base rate 6/49 ≈ 0.122; history provides marginal lift
        double confidence = history != null && history.Count > 0
            ? Math.Min(0.20, 0.12 + history.Count * 0.005)
            : 0.12;

        string strategyName = history != null && history.Count >= 3
            ? "freq-gap-blend-v2"
            : "uniform-prior-v2";

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = strategyName,
            Numbers      = selected,
            Confidence   = Math.Round(confidence, 3),
            Reasoning    = "No empirical data yet; uniform prior, balanced odd/even, equal range spread applied."
        };
    }
}
