using AgentsTheOdds.Domain;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Application;

public sealed class GameRunner(
    IDrawRepository draws,
    IAgentRepository agents,
    IPredictionRepository predictions,
    IGamePresenter presenter)
{
    public Task RunAsync()
    {
        var draw      = draws.GetCurrent();
        var allAgents = agents.GetAll();
        var rules     = LotteryRules.Standard;

        var context = new PredictionContext
        {
            Rules        = rules,
            DrawHistory  = draws.GetHistory(),
            AgentHistory = [],
            Leaderboard  = Leaderboard.Empty,
        };

        presenter.ShowHeader(draw);

        var agentById = allAgents.ToDictionary(a => a.Id);

        foreach (var agent in allAgents)
        {
            var prediction = agent.Strategy.GeneratePrediction(context);
            var validation = LotteryValidator.Validate(prediction, rules);

            if (!validation.IsValid)
            {
                presenter.ShowInvalidPrediction(agent.Name, validation.Error!);
                continue;
            }

            predictions.Add(Scorer.Score(prediction, draw));
        }

        var ranked = predictions.GetAll()
            .OrderByDescending(r => r.Points)
            .ThenByDescending(r => r.Prediction.Confidence)
            .Select(r => (Agent: agentById[r.Prediction.AgentId], Result: r))
            .ToList();

        presenter.ShowPredictions(ranked);
        presenter.ShowLeaderboard(ranked);

        return Task.CompletedTask;
    }
}
