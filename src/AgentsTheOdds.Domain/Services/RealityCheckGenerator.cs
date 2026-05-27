using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Services;

public sealed class RealityCheckGenerator : IRealityCheckGenerator
{
    public string Generate(int episodeNumber, IReadOnlyList<PredictionResult> scores, IReadOnlyList<Agent> agents)
    {
        if (scores.Count == 0)
            return $"Episode {episodeNumber}: No predictions were scored.";

        var nameById = agents.ToDictionary(a => a.Id, a => a.Name);
        string DisplayName(string id) => nameById.TryGetValue(id, out var n) ? n : id;

        var maxPoints = scores.Max(s => s.Points);
        var topScorers = scores
            .Where(s => s.Points == maxPoints)
            .OrderBy(s => s.Prediction.AgentId)
            .ToList();

        var totalPoints = scores.Sum(s => s.Points);
        var matches = topScorers[0].Matches;
        var matchWord = matches == 1 ? "match" : "matches";

        string leader = topScorers.Count == 1
            ? $"{DisplayName(topScorers[0].Prediction.AgentId)} led with {maxPoints} pts ({matches} {matchWord})"
            : $"{FormatTie(topScorers, DisplayName)} tied with {maxPoints} pts ({matches} {matchWord} each)";

        return $"Episode {episodeNumber}: {leader}. Combined table points this episode: {totalPoints}.";
    }

    private static string FormatTie(List<PredictionResult> agents, Func<string, string> displayName)
    {
        if (agents.Count == 2)
            return $"{displayName(agents[0].Prediction.AgentId)} and {displayName(agents[1].Prediction.AgentId)}";
        var allButLast = agents.Take(agents.Count - 1).Select(a => displayName(a.Prediction.AgentId));
        return string.Join(", ", allButLast) + $", and {displayName(agents[^1].Prediction.AgentId)}";
    }
}
