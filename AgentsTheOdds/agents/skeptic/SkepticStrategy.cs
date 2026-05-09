using AgentsTheOdds.Models;

namespace AgentsTheOdds.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context) => new()
    {
        AgentId = "skeptic",
        StrategyName = "anti-bias-v1",
        Numbers = [1, 9, 22, 29, 36, 43],
        Confidence = 0.45,
        Reasoning = "I have deliberately avoided all numbers other agents are likely to pick. This strategy is statistically sound and will probably also fail. I am at peace with this."
    };
}
