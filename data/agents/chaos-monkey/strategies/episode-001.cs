using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Chaos Monkey Episode 1: No history? No problem. We go full feral.
        // Seed with something spicy: episode number + draw history count + time ticks
        int episode = context.AgentHistory.Count + 1;
        long seed = DateTime.UtcNow.Ticks ^ (episode * 0xDEADBEEF) ^ (context.DrawHistory.Count * 31337);
        var rng = new Random((int)(seed & 0x7FFFFFFF));

        // Strategy mutation bag — pick one randomly
        int mutationMode = rng.Next(5);

        var numbers = new HashSet<int>();

        switch (mutationMode)
        {
            case 0:
                // Pure chaos: fully random
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 1:
                // Prime chaos: pick from primes only
                var primes = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };
                var shuffledPrimes = primes.OrderBy(_ => rng.Next()).ToList();
                foreach (var p in shuffledPrimes.Take(6)) numbers.Add(p);
                break;

            case 2:
                // Fibonacci chaos: fib numbers in range + random fill
                var fibs = new[] { 1, 2, 3, 5, 8, 13, 21, 34 };
                var shuffledFibs = fibs.OrderBy(_ => rng.Next()).ToList();
                foreach (var f in shuffledFibs.Take(3)) numbers.Add(f);
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 3:
                // High bias: prefer numbers above 25
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(25, context.Rules.MaxNumber + 1));
                break;

            case 4:
                // Scattered: one from each decade band
                int[] bands = { 1, 10, 20, 30, 40 };
                foreach (var band in bands)
                    numbers.Add(rng.Next(band, Math.Min(band + 9, context.Rules.MaxNumber) + 1));
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;
        }

        // Final safety: ensure exactly 6 unique numbers in range
        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string[] reasonings = {
            "Mutation mode activated. Throwing darts at the number wall. YOLO.",
            "No data? No problem. Chaos is its own strategy, trust me.",
            "Strategy bag randomized. Could be primes, could be vibes. Both valid.",
            "High numbers only? Low numbers? Yes. Chaotically, yes.",
            "Decade bands selected. Spread the chaos evenly. Science.",
        };

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v2-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.1 + (rng.NextDouble() * 0.4), // confidence is also chaotic
            Reasoning    = reasonings[mutationMode],
        };
    }
}
