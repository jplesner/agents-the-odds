using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Services;

public static class LeaderboardMerger
{
    public static Leaderboard Merge(
        Leaderboard existing,
        IReadOnlyList<PredictionResult> newScores,
        IReadOnlyList<Agent> agents)
    {
        var agentNames = agents.ToDictionary(a => a.Id, a => a.Name);

        var accumulated = existing.Entries
            .ToDictionary(e => e.AgentId, e => e.TotalPoints);

        foreach (var score in newScores)
        {
            var id = score.Prediction.AgentId;
            accumulated[id] = accumulated.GetValueOrDefault(id) + score.Points;
        }

        var entries = accumulated
            .OrderByDescending(kv => kv.Value)
            .Select((kv, i) => new LeaderboardEntry
            {
                AgentId = kv.Key,
                AgentName = agentNames.GetValueOrDefault(kv.Key, kv.Key),
                TotalPoints = kv.Value,
                Rank = i + 1,
            })
            .ToList();

        return new Leaderboard { Entries = entries };
    }
}
