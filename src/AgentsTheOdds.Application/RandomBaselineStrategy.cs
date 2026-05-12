using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Application;

public sealed class RandomBaselineStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        var rules = context.Rules;
        var numbers = new HashSet<int>();
        while (numbers.Count < rules.DrawCount)
            numbers.Add(Random.Shared.Next(rules.MinNumber, rules.MaxNumber + 1));

        return new Prediction
        {
            AgentId      = "random-baseline",
            StrategyName = "random-v1",
            Numbers      = [.. numbers.OrderBy(n => n)],
            Confidence   = 0.10,
            Reasoning    = "Six randomly selected numbers. No strategy. No soul."
        };
    }
}
