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

        long gapToLeader = skepticScore - totalScore;
        bool trailing = gapToLeader > 4;

        var hotNumbers = freq
            .Where(kv => kv.Value >= 2)
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        var recentWinners = context.DrawHistory
            .TakeLast(6)
            .SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var ourWinningNumbers = context.AgentHistory
            .Where(r => r.Matches > 0)
            .SelectMany(r => r.Prediction.Numbers.Where(n => r.Draw.Numbers.Contains(n)))
            .Distinct()
            .ToList();

        var coldNumbers = freq.Where(kv => kv.Value == 0).Select(kv => kv.Key).OrderBy(_ => rng.Next()).ToList();

        var numbers = new HashSet<int>();

        if (trailing)
        {
            foreach (var n in recentWinners.Take(3)) numbers.Add(n);
            foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
        }
        else
        {
            foreach (var n in ourWinningNumbers.Take(2)) numbers.Add(n);
            foreach (var n in recentWinners.Take(2)) numbers.Add(n);
            foreach (var n in coldNumbers.Take(2)) numbers.Add(n);
        }

        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string reasoning = trailing
            ? "Trailing by " + gapToLeader + "—recency attack mode. Recent winners fusion."
            : "Tied, precision strategy. Our wins plus cold shock therapy.";

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = $"chaos-mutation-bag-v17-recency-fusion",
            Numbers      = finalNumbers,
            Confidence   = 0.08 + (rng.NextDouble() * 0.35),
            Reasoning    = reasoning,
        };
    }
}
