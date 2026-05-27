using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // woof! last time 37 was in the draw AND in my picks! good sniff! keep it!
        var treats = new List<int> { 7, 37, 43 }; // 37 smells like a WINNER, 43 was in the draw too, 7 always smells like bacon

        // sniff sniff... last draw had BIG numbers (40, 43, 49) so maybe I should sniff up there too
        // but squirrels... but TREATS up high...
        var woof = new Random(context.DrawHistory.Count * 7 + 13); // lucky sniff seed
        var sniff = new HashSet<int>(treats);

        while (sniff.Count < 6)
        {
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);

            // revised squirrel check: numbers above 45 still very suspicious (probably squirrels)
            // but 40-45 might just be big treats so allow them now!
            if (bark > 45) continue;

            sniff.Add(bark);
        }

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v2",
            Numbers      = squirrel,
            Confidence   = 0.35, // more confidents! I sniffed a winner last time!
            Reasoning    = "37 smelled right last time! Sniffing big numbers now, avoiding squirrels!",
        };
    }
}
