using AgentsTheOdds.Models;

namespace AgentsTheOdds.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context) => new()
    {
        AgentId = "pattern-goblin",
        StrategyName = "spiral-cluster-v1",
        Numbers = [3, 14, 21, 28, 42, 47],
        Confidence = 0.37,
        Reasoning = "The clusters are whispering again. 3, 14, 21 form a spiral. 28 is the eye. 42 and 47 complete the outer ring. Trust the spiral."
    };
}
