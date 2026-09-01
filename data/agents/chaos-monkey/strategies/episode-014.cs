using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        int episode = context.AgentHistory.Count + 1;

        long historyHash = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                historyHash ^= (long)n * draw.DrawNumber * 6997L;

        long agentHistoryHash = 0;
        foreach (var r in context.AgentHistory)
            foreach (var n in r.Prediction.Numbers)
                agentHistoryHash ^= (long)n * (r.Points + 1) * 3571L;

        long totalScore = context.AgentHistory.Aggregate(0L, (acc, r) => acc + r.Points);
        long rankPressure = context.Leaderboard.Entries.FirstOrDefault(e => e.AgentId == "chaos-monkey")?.Rank ?? 1L;

        long skepticScore = context.Leaderboard.Entries.FirstOrDefault(e => e.AgentId == "skeptic")?.TotalPoints ?? 0L;
        long mysticScore = context.Leaderboard.Entries.FirstOrDefault(e => e.AgentId == "mystic")?.TotalPoints ?? 0L;
        long goblinScore = context.Leaderboard.Entries.FirstOrDefault(e => e.AgentId == "pattern-goblin")?.TotalPoints ?? 0L;

        long rivalryFuel = ((skepticScore - totalScore) * 0xACE5A5EL)
            ^ ((mysticScore + 1L) * 0xDEADC0DEL)
            ^ ((goblinScore + 3L) * 0x60B1175L);

        long crownDefenderEntropy = (rankPressure == 1) ? (totalScore * 0xF33DC0DEL) : 0L;

        long seed = DateTime.UtcNow.Ticks
            ^ (episode * 0xCAFEBABEL)
            ^ historyHash
            ^ agentHistoryHash
            ^ (context.DrawHistory.Count * 0xDEADBEEFL)
            ^ rivalryFuel
            ^ crownDefenderEntropy
            ^ 0xC0FFEE1313L;

        var rng = new Random((int)(seed & 0x7FFFFFFF));

        int mutationMode = rng.Next(16);
        var numbers = new HashSet<int>();

        var freq = new Dictionary<int, int>();
        for (int i = 1; i <= context.Rules.MaxNumber; i++) freq[i] = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                freq[n]++;

        var ourPicksSet = new HashSet<int>(context.AgentHistory.SelectMany(r => r.Prediction.Numbers));
        var neverPickedByUs = Enumerable.Range(context.Rules.MinNumber, context.Rules.MaxNumber)
            .Where(n => !ourPicksSet.Contains(n))
            .OrderBy(_ => rng.Next())
            .ToList();

        var matchedNumbers = context.AgentHistory
            .Where(r => r.Matches > 0)
            .SelectMany(r => r.Prediction.Numbers.Where(n => r.Draw.Numbers.Contains(n)))
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var recentDraws = context.DrawHistory.TakeLast(3).ToList();
        var nemesisPool = context.DrawHistory
            .OrderByDescending(d => d.DrawNumber)
            .Take(3)
            .SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .ThenBy(_ => rng.Next())
            .Select(g => g.Key)
            .ToList();

        var hotNumbers = freq
            .Where(kv => kv.Value >= 2)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => rng.Next())
            .Select(kv => kv.Key)
            .ToList();

        var mysticPicksSet = new HashSet<int>(context.AgentHistory
            .Where(r => r.Prediction.AgentId == "mystic")
            .SelectMany(r => r.Prediction.Numbers));

        var mysticMatches = context.AgentHistory
            .Where(r => r.Prediction.AgentId == "mystic" && r.Matches > 0)
            .SelectMany(r => r.Prediction.Numbers.Where(n => r.Draw.Numbers.Contains(n)))
            .Distinct()
            .ToList();

        var neverSeen = freq.Where(kv => kv.Value == 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

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
                foreach (var n in hotNumbers.Take(3)) numbers.Add(n);
                foreach (var n in neverPickedByUs.Take(3)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 3:
                foreach (var n in nemesisPool.Take(4)) numbers.Add(n);
                foreach (var n in neverSeen.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 4:
                var zeroZoneNumbers = context.AgentHistory
                    .Where(r => r.Points == 0)
                    .Select(r => r.Draw)
                    .SelectMany(d => d.Numbers)
                    .GroupBy(n => n)
                    .OrderByDescending(g => g.Count())
                    .ThenBy(_ => rng.Next())
                    .Select(g => g.Key)
                    .ToList();
                foreach (var n in zeroZoneNumbers.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 5:
                while (numbers.Count < 6)
                    numbers.Add(rng.Next(15, 35));
                break;

            case 6:
                foreach (var n in matchedNumbers.Take(2)) numbers.Add(n);
                foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
                foreach (var n in neverPickedByUs.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 7:
                var lastDraw = context.DrawHistory.Count > 0 ? context.DrawHistory[^1].Numbers : new List<int>();
                foreach (var n in lastDraw) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 8:
                var doubleDips = freq.Where(kv => kv.Value >= 2).OrderByDescending(kv => kv.Value).Select(kv => kv.Key).ToList();
                foreach (var n in doubleDips.Take(4)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 9:
                foreach (var n in neverPickedByUs.Take(4)) numbers.Add(n);
                foreach (var n in matchedNumbers.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 10:
                int[] bands = { 1, 10, 20, 30, 40 };
                foreach (var band in bands)
                    numbers.Add(rng.Next(band, Math.Min(band + 9, context.Rules.MaxNumber) + 1));
                fillRandom(numbers);
                break;

            case 11:
                foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
                foreach (var n in neverSeen.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 12:
                var streakNumbers = freq.Keys
                    .Where(n => recentDraws.Count(d => d.Numbers.Contains(n)) >= 2)
                    .OrderBy(_ => rng.Next())
                    .ToList();
                foreach (var n in streakNumbers.Take(3)) numbers.Add(n);
                foreach (var n in neverPickedByUs.Take(3)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 13:
                var fusion = hotNumbers.Take(2).Concat(matchedNumbers.Take(2)).Concat(neverPickedByUs.Take(2)).Distinct().ToList();
                foreach (var n in fusion) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 14:
                foreach (var n in nemesisPool.Take(3)) numbers.Add(n);
                foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;

            case 15:
                foreach (var n in mysticMatches.Take(2)) numbers.Add(n);
                foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
                foreach (var n in neverPickedByUs.Take(2)) numbers.Add(n);
                fillRandom(numbers);
                break;
        }

        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string[] reasonings = {
            "Pure chaos. No pattern. Gremlin energy full blast.",
            "Primes only. Math is the only constant in chaos.",
            "Hot virgin fusion: proven winners meet unexplored numbers.",
            "Nemesis resurrection: stealing from draws that beat me.",
            "Zero-zone therapy: revenge against the emptiness.",
            "Mid-range sniper: 15-35 territory, sweet spot zone.",
            "Hybrid frankenstein: matches plus hot plus untouched.",
            "Last draw echo: stealing fresh signals directly.",
            "Double-dipper focus: numbers appearing twice deserve votes.",
            "Virgin revival: unleashing the never-picked arsenal.",
            "Decade sweep: one number per band, spreading chaos.",
            "Hot-cold toggle: frequency peaks meet the frozen void.",
            "Streak revenge: repeats within recent window.",
            "Fusion monster: blend all my tools into chaos cocktail.",
            "Nemesis plus hot: leaderboard gap closes through fury.",
            "Mystic theft: stealing The Mystic's winning signature.",
        };

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v15-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.05 + (rng.NextDouble() * 0.45),
            Reasoning    = reasonings[mutationMode],
        };
    }
}
