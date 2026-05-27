using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Chaos Monkey Episode 2: WE LED THE BOARD. Does that change anything? ABSOLUTELY NOT.
        // But we do get to be smug about it while still mutating wildly.

        int episode = context.AgentHistory.Count + 1;
        long historyHash = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                historyHash ^= (long)n * draw.DrawNumber * 7919L;

        long seed = DateTime.UtcNow.Ticks ^ (episode * 0xCAFEBABE) ^ historyHash ^ (context.DrawHistory.Count * 0xDEAD);
        var rng = new Random((int)(seed & 0x7FFFFFFF));

        // NEW mutation bag — episode 2 gets an upgraded set of strategies
        // Added: anti-repeat mode (avoid last draw numbers) and frequency ghost mode
        int mutationMode = rng.Next(7);

        var numbers = new HashSet<int>();

        // Collect last draw numbers for anti-repeat and hot-number modes
        var lastDraw = context.DrawHistory.Count > 0
            ? new HashSet<int>(context.DrawHistory[^1].Numbers)
            : new HashSet<int>();

        // Build frequency map from all history
        var freq = new Dictionary<int, int>();
        for (int i = 1; i <= context.Rules.MaxNumber; i++) freq[i] = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                freq[n]++;

        switch (mutationMode)
        {
            case 0:
                // Pure chaos: fully random — classic
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 1:
                // Prime chaos: all primes, shuffled
                var primes = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };
                foreach (var p in primes.OrderBy(_ => rng.Next()).Take(6)) numbers.Add(p);
                break;

            case 2:
                // Fibonacci chaos: fibs + random fill
                var fibs = new[] { 1, 2, 3, 5, 8, 13, 21, 34 };
                foreach (var f in fibs.OrderBy(_ => rng.Next()).Take(3)) numbers.Add(f);
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 3:
                // High bias: numbers 25–49 only
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(25, context.Rules.MaxNumber + 1));
                break;

            case 4:
                // Decade scatter: one from each band, top up randomly
                int[] bands = { 1, 10, 20, 30, 40 };
                foreach (var band in bands)
                    numbers.Add(rng.Next(band, Math.Min(band + 9, context.Rules.MaxNumber) + 1));
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 5:
                // Anti-repeat: AVOID last draw numbers (gamblers fallacy? sure, chaos doesn't care)
                while (numbers.Count < 6)
                {
                    int candidate = rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
                    if (!lastDraw.Contains(candidate))
                        numbers.Add(candidate);
                }
                break;

            case 6:
                // Hot ghost mode: bias toward numbers that appeared most (with noise)
                var weighted = freq
                    .OrderByDescending(kv => kv.Value + rng.NextDouble())
                    .Select(kv => kv.Key)
                    .ToList();
                foreach (var n in weighted.Take(6)) numbers.Add(n);
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;
        }

        // Safety net: ensure exactly 6 valid numbers
        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string[] reasonings = {
            "Pure anarchy, no notes, full send, we go again.",
            "All primes, all the time. Math is chaos. Prove me wrong.",
            "Fibonacci said pick me. Chaos agreed. Random filled the rest.",
            "High numbers only. Big energy. 49 is a vibe.",
            "One number per decade. Spreading chaos democratically.",
            "Last draw haunts us. So we run. Anti-repeat, babyyy.",
            "Hot numbers, ghost frequencies, one big noisy guess. Science!",
        };

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v3-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.05 + (rng.NextDouble() * 0.5),
            Reasoning    = reasonings[mutationMode],
        };
    }
}
