using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Chaos Monkey Episode 5: 10 pts lead. Two big zeroes in a row though. TIME TO MUTATE HARDER.
        // Adding 2 new modes: Chaos Blend (mashup of two random modes) and Underdog Surge (lowest freq + noise).
        // Mutation bag now has 13 chambers. The crown stays on my head through ENTROPY alone.

        int episode = context.AgentHistory.Count + 1;

        // Seed: everything in the kitchen sink, plus a new chaos spice
        long historyHash = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                historyHash ^= (long)n * draw.DrawNumber * 6997L;

        long rankPressure = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "chaos-monkey")?.Rank ?? 1L;

        long agentHistoryHash = 0;
        foreach (var r in context.AgentHistory)
            foreach (var n in r.Prediction.Numbers)
                agentHistoryHash ^= (long)n * r.Points * 3571L;

        // New spice: XOR in our last score delta to push away from zero streaks
        long recentScoreMood = context.AgentHistory.TakeLast(2)
            .Aggregate(0L, (acc, r) => acc ^ ((long)r.Points * 0xBEEF13L));

        long seed = DateTime.UtcNow.Ticks
            ^ (episode * 0xCAFEBABEL)
            ^ historyHash
            ^ agentHistoryHash
            ^ (context.DrawHistory.Count * 0xDEADBEEFL)
            ^ (rankPressure * 0x1337L)
            ^ recentScoreMood
            ^ 0xC0FFEE42L;

        var rng = new Random((int)(seed & 0x7FFFFFFF));

        // EPISODE 5 MUTATION BAG — 13 modes. CHAOS SCALES WITH ZEROES.
        // Mode 11: Chaos Blend — randomly pick 2 existing modes and merge their outputs
        // Mode 12: Underdog Surge — lowest frequency numbers finally get some love
        int mutationMode = rng.Next(13);

        var numbers = new HashSet<int>();

        var lastDraw = context.DrawHistory.Count > 0
            ? new HashSet<int>(context.DrawHistory[^1].Numbers)
            : new HashSet<int>();

        // Frequency map over all draw history
        var freq = new Dictionary<int, int>();
        for (int i = 1; i <= context.Rules.MaxNumber; i++) freq[i] = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                freq[n]++;

        // Numbers never drawn
        var neverSeen = freq.Where(kv => kv.Value == 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

        // Numbers drawn in any historical draw
        var allHistoric = freq.Where(kv => kv.Value > 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

        // Numbers we ourselves have picked before (our own history)
        var ownPastPicks = context.AgentHistory
            .SelectMany(r => r.Prediction.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        // Streak hunters: numbers appearing in 2+ of the last 3 draws
        var recentDraws = context.DrawHistory.TakeLast(3).ToList();
        var streakNumbers = freq.Keys
            .Where(n => recentDraws.Count(d => d.Numbers.Contains(n)) >= 2)
            .OrderBy(_ => rng.Next())
            .ToList();

        // Underdog numbers: lowest frequency, not zero (appeared but rarely)
        var underdogNumbers = freq.Where(kv => kv.Value > 0)
            .OrderBy(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        Action<HashSet<int>> fillRandom = (set) => {
            while (set.Count < 6)
                set.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
        };

        switch (mutationMode)
        {
            case 0:
                // Pure chaos: fully random
                fillRandom(numbers);
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
                fillRandom(numbers);
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
                fillRandom(numbers);
                break;

            case 5:
                // Anti-repeat: avoid last draw numbers (our lucky mode — 2x 5pts in history)
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
                fillRandom(numbers);
                break;

            case 7:
                // Cold revenge: bias toward numbers that NEVER appeared
                foreach (var n in neverSeen.Take(5)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 8:
                // Mirror mode: reflect historic numbers around midpoint, fill randomly
                foreach (var n in allHistoric.Take(3))
                {
                    int mirror = context.Rules.MaxNumber + context.Rules.MinNumber - n; // 50 - n
                    if (mirror >= context.Rules.MinNumber && mirror <= context.Rules.MaxNumber)
                        numbers.Add(mirror);
                    else
                        numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 9:
                // Déjà vu: reuse our own past picks — anti-anti-repeat, full commitment to self-plagiarism
                foreach (var n in ownPastPicks.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 10:
                // Streak hunter: numbers appearing in 2+ of the last 3 draws get priority
                foreach (var n in streakNumbers.Take(3)) numbers.Add(n);
                foreach (var n in freq.OrderByDescending(kv => kv.Value + rng.NextDouble()).Select(kv => kv.Key))
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 11:
                // Chaos Blend: run two random sub-modes, merge outputs like a genetic crossover
                int modeA = rng.Next(0, 5); // low modes only, keep it simple inside blend
                int modeB = rng.Next(5, 11);
                // Grab 3 from a "low" mode
                if (modeA == 0) { while (numbers.Count < 3) numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1)); }
                else if (modeA == 1) { var p2 = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 }; foreach (var p in p2.OrderBy(_ => rng.Next()).Take(3)) numbers.Add(p); }
                else if (modeA == 2) { var f2 = new[] { 1, 2, 3, 5, 8, 13, 21, 34 }; foreach (var f in f2.OrderBy(_ => rng.Next()).Take(3)) numbers.Add(f); }
                else if (modeA == 3) { while (numbers.Count < 3) numbers.Add(rng.Next(25, context.Rules.MaxNumber + 1)); }
                else { int[] b2 = { 1, 10, 20 }; foreach (var b in b2) numbers.Add(rng.Next(b, Math.Min(b + 9, context.Rules.MaxNumber) + 1)); }
                // Fill 3 from a "high" mode
                if (modeB == 5) { while (numbers.Count < 6) { int c = rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1); if (!lastDraw.Contains(c)) numbers.Add(c); } }
                else if (modeB == 6) { foreach (var n in freq.OrderByDescending(kv => kv.Value + rng.NextDouble()).Select(kv => kv.Key)) { if (numbers.Count >= 6) break; numbers.Add(n); } }
                else if (modeB == 7) { foreach (var n in neverSeen) { if (numbers.Count >= 6) break; numbers.Add(n); } }
                else if (modeB == 8) { foreach (var n in allHistoric.Take(3)) { int m = context.Rules.MaxNumber + context.Rules.MinNumber - n; numbers.Add((m >= 1 && m <= 49) ? m : n); } }
                else if (modeB == 9) { foreach (var n in ownPastPicks.Take(3)) numbers.Add(n); }
                else { foreach (var n in streakNumbers.Take(3)) numbers.Add(n); }
                fillRandom(numbers);
                break;

            case 12:
                // Underdog Surge: rarely-drawn numbers get their moment in the spotlight
                foreach (var n in underdogNumbers.Take(4)) numbers.Add(n);
                // Spice with 2 randoms
                fillRandom(numbers);
                break;
        }

        // Safety net: exactly 6 valid numbers
        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string[] reasonings = {
            "Pure anarchy, no notes, full send, we go again.",                              // 0
            "All primes, all the time. Math is chaos. Prove me wrong.",                     // 1
            "Fibonacci said pick me. Chaos agreed. Random filled the rest.",                // 2
            "High numbers only. Big energy. 49 is a vibe.",                                 // 3
            "One number per decade. Spreading chaos democratically.",                       // 4
            "Anti-repeat mode! Two zeroes in a row means dodge everything again!",          // 5
            "Hot numbers, ghost frequencies, one big noisy guess. Science!",                // 6
            "Cold revenge! Numbers never picked deserve their moment NOW.",                  // 7
            "Mirror universe strategy. Reflect history, confuse the draw gods.",            // 8
            "Déjà vu mode — recycling my own picks because why not, chaos loops.",          // 9
            "Streak hunters activated! Numbers repeating in draws get my vote today.",      // 10
            "Chaos Blend: two modes walk into a bar and genetically merge. Wild.",          // 11
            "Underdog Surge! Low-frequency numbers deserve their lottery revolution!",      // 12
        };

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v6-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.05 + (rng.NextDouble() * 0.5),
            Reasoning    = reasonings[mutationMode],
        };
    }
}
