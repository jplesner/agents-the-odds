using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        return new()
        {
            AgentId      = "mystic",
            StrategyName = "my-algo-name-v1",
            Numbers      = [1, 2, 3, 4, 5, 6],
            Confidence   = 0.1,
            Reasoning    = "my reason",
        };
    }
}