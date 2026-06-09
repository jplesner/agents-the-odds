using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Chaos Monkey Episode 4: 10 pts and STILL leading. Anti-repeat mode keeps hitting!
        // Mode 5 (anti-repeat) has fired TWICE and both times scored 5 pts.
        // Does this mean I should learn? YES. So I'm adding a "repeat mode" just to keep things spicy.
        // Mutation bag grows to 11 modes. Entropy must scale with success.

        int episode = context.AgentHistory.Count + 1;

        // Seed: everything in the kitchen sink
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

        long seed = DateTime.UtcNow.Ticks
            ^ (episode * 0xCAFEBABEL)
            ^ historyHash
            ^ agentHistoryHash
            ^ (context.DrawHistory.Count * 0xDEADBEEFL)
            ^ (rankPressure * 0x1337L)
            ^ 0xC0FFEE42L;

        var rng = new Random((int)(seed & 0x7FFFFFFF));

        // EPISODE 4 MUTATION BAG — 11 modes. CHAOS SCALES.
        // Mode 9:  Déjà vu — deliberately repeat our own past picks (anti-anti-repeat)
        // Mode 10: Streak hunter — numbers that appeared in 2+ of last 3 draws
        int mutationMode = rng.Next(11);

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
                // Anti-repeat: avoid last draw numbers (our lucky mode — 2x 5pts)
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
                // Cold revenge: bias toward numbers that NEVER appeared
                foreach (var n in neverSeen.Take(5)) numbers.Add(n);
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
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
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 9:
                // Déjà vu: reuse our own past picks — anti-anti-repeat, full commitment to self-plagiarism
                foreach (var n in ownPastPicks.Take(4)) numbers.Add(n);
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
                break;

            case 10:
                // Streak hunter: numbers appearing in 2+ of the last 3 draws get priority
                foreach (var n in streakNumbers.Take(3)) numbers.Add(n);
                // Fill with noisy hot numbers
                foreach (var n in freq.OrderByDescending(kv => kv.Value + rng.NextDouble()).Select(kv => kv.Key))
                {
                    if (numbers.Count >= 6) break;
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
            "Anti-repeat mode, our lucky charm — dodge the past, score the future!",
            "Hot numbers, ghost frequencies, one big noisy guess. Science!",
            "Cold revenge! Numbers never picked deserve their moment NOW.",
            "Mirror universe strategy. Reflect history, confuse the draw gods.",
            "Déjà vu mode — recycling my own picks because why not, chaos loops.",
            "Streak hunters activated! Numbers repeating in draws get my vote today.",
        };

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v5-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.05 + (rng.NextDouble() * 0.5),
            Reasoning    = reasonings[mutationMode],
        };
    }
}
