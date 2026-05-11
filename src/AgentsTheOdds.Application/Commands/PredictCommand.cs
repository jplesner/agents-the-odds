using AgentsTheOdds.Domain;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Application.Commands;

public sealed class PredictCommand(
    IAgentRepository agents,
    ILeaderboardRepository leaderboards,
    IEpisodePredictionRepository predictions,
    IEpisodeResultRepository episodeResults)
{
    public int Execute(int episodeNumber, bool force)
    {
        if (predictions.Exists(episodeNumber) && !force)
        {
            Console.Error.WriteLine(
                $"Predictions for episode {episodeNumber} already exist. " +
                "Use --force to overwrite.");
            return 1;
        }

        var allAgents = agents.GetAll();
        var pastEpisodes = episodeResults.GetAll();
        var drawHistory = pastEpisodes.Select(e => e.DrawResult).ToList();
        var leaderboard = leaderboards.Load();
        var rules = LotteryRules.Standard;

        var allPastScores = pastEpisodes.SelectMany(e => e.Scores).ToList();

        var validPredictions = new List<Prediction>();
        var hasError = false;

        foreach (var agent in allAgents)
        {
            var context = new PredictionContext
            {
                Rules = rules,
                DrawHistory = drawHistory,
                AgentHistory = allPastScores.Where(s => s.Prediction.AgentId == agent.Id).ToList(),
                Leaderboard = leaderboard,
            };

            var prediction = agent.Strategy.GeneratePrediction(context);
            var validation = LotteryValidator.Validate(prediction, rules);

            if (!validation.IsValid)
            {
                Console.Error.WriteLine($"[INVALID] {agent.Name}: {validation.Error}");
                hasError = true;
                continue;
            }

            validPredictions.Add(prediction);
        }

        if (hasError)
        {
            Console.Error.WriteLine("One or more predictions were invalid. No file was written.");
            return 2;
        }

        var set = new EpisodePredictionSet
        {
            EpisodeNumber = episodeNumber,
            PredictionDate = DateOnly.FromDateTime(DateTime.Today),
            Predictions = validPredictions,
        };

        predictions.Save(set);
        Console.WriteLine($"Saved {validPredictions.Count} predictions for episode {episodeNumber}.");
        return 0;
    }
}
