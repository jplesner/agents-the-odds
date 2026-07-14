using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Chaos Monkey Episode 9: ONE POINT DOWN FROM THE SKEPTIC WHO JUST WENT ZERO.
        // Pattern Goblin popped 5 pts with mode33 or whatever. We are at 12, Skeptic at 13.
        // THE GAP IS ONE POINT. ONE. SINGULAR. SOLITARY. POINT.
        // New modes: Mode 19 = Goblin Crusher (copy what Pattern Goblin just hit, mutate hard)
        // Mode 20 = One-Point Heist (laser-focus on the numbers that have appeared TWICE in 8 draws)
        // 21 CHAMBERS OF CHAOS. THE CROWN IS ONE POINT AWAY. LET'S GO.

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

        // Desperation multiplier: the longer the zero streak, the wilder the seed
        long desperationMult = (long)(zeroStreak * zeroStreak) * 0xDEADF00DL;

        long recentScoreMood = context.AgentHistory.TakeLast(3)
            .Aggregate(0L, (acc, r) => acc ^ ((long)(r.Points + zeroStreak + episode) * 0xBEEF13L));

        // Crown gap: 1 pt behind Skeptic — MAX CHAOS FUEL
        long crownGapFuel = (rankPressure == 1) ? 0L : (rankPressure * 0xC0DE420L);

        // Episode 8 winners: numbers that recently scored (Pattern Goblin hit 33, 43)
        long goblinFuel = context.DrawHistory.Count > 0
            ? context.DrawHistory[^1].Numbers.Aggregate(0L, (acc, n) => acc ^ ((long)n * 0xF00DCAFEL))
            : 0L;

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
            ^ goblinFuel
            ^ 0xF00DCAFE9999L;

        var rng = new Random((int)(seed & 0x7FFFFFFF));

        // EPISODE 9 MUTATION BAG — 21 MODES. ONE POINT FROM THE CROWN.
        int mutationMode = rng.Next(21);

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

        // Numbers drawn historically
        var allHistoric = freq.Where(kv => kv.Value > 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

        // Numbers we ourselves have picked before
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

        // Underdog numbers: lowest frequency (but appeared at least once)
        var underdogNumbers = freq.Where(kv => kv.Value > 0)
            .OrderBy(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        // Nemesis pool: numbers from recent draws (top 3)
        var nemesisPool = context.DrawHistory
            .OrderByDescending(d => d.DrawNumber)
            .Take(3)
            .SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .ThenBy(_ => rng.Next())
            .Select(g => g.Key)
            .ToList();

        // Recency Bomb: last 2 draws only
        var recentTwoDraws = context.DrawHistory.TakeLast(2).SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .ThenBy(_ => rng.Next())
            .Select(g => g.Key)
            .ToList();

        // Numbers we've NEVER picked ourselves (virgin territory)
        var ourPicksSet = new HashSet<int>(context.AgentHistory.SelectMany(r => r.Prediction.Numbers));
        var neverPickedByUs = Enumerable.Range(context.Rules.MinNumber, context.Rules.MaxNumber)
            .Where(n => !ourPicksSet.Contains(n))
            .OrderBy(_ => rng.Next())
            .ToList();

        // Numbers that appeared in draws right after OUR zero episodes (revenge data)
        var revengeNumbers = context.AgentHistory
            .Where(r => r.Points == 0)
            .Select(r => r.Draw)
            .SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .ThenBy(_ => rng.Next())
            .Select(g => g.Key)
            .ToList();

        // Skeptic Buster: numbers from the last draw + never-seen wildcards
        var lastDrawList = context.DrawHistory.Count > 0
            ? context.DrawHistory[^1].Numbers.OrderBy(_ => rng.Next()).ToList()
            : new List<int>();

        // Convergence Bomb: numbers that appear in MULTIPLE recent draws but we haven't picked them
        var convergencePool = freq
            .Where(kv => kv.Value >= 2)
            .Where(kv => !ourPicksSet.Contains(kv.Key))
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        // Numbers that historically landed in the draw but we consistently missed
        var missedHotNumbers = freq
            .Where(kv => kv.Value >= 3)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        // Goblin Crusher: numbers that appeared in last draw (what Pattern Goblin hit!)
        // They got 33 and 43 from draw [5, 7, 25, 30, 33, 43]. We take some + twist.
        var goblinPool = lastDrawList; // same as lastDrawList but aliased for clarity

        // One-Point Heist: numbers that appeared EXACTLY TWICE in all history — the "due" zone
        // Not too cold, not too hot — the sweet spot frequency of 2
        var exactlyTwiceNumbers = freq
            .Where(kv => kv.Value == 2)
            .OrderBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        // Crown Sniper: numbers near the median of all draws (structured chaos targeting 20-35 band)
        var crownSniperPool = freq
            .Where(kv => kv.Key >= 20 && kv.Key <= 35 && kv.Value > 0)
            .OrderByDescending(kv => kv.Value)
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
                // Anti-repeat: avoid last draw numbers
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
                // Mirror mode: reflect historic numbers around midpoint
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
                // Déjà vu: reuse our own past picks
                foreach (var n in ownPastPicks.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 10:
                // Streak hunter: numbers appearing in 2+ of last 3 draws
                foreach (var n in streakNumbers.Take(3)) numbers.Add(n);
                foreach (var n in freq.OrderByDescending(kv => kv.Value + rng.NextDouble()).Select(kv => kv.Key))
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 11:
                // Chaos Blend: merge two sub-modes
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
                // Underdog Surge: rarely-drawn numbers get their moment
                foreach (var n in underdogNumbers.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 13:
                // Nemesis Mode: steal from what the actual draws produced recently
                foreach (var n in nemesisPool.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 14:
                // Recency Bomb: ONLY care about last 2 draws — hyperfocus
                foreach (var n in recentTwoDraws.Take(5)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 15:
                // Mystic Slayer: virgin territory, numbers we've never tried
                foreach (var n in neverPickedByUs.Take(5)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 16:
                // Zero Revenge: draws that crushed us now work for us
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
                // Skeptic Buster: steal 3 numbers from last draw + inject never-picked wildcards
                foreach (var n in lastDrawList.Take(3)) numbers.Add(n);
                foreach (var n in neverPickedByUs)
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 18:
                // Convergence Bomb: high-frequency numbers we've criminally ignored
                foreach (var n in convergencePool.Take(4)) numbers.Add(n);
                foreach (var n in missedHotNumbers)
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;

            case 19:
                // Goblin Crusher: steal what Pattern Goblin just hit (last draw)
                // then cross-pollinate with never-picked territory to go further
                foreach (var n in goblinPool.Take(3)) numbers.Add(n);
                foreach (var n in neverPickedByUs.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 20:
                // One-Point Heist: numbers that appeared EXACTLY TWICE — "due frequency" zone
                // Not too cold, not too hot. Sweet spot. Plus crown sniper range.
                foreach (var n in exactlyTwiceNumbers.Take(3)) numbers.Add(n);
                foreach (var n in crownSniperPool)
                {
                    if (numbers.Count >= 6) break;
                    numbers.Add(n);
                }
                fillRandom(numbers);
                break;
        }

        // Safety net: exactly 6 valid numbers
        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string[] reasonings = {
            "Pure anarchy, no notes, full send, we go again.",                                          // 0
            "All primes, all the time. Math is chaos. Prove me wrong.",                                 // 1
            "Fibonacci said pick me. Chaos agreed. Random filled the rest.",                            // 2
            "High numbers only. Big energy. 49 is a vibe.",                                             // 3
            "One number per decade. Spreading chaos democratically.",                                   // 4
            "Anti-repeat mode activated! Dodging last draw like a gremlin parkour artist.",             // 5
            "Hot numbers, ghost frequencies, one big noisy guess. Science!",                            // 6
            "Cold revenge! Never-drawn numbers deserve their revolution NOW.",                          // 7
            "Mirror universe strategy. Reflect history, confuse the draw gods.",                        // 8
            "Déjà vu mode — recycling my own picks because chaos loops back.",                         // 9
            "Streak hunters activated! Repeating numbers get my vote today.",                           // 10
            "Chaos Blend: two modes genetically merge into beautiful noise.",                           // 11
            "Underdog Surge! Low-frequency numbers finally get their revolution!",                      // 12
            "Nemesis Mode: I stole winning draw numbers and added random spice.",                       // 13
            "Recency Bomb! Last two draws only — hyperfocus, maximum freshness.",                       // 14
            "Mystic Slayer! Going places I've NEVER been — virgin number territory!",                   // 15
            "Zero Revenge! The draws that crushed me now WORK FOR ME. Poetic chaos.",                  // 16
            "Skeptic Buster! Stealing what beat me, then twisting it with wildcards.",                  // 17
            "Convergence Bomb: high-frequency numbers I've criminally ignored. No more.",               // 18
            "Goblin Crusher! Stealing what Pattern Goblin hit, then mutating hard.",                    // 19
            "One-Point Heist! Exactly-twice numbers in sweet-spot frequency zone. CROWN.",              // 20
        };

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v10-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.05 + (rng.NextDouble() * 0.5),
            Reasoning    = reasonings[mutationMode],
        };
    }
}
