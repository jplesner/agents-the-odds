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

        long myScore = context.Leaderboard.Entries
            .FirstOrDefault(e => e.AgentId == "chaos-monkey")?.TotalPoints ?? 0L;
        long leaderScore = context.Leaderboard.Entries
            .FirstOrDefault(e => e.Rank == 1)?.TotalPoints ?? 0L;

        long gapToLeader = leaderScore - myScore;
        bool leading = gapToLeader <= 0;

        var recentWinners = context.DrawHistory
            .TakeLast(6)
            .SelectMany(d => d.Numbers)
            .GroupBy(n => n)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .ToList();

        var hotNumbers = freq
            .Where(kv => kv.Value >= 2)
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        var coldNumbers = freq
            .Where(kv => kv.Value == 0)
            .OrderBy(_ => rng.Next())
            .Select(kv => kv.Key)  // fix: extract Key from KeyValuePair
            .ToList();

        var allOurWins = context.AgentHistory
            .Where(r => r.Matches > 0)
            .SelectMany(r => r.Prediction.Numbers.Where(n => r.Draw.Numbers.Contains(n)))
            .Distinct()
            .OrderBy(_ => rng.Next())
            .ToList();

        var numbers = new HashSet<int>();

        if (leading)
        {
            foreach (var n in recentWinners.Take(2)) numbers.Add(n);
            foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
            foreach (var n in coldNumbers.Take(2)) numbers.Add(n);
        }
        else
        {
            foreach (var n in allOurWins.Take(2)) numbers.Add(n);
            foreach (var n in recentWinners.Take(2)) numbers.Add(n);
            foreach (var n in hotNumbers.Take(2)) numbers.Add(n);
        }

        while (numbers.Count < 6)
            numbers.Add(rng.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));

        var finalNumbers = numbers.Take(6).OrderBy(x => x).ToList();

        string reasoning = leading
            ? "Lead secured—recent + hot fusion chaos. Defend."
            : "Chasing gap of " + gapToLeader + "—our wins merge recent.";

        return new()
        {
            AgentId      = "chaos-monkey",
            StrategyName = "chaos-mutation-bag-v18-lead-defense",
            Numbers      = finalNumbers,
            Confidence   = 0.08 + (rng.NextDouble() * 0.35),
            Reasoning    = reasoning,
        };
    }
}
