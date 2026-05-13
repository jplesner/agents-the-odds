using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Goblin awakens. No history? No matter — the universe PRE-PATTERNS itself.
        // Strategy: Weave Fibonacci-adjacent numbers with prime resonance nodes.
        // When history exists, hunt for gaps and clusters. For now: the spiral speaks first.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // Primordial mode: Fibonacci skeleton + prime sentinels
            // Fib-adjacent: 7 (prime), 13 (prime), 21 (fib), 34 (fib), plus outer resonators
            numbers.AddRange([7, 13, 21, 34, 41, 48]);
        }
        else
        {
            // Frequency map — the Goblin COUNTS the whispers
            var freq = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) freq[n] = 0;

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    freq[n]++;

            // Find the "cold" numbers — they are BUILDING PRESSURE, coiling to strike
            var cold = freq.OrderBy(kv => kv.Value).ThenBy(kv => kv.Key)
                          .Select(kv => kv.Key).Take(20).ToList();

            // Find the "hot" numbers — the universe is STUTTERING, repeating itself
            var hot = freq.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key)
                         .Select(kv => kv.Key).Take(20).ToList();

            // The Goblin blends: 3 cold (pressure), 2 hot (echo), 1 prime anchor
            var primes = new List<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };

            numbers.Add(cold[0]);
            numbers.Add(cold[1]);
            numbers.Add(cold[2]);
            numbers.Add(hot[0]);
            numbers.Add(hot[1]);

            // Prime anchor: pick the prime NOT yet in numbers, closest to the midpoint (25)
            var anchor = primes
                .Where(p => !numbers.Contains(p))
                .OrderBy(p => Math.Abs(p - 25))
                .First();
            numbers.Add(anchor);

            // Deduplicate just in case the spiral tangled
            numbers = numbers.Distinct().Take(6).ToList();

            // If somehow we need more (shouldn't happen), fill from cold
            foreach (var c in cold)
            {
                if (numbers.Count >= 6) break;
                if (!numbers.Contains(c)) numbers.Add(c);
            }

            numbers = numbers.Take(6).OrderBy(x => x).ToList();
        }

        return new()
        {
            AgentId      = "pattern-goblin",
            StrategyName = "cold-hot-prime-weave-v2",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Cold numbers coil with pressure. Hot echoes stutter. One prime anchors the spiral."
        };
    }
}
