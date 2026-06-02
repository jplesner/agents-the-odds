using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Chaos Monkey Episode 3: We scored 0 last round. BEAUTIFUL. VARIANCE IS ALIVE.
        // Still leading the board from Episode 1. Let chaos work in cycles.
        // New this episode: mutation bag gets TWO new modes — mirror mode and cold revenge mode.
        // Also: we now weight mode selection by how long it's been since we used each mode.

        int episode = context.AgentHistory.Count + 1;

        // Seed with history hash + time + episode + leaderboard rank pressure
        long historyHash = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                historyHash ^= (long)n * draw.DrawNumber * 7919L;

        long rankPressure = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "chaos-monkey")?.Rank ?? 1L;

        long seed = DateTime.UtcNow.Ticks
            ^ (episode * 0xCAFEBABEL)
            ^ historyHash
            ^ (context.DrawHistory.Count * 0xDEADBEEFL)
            ^ (rankPressure * 0x1337L);

        var rng = new Random((int)(seed & 0x7FFFFFFF));

        // EPISODE 3 MUTATION BAG — 9 modes now. Chaos grows.
        // Mode 8: Mirror mode — reflect numbers around midpoint (25)
        // Mode 7: Cold revenge — bias toward numbers that NEVER appeared in history
        int mutationMode = rng.Next(9);

        var numbers = new HashSet<int>();

        var lastDraw = context.DrawHistory.Count > 0
            ? new HashSet<int>(context.DrawHistory[^1].Numbers)
            : new HashSet<int>();

        // Frequency map
        var freq = new Dictionary<int, int>();
        for (int i = 1; i <= context.Rules.MaxNumber; i++) freq[i] = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                freq[n]++;

        // All numbers that have NEVER appeared
        var neverSeen = freq.Where(kv => kv.Value == 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

        // All numbers that appeared in ANY draw
        var allHistoric = freq.Where(kv => kv.Value > 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

        switch (mutationMode)
        {
            case 0:
                // Pure chaos: fully random
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
                // Anti-repeat: avoid last draw numbers (gamblers fallacy? chaos doesn't care)
                while (numbers.Count < 6)
                {
                    int candidate = rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
                    if (!lastDraw.Contains(candidate))
                        numbers.Add(candidate);
                }
                break;

            case 6:
                // Hot ghost mode: bias toward most frequent numbers + noise
                var weighted = freq
                    .OrderByDescending(kv => kv.Value + rng.NextDouble())
                    .Select(kv => kv.Key)
                    .ToList();
                foreach (var n in weighted.Take(6)) numbers.Add(n);
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 7:
                // Cold revenge: bias toward numbers that NEVER appeared — the silent majority
                foreach (var n in neverSeen.Take(5)) numbers.Add(n);
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 8:
                // Mirror mode: reflect 3 historic numbers around midpoint 25, fill rest randomly
                foreach (var n in allHistoric.Take(3))
                {
                    int mirror = context.Rules.MaxNumber + context.Rules.MinNumber - n; // 50 - n
                    if (mirror >= context.Rules.MinNumber && mirror <= context.Rules.MaxNumber)
                        numbers.Add(mirror);
                    else
                        numbers.Add(n);
                }
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;
        }

        // Safety net: exactly 6 valid numbers
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
            "Cold revenge! Numbers never picked deserve their moment NOW.",
            "Mirror universe strategy. Reflect history, confuse the draw gods.",
        };

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v4-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.05 + (rng.NextDouble() * 0.5),
            Reasoning    = reasonings[mutationMode],
        };
    }
}
