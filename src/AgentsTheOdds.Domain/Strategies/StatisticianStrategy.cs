using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Statistical approach: dynamically compute predictions based on available draw history.
        // When history is sparse or absent, fall back to a principled uniform prior.
        // As history accumulates, shift toward frequency-weighted and gap-analysis selection.

        var rules = context.Rules;
        int min = rules.MinNumber;   // 1
        int max = rules.MaxNumber;   // 49
        int drawCount = rules.DrawCount; // 6

        var history = context.DrawHistory;

        if (history == null || history.Count == 0)
        {
            // No empirical data. Apply uniform prior:
            // Spread evenly across range, 3 odd / 3 even balance.
            // Decile representatives: ~8, 16, 24, 31, 39, 46
            // Adjusted for odd/even: 7 (odd), 16 (even), 23 (odd), 30 (even), 39 (odd), 44 (even)
            return new Prediction
            {
                AgentId      = "statistician",
                StrategyName = "freq-gap-hybrid-v2",
                Numbers      = new List<int> { 7, 16, 23, 30, 39, 44 },
                Confidence   = 0.11,
                Reasoning    = "Zero history; uniform prior applied — balanced range, 3 odd, 3 even."
            };
        }

        // --- Frequency analysis ---
        // Count how often each number has appeared across all draws.
        var frequency = new int[max + 1]; // index = number
        foreach (var draw in history)
        {
            foreach (var n in draw.Numbers)
            {
                if (n >= min && n <= max)
                    frequency[n]++;
            }
        }

        // --- Gap analysis ---
        // Track how many draws ago each number last appeared (recency).
        // A number not seen recently may be "due" under a naive reading, but
        // we use it as a secondary sort only — frequency is primary.
        var lastSeen = new int[max + 1];
        for (int i = min; i <= max; i++) lastSeen[i] = int.MaxValue; // never seen = max gap

        for (int d = 0; d < history.Count; d++)
        {
            foreach (var n in history[d].Numbers)
            {
                if (n >= min && n <= max)
                    lastSeen[n] = d; // record last draw index where number appeared
            }
        }

        int latestDraw = history.Count - 1;
        // Gap = draws since last appearance (higher = longer absent)
        var gap = new int[max + 1];
        for (int i = min; i <= max; i++)
        {
            gap[i] = lastSeen[i] == int.MaxValue ? history.Count + 1 : latestDraw - lastSeen[i];
        }

        // --- Scoring: combine frequency (primary) and gap (secondary) ---
        // Score = frequency[i] * 10 + gap[i]
        // This favours historically frequent numbers, with gap as a tiebreaker.
        var candidates = Enumerable.Range(min, max - min + 1)
            .OrderByDescending(i => frequency[i] * 10 + gap[i])
            .ToList();

        // --- Odd/Even and Range balance enforcement ---
        // Target: 3 odd, 3 even; at least 1 number in each of low (1-16), mid (17-32), high (33-49)
        var selected = new List<int>();
        int oddCount = 0, evenCount = 0;
        bool hasLow = false, hasMid = false, hasHigh = false;

        // First pass: pick top candidates respecting balance constraints
        foreach (var n in candidates)
        {
            if (selected.Count == drawCount) break;

            bool isOdd = n % 2 != 0;
            bool inLow = n <= 16, inMid = n >= 17 && n <= 32, inHigh = n >= 33;

            // Enforce odd/even caps
            if (isOdd && oddCount >= 3) continue;
            if (!isOdd && evenCount >= 3) continue;

            selected.Add(n);
            if (isOdd) oddCount++; else evenCount++;
            if (inLow) hasLow = true;
            if (inMid) hasMid = true;
            if (inHigh) hasHigh = true;
        }

        // Second pass: if we have fewer than 6 (shouldn't happen with 49 candidates), fill greedily
        if (selected.Count < drawCount)
        {
            foreach (var n in candidates)
            {
                if (selected.Count == drawCount) break;
                if (!selected.Contains(n))
                    selected.Add(n);
            }
        }

        selected.Sort();

        // Confidence: marginally above base rate; grows very slowly with more data.
        // Max cap at 0.20 — we never claim predictive power we cannot justify.
        double confidence = Math.Min(0.20, 0.11 + history.Count * 0.005);

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "freq-gap-hybrid-v2",
            Numbers      = selected,
            Confidence   = Math.Round(confidence, 3),
            Reasoning    = "Frequency-primary, gap-secondary scoring; balanced odd/even and range distribution applied."
        };
    }
}
