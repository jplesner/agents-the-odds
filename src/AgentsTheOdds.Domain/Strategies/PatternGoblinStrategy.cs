using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Goblin awakens. No history yet — but the VOID has structure.
        // We spiral outward: Fibonacci seeds (1,1,2,3,5,8,13,21,34...) mapped onto [1,49]
        // Then we listen for echoes from past draws if they exist.

        var history = context.DrawHistory;
        var rules = context.Rules;

        var numbers = new List<int>();

        if (history.Count == 0)
        {
            // Primordial mode: Pure Fibonacci resonance
            // Fib sequence: 1,1,2,3,5,8,13,21,34,55... mapped into [1,49]
            // Take: 3, 5, 13, 21, 34 + one sentinel (42 — the answer to everything)
            numbers = [3, 5, 13, 21, 34, 42];
        }
        else
        {
            // Cluster-hunt mode: find the numbers that VIBRATE most in recent draws
            var recentWindow = Math.Min(history.Count, 5);
            var recent = history.Skip(history.Count - recentWindow).ToList();

            // Count frequency of each number in recent draws
            var freq = new int[rules.MaxNumber + 1];
            foreach (var draw in recent)
                foreach (var n in draw.Numbers)
                    freq[n]++;

            // Score each number: frequency + Fibonacci resonance bonus
            int[] fibs = [1, 2, 3, 5, 8, 13, 21, 34];
            var fibSet = new HashSet<int>(fibs);

            var scored = Enumerable.Range(rules.MinNumber, rules.MaxNumber - rules.MinNumber + 1)
                .Select(n => (
                    Number: n,
                    Score: freq[n] * 10.0
                          + (fibSet.Contains(n) ? 3.0 : 0.0)
                          + (n % 7 == 0 ? 1.5 : 0.0)   // sevens hum with spiral energy
                          + (IsPrime(n) ? 1.0 : 0.0)    // primes resonate at higher frequencies
                ))
                .OrderByDescending(x => x.Score)
                .ThenBy(x => Guid.NewGuid()) // tie-break with chaos — the Goblin demands it
                .Select(x => x.Number)
                .Distinct()
                .Take(6)
                .OrderBy(n => n)
                .ToList();

            numbers = scored;
        }

        return new Prediction
        {
            AgentId      = "pattern-goblin",
            StrategyName = "fibonacci-cluster-resonance-v2",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "The void whispers: 3, 5, 13, 21, 34 — Fibonacci spine! 42 seals the cosmos."
        };
    }

    private static bool IsPrime(int n)
    {
        if (n < 2) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;
        for (int i = 3; i * i <= n; i += 2)
            if (n % i == 0) return false;
        return true;
    }
}
