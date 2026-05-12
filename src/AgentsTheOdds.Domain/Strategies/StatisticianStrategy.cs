using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1: No draw history available.
        // Without empirical data, we fall back to a statistically principled prior:
        //
        // Design rationale:
        // - 6 numbers spread across the 1–49 range in near-equal decile bands
        //   (bands: 1–8, 9–16, 17–24, 25–32, 33–40, 41–49)
        // - Exactly 3 odd / 3 even — consistent with the expected modal split
        //   in a uniform draw (P(3 odd, 3 even) ≈ 0.3292, the single most likely outcome)
        // - No two numbers adjacent (minimises redundant positional clustering)
        // - Sum = 7+14+23+32+37+46 = 159, close to expected mean sum of ~150
        //   (E[sum of 6 from 1–49] = 6 * 25 = 150); slight upward bias acceptable
        //
        // Confidence: 0.12 reflects nothing more than the rough per-number base rate
        // of 6/49 ≈ 0.122; overclaiming would be statistically dishonest.

        var numbers = new List<int> { 7, 14, 23, 32, 37, 46 };

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "uniform-prior-v1",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "No history; applying uniform prior — balanced odd/even, decile spread, near-mean sum."
        };
    }
}
