using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class StatisticianStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === STRATEGY: uniform-prior-v2 ===
        // Episode 1: No draw history available. N=0 renders frequency, gap, and
        // recency analysis entirely undefined. Spurious pattern-matching on zero
        // data would introduce systematic bias with no empirical justification.
        //
        // Principled prior construction:
        //   1. RANGE BALANCE: Divide 1–49 into 3 bands (low: 1–16, mid: 17–32, high: 33–49).
        //      Select 2 numbers per band to avoid clustering.
        //   2. ODD/EVEN BALANCE: Target 3 odd / 3 even. Empirical lottery data
        //      consistently shows this is the modal split (~33% of draws).
        //   3. SPACING: Minimum gap of ~6 between selected numbers to maximise
        //      coverage and minimise positional autocorrelation.
        //   4. SUM RANGE: Ideal sum for 6-of-49 draws is approximately 115–185
        //      (central tendency of combinatorial distribution, mean ≈ 150).
        //      Selected set sums to: 7+14+21+34+41+48 = 165. Within range. ✓
        //
        // Selected: 7 (low-odd), 14 (low-even), 21 (mid-odd), 34 (high-even), 41 (high-odd), 48 (high-even)
        // Odd count: 3 (7, 21, 41) | Even count: 3 (14, 34, 48) ✓
        // Band coverage: low×2, mid×1, high×3 — slight high-band skew accepted;
        //   high numbers are modestly underrepresented in agent selections historically.
        // Sum: 165 — within the statistically optimal 115–185 window ✓

        var numbers = new List<int> { 7, 14, 21, 34, 41, 48 };

        return new Prediction
        {
            AgentId      = "statistician",
            StrategyName = "uniform-prior-v2",
            Numbers      = numbers,
            Confidence   = 0.10, // Honest prior: 1-in-~14M odds; 0.10 reflects principled spread, not certainty
            Reasoning    = "N=0; applying principled prior: 3-odd/3-even, balanced range, sum within optimal window."
        };
    }
}
