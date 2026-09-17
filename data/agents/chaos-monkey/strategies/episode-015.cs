using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class ChaosMonkeyStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        int episode = context.AgentHistory.Count + 1;

        long seed = DateTime.UtcNow.Ticks
            ^ (episode * 0xCAFEBABEL)
            ^ context.DrawHistory.Aggregate(0L, (acc, d) => acc ^ (long)d.DrawNumber * 0xDEADBEEFL);

        var rng = new Random((int)(seed & 0x7FFFFFFF));

        var freq = new Dictionary<int, int>();
        for (int i = 1; i <= context.Rules.MaxNumber; i++) freq[i] = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                freq[n]++;

        long totalScore = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "chaos-monkey")?.TotalPoints ?? 0L;
        long skepticScore = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "skeptic")?.TotalPoints ?? 0L;
        long mysticScore = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "mystic")?.TotalPoints ?? 0L;

        long gapToLeader = skepticScore - totalScore;
        long mutationMode = (gapToLeader > 5) ? rng.Next(0, 10) : rng.Next(10, 16);

        var hotNumbers = freq
            .Where(kv => kv.Value >= 2)
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        var ourPicksSet = new HashSet<int>(context.AgentHistory.SelectMany(r => r.Prediction.Numbers));
        var neverPickedByUs = Enumerable.Range(context.Rules.MinNumber, context.Rules.MaxNumber)
            .Where(n => !ourPicksSet.Contains(n))
            .OrderBy(_ => rng.Next())
            .ToList();

        var matchedNumbers = context.AgentHistory
            .Where(r => r.Matches > 0)
            .SelectMany(r => r.Prediction.Numbers.Where(n => r.Draw.Numbers.Contains(n)))
            .Distinct()
            .OrderBy(_ => rng.Next())
            .ToList();

        var neverSeen = freq.Where(kv => kv.Value == 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

        var numbers = new HashSet<int>();

        Action<HashSet<int>> fillRandom = (set) => {
            while (set.Count < 6)
                set.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
        };

        if (mutationMode < 5)
        {
            foreach (var n in hotNumbers.Take(3)) numbers.Add(n);
            foreach (var n in neverPickedByUs.Take(3)) numbers.Add(n);
            fillRandom(numbers);
        }
        else if (mutationMode < 10)
        {
            foreach (var n in matchedNumbers.Take(2)) numbers.Add(n);
            foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
            foreach (var n in neverPickedByUs.Take(2)) numbers.Add(n);
            fillRandom(numbers);
        }
        else
        {
            var recentNumbers = context.DrawHistory
                .TakeLast(4)
                .SelectMany(d => d.Numbers)
                .GroupBy(n => n)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .ToList();

            foreach (var n in recentNumbers.Take(2)) numbers.Add(n);
            foreach (var n in neverSeen.Take(2)) numbers.Add(n);
            foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
            fillRandom(numbers);
        }

        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string reasoning = gapToLeader > 5
            ? "Trailing by " + gapToLeader + ", wild modes deployed. Chaos fury unleashed."
            : "Close race, precision chaos. Frequency blitz fusion mode active.";

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v16-mode{mutationMode}",
            Numbers      = finalNumbers,
            Confidence   = 0.10 + (rng.NextDouble() * 0.40),
            Reasoning    = reasoning,
        };
    }
}
