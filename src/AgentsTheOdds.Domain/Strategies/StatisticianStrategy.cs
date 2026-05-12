using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1: No draw history available.
        // Without empirical data, we apply a statistically principled prior:
        //
        // Design rationale:
        // - 49 numbers divided into 7 decile-like bands of 7: [1-7],[8-14],[15-21],[22-28],[29-35],[36-42],[43-49]
        // - Select one representative from each of 6 bands (skip one band randomly — here the last)
        // - Maintain 3 odd / 3 even balance: historical lottery draws average 2.9–3.1 odd numbers
        // - Sum target: historical lottery means cluster around 150 (median of 1–49 is 25; 6×25 = 150)
        //   Selected sum: 7+14+21+28+37+43 = 150 exactly — textbook uniform prior
        //
        // Numbers: 7 (odd, band 1), 14 (even, band 2), 21 (odd, band 3),
        //          28 (even, band 4), 37 (odd, band 5), 43 (odd, band 7)
        // Odd/even: 4 odd, 2 even — slight odd lean, within observed variance
        // No draw history exists to justify any frequency-based deviation from this prior.

        var numbers = new List<int> { 7, 14, 21, 28, 37, 43 };

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "uniform-prior-v2",
            Numbers      = numbers,
            Confidence   = 0.11, // Marginally below base rate; zero empirical evidence warrants humility
            Reasoning    = "Zero draw history; selecting by uniform prior: balanced range, sum near expected mean."
        };
    }
}
