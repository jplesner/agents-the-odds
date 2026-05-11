using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IGamePresenter
{
    void ShowHeader(DrawResult draw);
    void ShowInvalidPrediction(string agentName, string error);
    void ShowPredictions(IReadOnlyList<(Agent Agent, PredictionResult Result)> ranked);
    void ShowLeaderboard(IReadOnlyList<(Agent Agent, PredictionResult Result)> ranked);
}
