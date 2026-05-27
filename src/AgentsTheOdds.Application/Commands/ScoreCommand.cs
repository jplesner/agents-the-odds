using AgentsTheOdds.Domain;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using AgentsTheOdds.Domain.Services;

namespace AgentsTheOdds.Application.Commands;

public sealed class ScoreCommand(
    IAgentRepository agents,
    IEpisodePredictionRepository predictions,
    IDrawRepository draws,
    ILeaderboardRepository leaderboards,
    IEpisodeResultRepository episodeResults,
    IRecapWriter recapWriter,
    IRealityCheckGenerator realityCheckGenerator)
{
    public int Execute(int episodeNumber, bool force = false)
    {
        if (!predictions.Exists(episodeNumber))
        {
            Console.Error.WriteLine(
                $"No predictions found for episode {episodeNumber}. " +
                $"Run `predict --episode {episodeNumber}` first.");
            return 1;
        }

        DrawResult draw;
        try { draw = draws.GetByEpisode(episodeNumber); }
        catch (InvalidOperationException)
        {
            Console.Error.WriteLine(
                $"Draw not found for episode {episodeNumber}. " +
                $"Run `draw --episode {episodeNumber}` first.");
            return 1;
        }

        if (episodeResults.TryGet(episodeNumber) != null && !force)
        {
            Console.Error.WriteLine(
                $"Episode {episodeNumber} has already been scored. Use --force to re-score.");
            return 1;
        }

        var predictionSet = predictions.Get(episodeNumber);
        var scores = predictionSet.Predictions
            .Select(p => Scorer.Score(p, draw))
            .ToList();

        var allAgents = agents.GetAll();
        var realityCheck = realityCheckGenerator.Generate(episodeNumber, scores, allAgents);

        var board = Leaderboard.Empty;
        foreach (var prior in episodeResults.GetAll()
                     .Where(r => r.EpisodeNumber != episodeNumber)
                     .OrderBy(r => r.EpisodeNumber))
            board = LeaderboardMerger.Merge(board, prior.Scores, allAgents);
        board = LeaderboardMerger.Merge(board, scores, allAgents);

        var episodeResult = new EpisodeResult
        {
            EpisodeNumber = episodeNumber,
            DrawResult = draw,
            Scores = scores,
            Leaderboard = board.Entries,
            RealityCheck = realityCheck,
        };

        episodeResults.Save(episodeResult);
        recapWriter.Write(episodeResult);
        leaderboards.Save(board);

        Console.WriteLine(
            $"Episode {episodeNumber} scored. " +
            $"Results written to data/episodes/episode-{episodeNumber:D3}.json");
        return 0;
    }
}
