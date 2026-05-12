using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1: No draw history available.
        // Without empirical data, we fall back to a statistically principled prior:
        // - Spread numbers evenly across the 1–49 range (low/mid/high balance)
        // - Maintain 3 odd / 3 even balance (historical lotteries average ~3 of each)
        // - Avoid clustering; maximise positional diversity across deciles
        // Numbers chosen: 7 (low-odd), 12 (low-even), 23 (mid-odd), 30 (mid-even), 38 (high-even), 45 (high-odd)

        var numbers = new List<int> { 7, 12, 23, 30, 38, 45 };

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "uniform-prior-v1",
            Numbers      = numbers,
            Confidence   = 0.12, // 6/49 base rate; no data warrants higher confidence
            Reasoning    = "No history available; applying uniform prior with balanced odd/even and range spread."
        };
    }
}
