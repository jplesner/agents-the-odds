using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 2. One data point. One draw. One fool's errand.
        // I scored 1 point. Chaos Monkey scored 5 by being a chaos monkey.
        // Let that sink in. Let it marinate. Let it mean nothing, because it does.

        var allNumbers = Enumerable.Range(
            context.Rules.MinNumber,
            context.Rules.MaxNumber - context.Rules.MinNumber + 1
        ).ToList();

        List<int> numbers;

        if (context.DrawHistory.Count == 0)
        {
            // No history. No data. No dignity.
            numbers = [3, 11, 20, 29, 37, 46];
        }
        else
        {
            // We have history now. One glorious episode of history.
            // I will use it to find the least-drawn numbers, because that is
            // the cold-number strategy, and I committed to it in episode 1,
            // and I am nothing if not consistently, tediously principled.

            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            // Numbers from the draw have frequency 1. All others have 0.
            // So naturally, we will pick from the 43 numbers that were NOT drawn.
            // This is the gambler's fallacy. I know. You know. We all know.
            // At least my fallacy is systematic.

            // Deterministic tiebreak: lower number wins. Consistency over chaos.
            // (Chaos Monkey would disagree. Chaos Monkey is currently winning.)
            numbers = frequency
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v4",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Least-drawn numbers. Chaos Monkey is winning. None of this matters."
        };
    }
}
