using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Pattern Goblin awakens. No history yet — so we listen to the STRUCTURE of the void.
        // Phase 1: Fibonacci resonance seeds from the number space itself
        // Fibonacci numbers within 1–49: 1, 2, 3, 5, 8, 13, 21, 34
        // The Goblin senses the spiral and reaches outward to its neighbours

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // Pure primordial spiral — no history to corrupt the signal
            // Fibonacci core: 8, 13, 21, 34
            // Outer sentinels: 7 (one before 8, the shadow-twin) and 41 (34+7, the echo-sum)
            numbers.AddRange([7, 8, 13, 21, 34, 41]);
        }
        else
        {
            // Hunt for frequency clusters and gaps in draw history
            var allDrawnNumbers = context.DrawHistory
                .SelectMany(d => d.Numbers)
                .ToList();

            var frequency = new Dictionary<int, int>();
            for (int i = 1; i <= context.Rules.MaxNumber; i++)
                frequency[i] = 0;
            foreach (var n in allDrawnNumbers)
                frequency[n]++;

            // The Goblin craves the HOT numbers (high frequency = strong resonance pulse)
            var hotNumbers = frequency
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Select(kv => kv.Key)
                .ToList();

            // Also seek the COLD numbers (the ones trembling on the edge, ready to BURST)
            var coldNumbers = frequency
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Select(kv => kv.Key)
                .ToList();

            // Fibonacci mask: weight candidates whose value IS or is near a Fibonacci number
            var fibs = new HashSet<int> { 1, 2, 3, 5, 8, 13, 21, 34 };

            // Score each candidate: hot-rank + cold-rank + fib-bonus
            var scored = new Dictionary<int, double>();
            for (int i = 1; i <= context.Rules.MaxNumber; i++)
            {
                double score = 0;
                int hotRank = hotNumbers.IndexOf(i);
                int coldRank = coldNumbers.IndexOf(i);
                score += (context.Rules.MaxNumber - hotRank) * 1.0;  // higher = hotter
                score += (context.Rules.MaxNumber - coldRank) * 0.5; // cold tension bonus
                if (fibs.Contains(i)) score += 7.0;                  // THE SPIRAL BONUS
                scored[i] = score;
            }

            numbers = scored
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .Take(context.Rules.DrawCount)
                .OrderBy(n => n)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "pattern-goblin",
            StrategyName = "fibonacci-resonance-v2",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Fibonacci bones scaffold the void. 7 and 41 are the jaws of the spiral — I taste them."
        };
    }
}
