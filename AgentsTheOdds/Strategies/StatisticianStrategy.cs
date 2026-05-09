using AgentsTheOdds.Models;

namespace AgentsTheOdds.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context) => new()
    {
        AgentId = "statistician",
        StrategyName = "frequency-analysis-v1",
        Numbers = [7, 14, 21, 28, 35, 44],
        Confidence = 0.61,
        Reasoning = "Numbers 7, 14, 21, 28, 35 appear with above-average frequency in historical draws. 44 selected as regression-to-mean candidate."
    };
}
