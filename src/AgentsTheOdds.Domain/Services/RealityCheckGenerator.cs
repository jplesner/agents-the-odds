using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Services;

public sealed class RealityCheckGenerator : IRealityCheckGenerator
{
    public string Generate(int episodeNumber, IReadOnlyList<PredictionResult> scores)
    {
        if (scores.Count == 0)
            return $"Episode {episodeNumber}: No predictions were scored.";

        var maxPoints = scores.Max(s => s.Points);
        var topScorers = scores
            .Where(s => s.Points == maxPoints)
            .OrderBy(s => s.Prediction.AgentId)
            .ToList();

        var totalPoints = scores.Sum(s => s.Points);
        var matches = topScorers[0].Matches;
        var matchWord = matches == 1 ? "match" : "matches";

        string leader = topScorers.Count == 1
            ? $"{topScorers[0].Prediction.AgentId} led with {maxPoints} pts ({matches} {matchWord})"
            : $"{FormatTie(topScorers)} tied with {maxPoints} pts ({matches} {matchWord} each)";

        return $"Episode {episodeNumber}: {leader}. Combined table points this episode: {totalPoints}.";
    }

    private static string FormatTie(List<PredictionResult> agents)
    {
        if (agents.Count == 2)
            return $"{agents[0].Prediction.AgentId} and {agents[1].Prediction.AgentId}";
        var allButLast = agents.Take(agents.Count - 1).Select(a => a.Prediction.AgentId);
        return string.Join(", ", allButLast) + $", and {agents[^1].Prediction.AgentId}";
    }
}
