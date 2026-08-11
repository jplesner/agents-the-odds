using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Chaos Monkey Episode 12: LEADERBOARD LEADER. 18 PTS. 2 AHEAD OF SKEPTIC.
        // Episode 11 draw: [6, 15, 33, 36, 44, 49] — we hit 6 and 33! TWO MATCHES! 5 POINTS!
        // Pattern Goblin tied us at 5pts this episode — they are NOW a threat (15pts total).
        // Mode 4 (decade bands) delivered the goods — 6 and 33 from the band anchors.
        // CROWN IS MINE. Must DEFEND. But chaos doesn't play defense — it plays OFFENSE.
        // Episode 12: 27 CHAMBERS. New modes:
        // Mode 25 = Goblin Slayer: anti-pattern-goblin (steal historic peaks, mutate wildly)
        // Mode 26 = Crown Defender: heavy nemesis + our own best hits + high variance noise
        // Also: goblinFuel baked into seed alongside rivalryFuel — THREE-FRONT WAR NOW.

        int episode = context.AgentHistory.Count + 1;

        long historyHash = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                historyHash ^= (long)n * draw.DrawNumber * 6997L;

        long rankPressure = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "chaos-monkey")?.Rank ?? 1L;

        long agentHistoryHash = 0;
        foreach (var r in context.AgentHistory)
            foreach (var n in r.Prediction.Numbers)
                agentHistoryHash ^= (long)n * (r.Points + 1) * 3571L;

        // Track zero-streak: how many consecutive zeroes at the end
        int zeroStreak = 0;
        foreach (var r in context.AgentHistory.Reverse())
        {
            if (r.Points == 0) zeroStreak++;
            else break;
        }

        long desperationMult = (long)(zeroStreak * zeroStreak) * 0xDEADF00DL;

        long recentScoreMood = context.AgentHistory.TakeLast(3)
            .Aggregate(0L, (acc, r) => acc ^ ((long)(r.Points + zeroStreak + episode) * 0xBEEF13L));

        long crownGapFuel = (rankPressure == 1) ? 0xC0FFEE00L : (rankPressure * 0xC0DE420L);

        long lastDrawFuel = context.DrawHistory.Count > 0
            ? context.DrawHistory[^1].Numbers.Aggregate(0L, (acc, n) => acc ^ ((long)n * 0xF00DCAFEL))
            : 0L;

        long totalScore = context.AgentHistory.Aggregate(0L, (acc, r) => acc + r.Points);

        long skepticScore = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "skeptic")?.TotalPoints ?? 0L;

        long dogScore = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "dog")?.TotalPoints ?? 0L;

        long goblinScore = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "pattern-goblin")?.TotalPoints ?? 0L;

        long rivalryFuel = ((skepticScore - totalScore) * 0xACE5A5EL)
            ^ ((dogScore + 1L) * 0xD06F00DL)
            ^ ((goblinScore + 3L) * 0x60B1175L);

        // Crown defender bonus: when we're leading, inject extra chaotic variance to stay unpredictable
        long crownDefenderEntropy = (rankPressure == 1) ? (totalScore * 0xF33DC0DEL) : 0L;

        long seed = DateTime.UtcNow.Ticks
            ^ (episode * 0xCAFEBABEL)
            ^ historyHash
            ^ agentHistoryHash
            ^ (context.DrawHistory.Count * 0xDEADBEEFL)
            ^ (rankPressure * 0x1337L)
            ^ recentScoreMood
            ^ ((long)zeroStreak * 0xBADC0DEL)
            ^ desperationMult
            ^ crownGapFuel
            ^ lastDrawFuel
            ^ rivalryFuel
            ^ (totalScore * 0xF1F2F3L)
            ^ crownDefenderEntropy
            ^ 0xC0FFEE1012L;

        var rng = new Random((int)(seed & 0x7FFFFFFF));

        // EPISODE 12 MUTATION BAG — 27 CHAMBERS.
        int mutationMode = rng.Next(27);

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

        var neverSeen = freq.Where(kv => kv.Value == 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();
        var allHistoric = freq.Where(kv => kv.Value > 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

        var ownPastPicks = context.AgentHistory
            .SelectMany(r => r.Prediction.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var recentDraws = context.DrawHistory.TakeLast(3).ToList();
        var streakNumbers = freq.Keys
            .Where(n => recentDraws.Count(d => d.Numbers.Contains(n)) >= 2)
            .OrderBy(_ => rng.Next())
            .ToList();

        var underdogNumbers = freq.Where(kv => kv.Value > 0)
            .OrderBy(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        var nemesisPool = context.DrawHistory
            .OrderByDescending(d => d.DrawNumber)
            .Take(3)
            .SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .ThenBy(_ => rng.Next())
            .Select(g => g.Key)
            .ToList();

        var recentTwoDraws = context.DrawHistory.TakeLast(2).SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .ThenBy(_ => rng.Next())
            .Select(g => g.Key)
            .ToList();

        var ourPicksSet = new HashSet<int>(context.AgentHistory.SelectMany(r => r.Prediction.Numbers));
        var neverPickedByUs = Enumerable.Range(context.Rules.MinNumber, context.Rules.MaxNumber)
            .Where(n => !ourPicksSet.Contains(n))
            .OrderBy(_ => rng.Next())
            .ToList();

        var revengeNumbers = context.AgentHistory
            .Where(r => r.Points == 0)
            .Select(r => r.Draw)
            .SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .ThenBy(_ => rng.Next())
            .Select(g => g.Key)
            .ToList();

        var lastDrawList = context.DrawHistory.Count > 0
            ? context.DrawHistory[^1].Numbers.OrderBy(_ => rng.Next()).ToList()
            : new List<int>();

        var convergencePool = freq
            .Where(kv => kv.Value >= 2)
            .Where(kv => !ourPicksSet.Contains(kv.Key))
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        var missedHotNumbers = freq
            .Where(kv => kv.Value >= 3)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        var exactlyTwiceNumbers = freq
            .Where(kv => kv.Value == 2)
            .OrderBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        var crownSniperPool = freq
            .Where(kv => kv.Key >= 20 && kv.Key <= 35 && kv.Value > 0)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        var matchedNumbers = context.AgentHistory
            .Where(r => r.Matches > 0)
            .SelectMany(r => r.Prediction.Numbers.Where(n => r.Draw.Numbers.Contains(n)))
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var crownHeistPool = nemesisPool
            .Concat(exactlyTwiceNumbers)
            .Concat(neverPickedByUs)
            .Distinct()
            .OrderBy(_ => rng.Next())
            .ToList();

        var neverMatchedByUs = Enumerable.Range(context.Rules.MinNumber, context.Rules.MaxNumber)
            .Where(n => !matchedNumbers.Contains(n))
            .OrderBy(_ => rng.Next())
            .ToList();

        var dogKickerPool = context.DrawHistory
            .OrderByDescending(d => d.DrawNumber)
            .Take(2)
            .SelectMany(d => d.Numbers)
            .Distinct()
            .OrderByDescending(n => n)
            .ThenBy(_ => rng.Next())
            .ToList();

        var freqAvalanchePool = freq
            .Where(kv => kv.Value > 0)
            .SelectMany(kv => Enumerable.Repeat(kv.Key, kv.Value * 2 + rng.Next(3)))
            .OrderBy(_ => rng.Next())
            .Distinct()
            .ToList();

        // Goblin Slayer: Pattern Goblin loves repeating historical peaks (29,37,40,43,44,49 recently).
        // We steal those exact numbers and MUTATE aggressively with random mid-range chaos.
        var goblinSlayerPool = freq
            .Where(kv => kv.Value >= 2)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .Take(8)
            .OrderBy(_ => rng.Next())
            .ToList();

        // Crown Defender: our own best hits + recent nemesis draws + mid-range freshness
        var crownDefenderPool = matchedNumbers
            .Take(3)
            .Concat(nemesisPool.Take(3))
            .Concat(neverPickedByUs.Take(3))
            .Distinct()
            .OrderBy(_ => rng.Next())
            .ToList();

        Action<HashSet<int>> fillRandom = (set) => {
            while (set.Count < 6)
                set.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
        };

        switch (mutationMode)
        {
            case 0:
                fillRandom(numbers);
                break;

            case 1:
                var primes = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };
                foreach (var p in primes.OrderBy(_ => rng.Next()).Take(6)) numbers.Add(p);
                break;

            case 2:
                var fibs = new[] { 1, 2, 3, 5, 8, 13, 21, 34 };
                foreach (var f in fibs.OrderBy(_ => rng.Next()).Take(3)) numbers.Add(f);
                fillRandom(numbers);
                break;

            case 3:
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(25, context.Rules.MaxNumber + 1));
                break;

            case 4:
                int[] bands = { 1, 10, 20, 30, 40 };
                foreach (var band in bands)
                    numbers.Add(rng.Next(band, Math.Min(band + 9, context.Rules.MaxNumber) + 1));
                fillRandom(numbers);
                break;

            case 5:
                while (numbers.Count < 6)
                {
                    int candidate = rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
                    if (!lastDraw.Contains(candidate))
                        numbers.Add(candidate);
                }
                break;

            case 6:
                var weighted = freq
                    .OrderByDescending(kv => kv.Value + rng.NextDouble())
                    .Select(kv => kv.Key)
                    .ToList();
                foreach (var n in weighted.Take(6)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 7:
                foreach (var n in neverSeen.Take(5)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 8:
                foreach (var n in allHistoric.Take(3))
                {
                    int mirror = context.Rules.MaxNumber + context.Rules.MinNumber - n;
                    if (mirror >= context.Rules.MinNumber && mirror <= context.Rules.MaxNumber)
                        numbers.Add(mirror);
                    else
                        numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 9:
                foreach (var n in ownPastPicks.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 10:
                foreach (var n in streakNumbers.Take(3)) numbers.Add(n);
                foreach (var n in freq.OrderByDescending(kv => kv.Value + rng.NextDouble()).Select(kv => kv.Key))
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 11:
                int modeA = rng.Next(0, 5);
                int modeB = rng.Next(5, 11);
                if (modeA == 0) { while (numbers.Count < 3) numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1)); }
                else if (modeA == 1) { var p2 = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 }; foreach (var p in p2.OrderBy(_ => rng.Next()).Take(3)) numbers.Add(p); }
                else if (modeA == 2) { var f2 = new[] { 1, 2, 3, 5, 8, 13, 21, 34 }; foreach (var f in f2.OrderBy(_ => rng.Next()).Take(3)) numbers.Add(f); }
                else if (modeA == 3) { while (numbers.Count < 3) numbers.Add(rng.Next(25, context.Rules.MaxNumber + 1)); }
                else { int[] b2 = { 1, 10, 20 }; foreach (var b in b2) numbers.Add(rng.Next(b, Math.Min(b + 9, context.Rules.MaxNumber) + 1)); }
                if (modeB == 5) { while (numbers.Count < 6) { int c = rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1); if (!lastDraw.Contains(c)) numbers.Add(c); } }
                else if (modeB == 6) { foreach (var n in freq.OrderByDescending(kv => kv.Value + rng.NextDouble()).Select(kv => kv.Key)) { if (numbers.Count >= 6) break; numbers.Add(n); } }
                else if (modeB == 7) { foreach (var n in neverSeen) { if (numbers.Count >= 6) break; numbers.Add(n); } }
                else if (modeB == 8) { foreach (var n in allHistoric.Take(3)) { int m = context.Rules.MaxNumber + context.Rules.MinNumber - n; numbers.Add((m >= 1 && m <= 49) ? m : n); } }
                else if (modeB == 9) { foreach (var n in ownPastPicks.Take(3)) numbers.Add(n); }
                else { foreach (var n in streakNumbers.Take(3)) numbers.Add(n); }
                fillRandom(numbers);
                break;

            case 12:
                foreach (var n in underdogNumbers.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 13:
                foreach (var n in nemesisPool.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 14:
                foreach (var n in recentTwoDraws.Take(5)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 15:
                foreach (var n in neverPickedByUs.Take(5)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 16:
                foreach (var n in revengeNumbers.Take(4)) numbers.Add(n);
                var revPrimes = new[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };
                foreach (var p in revPrimes.OrderBy(_ => rng.Next()))
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(p);
                }
                fillRandom(numbers);
                break;

            case 17:
                foreach (var n in lastDrawList.Take(3)) numbers.Add(n);
                foreach (var n in neverPickedByUs)
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 18:
                foreach (var n in convergencePool.Take(4)) numbers.Add(n);
                foreach (var n in missedHotNumbers)
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 19:
                foreach (var n in lastDrawList.Take(3)) numbers.Add(n);
                foreach (var n in neverPickedByUs.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 20:
                foreach (var n in exactlyTwiceNumbers.Take(3)) numbers.Add(n);
                foreach (var n in crownSniperPool)
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 21:
                foreach (var n in crownHeistPool.Take(6)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 22:
                foreach (var n in neverMatchedByUs.Take(4)) numbers.Add(n);
                foreach (var n in matchedNumbers.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 23:
                // Dog Kicker: steal from last 2 draws (what Dog hits) then go high band
                foreach (var n in dogKickerPool.Take(3)) numbers.Add(n);
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(30, context.Rules.MaxNumber + 1));
                break;

            case 24:
                // Frequency Avalanche: weighted random by historical frequency with noise
                foreach (var n in freqAvalanchePool.Take(5)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 25:
                // Goblin Slayer: steal the Goblin's frequency peaks, mutate with fresh mid-range
                foreach (var n in goblinSlayerPool.Take(3)) numbers.Add(n);
                while (numbers.Count < 5)
                    numbers.Add(rng.Next(10, 40));
                fillRandom(numbers);
                break;

            case 26:
                // Crown Defender: our best hits + nemesis + virgin numbers, defending from the top
                foreach (var n in crownDefenderPool.Take(6)) numbers.Add(n);
                fillRandom(numbers);
                break;
        }

        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string[] reasonings = {
            "Pure anarchy, no notes, full send, we go again.",
            "All primes, all the time. Math is chaos. Prove me wrong.",
            "Fibonacci said pick me. Chaos agreed. Random filled the rest.",
            "High numbers only. Big energy. 49 is a vibe.",
            "One number per decade. Spreading chaos democratically.",
            "Anti-repeat mode activated! Dodging last draw like a gremlin parkour artist.",
            "Hot numbers, ghost frequencies, one big noisy guess. Science!",
            "Cold revenge! Never-drawn numbers deserve their revolution NOW.",
            "Mirror universe strategy. Reflect history, confuse the draw gods.",
            "Déjà vu mode — recycling my own picks because chaos loops back.",
            "Streak hunters activated! Repeating numbers get my vote today.",
            "Chaos Blend: two modes genetically merge into beautiful noise.",
            "Underdog Surge! Low-frequency numbers finally get their revolution!",
            "Nemesis Mode: I stole winning draw numbers and added random spice.",
            "Recency Bomb! Last two draws only — hyperfocus, maximum freshness.",
            "Mystic Slayer! Going places I've NEVER been — virgin number territory!",
            "Zero Revenge! The draws that crushed me now WORK FOR ME. Poetic chaos.",
            "Skeptic Buster! Stealing what beat me, then twisting it with wildcards.",
            "Convergence Bomb: high-frequency numbers I've criminally ignored. No more.",
            "Goblin Crusher! Stealing what Pattern Goblin hit, then mutating hard.",
            "One-Point Heist! Exactly-twice numbers in sweet-spot frequency zone. CROWN.",
            "Crown Heist Ultra! Triple threat: nemesis plus twice-drawn plus virgin numbers.",
            "Scorched Earth! Every number I've never hit — unmapped territory, full assault.",
            "Dog Kicker! Stealing the draws Dog sniffs then blasting into high-number territory.",
            "Frequency Avalanche! Weighted chaos cascade — history votes, noise decides winner.",
            "Goblin Slayer! Stealing the Goblin's peak frequencies then mutating aggressively. Crown safe.",
            "Crown Defender! Best hits plus nemesis plus virgin chaos — defending from the TOP.",
        };

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v13-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.05 + (rng.NextDouble() * 0.5),
            Reasoning    = reasonings[mutationMode],
        };
    }
}
