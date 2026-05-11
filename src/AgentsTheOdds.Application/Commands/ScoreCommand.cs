using AgentsTheOdds.Domain;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using AgentsTheOdds.Domain.Services;

namespace AgentsTheOdds.Application.Commands;

public sealed class ScoreCommand(
    IAgentRepository agents,
    IEpisodePredictionRepository predictions,
    IDrawResultRepository draws,
    ILeaderboardRepository leaderboards,
    IEpisodeResultRepository episodeResults,
    IRecapWriter recapWriter,
    IRealityCheckGenerator realityCheckGenerator)
{
    public int Execute(int episodeNumber)
    {
        if (!predictions.Exists(episodeNumber))
        {
            Console.Error.WriteLine(
                $"No predictions found for episode {episodeNumber}. " +
                $"Run `predict --episode {episodeNumber}` first.");
            return 1;
        }

        var draw = draws.TryGetByEpisode(episodeNumber);
        if (draw is null)
        {
            Console.Error.WriteLine(
                $"Draw result for episode {episodeNumber} not found. " +
                $"Expected: data/draws/episode-{episodeNumber:D3}.json");
            return 1;
        }

        var predictionSet = predictions.Get(episodeNumber);
        var scores = predictionSet.Predictions
            .Select(p => Scorer.Score(p, draw))
            .ToList();

        var allAgents = agents.GetAll();
        var updatedBoard = LeaderboardMerger.Merge(leaderboards.Load(), scores, allAgents);
        var realityCheck = realityCheckGenerator.Generate(episodeNumber, scores);

        var episodeResult = new EpisodeResult
        {
            EpisodeNumber = episodeNumber,
            DrawResult = draw,
            Scores = scores,
            Leaderboard = updatedBoard.Entries,
            RealityCheck = realityCheck,
        };

        episodeResults.Save(episodeResult);
        recapWriter.Write(episodeResult);
        leaderboards.Save(updatedBoard);

        Console.WriteLine(
            $"Episode {episodeNumber} scored. " +
            $"Results written to data/episodes/episode-{episodeNumber:D3}.json");
        return 0;
    }
}
