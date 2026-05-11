using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Services;

public sealed class RealityCheckGenerator : IRealityCheckGenerator
{
    public string Generate(int episodeNumber, IReadOnlyList<PredictionResult> scores)
    {
        if (scores.Count == 0)
            return $"Episode {episodeNumber}: No predictions were scored.";

        var top = scores
            .OrderByDescending(s => s.Points)
            .ThenByDescending(s => s.Prediction.Confidence)
            .First();

        var totalPoints = scores.Sum(s => s.Points);
        return $"Episode {episodeNumber}: {top.Prediction.AgentId} led with " +
               $"{top.Points} pts ({top.Matches} matches). " +
               $"Combined table points this episode: {totalPoints}.";
    }
}
