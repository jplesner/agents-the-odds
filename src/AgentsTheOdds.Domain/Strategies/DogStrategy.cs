using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // woof woof! these are my favorite numbers (they smell like treats)
        var treats = new List<int> { 7, 11, 3 }; // 7 smells like bacon, 11 like biscuit, 3 is my age

        // sniff sniff... pick more numbers randomly but avoid suspicious ones
        var woof = new Random(context.DrawHistory.Count + 42); // 42 smells like a good walk
        var sniff = new HashSet<int>(treats);

        while (sniff.Count < 6)
        {
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);

            // squirrel check: numbers above 40 are suspicious (might be squirrels)
            if (bark > 40) continue;

            sniff.Add(bark);
        }

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v1",
            Numbers      = squirrel,
            Confidence   = 0.3, // treat confidence! very excite!
            Reasoning    = "Sniffed 3 treat-numbers, randomly sniffed 3 more. Avoided squirrel numbers!",
        };
    }
}
