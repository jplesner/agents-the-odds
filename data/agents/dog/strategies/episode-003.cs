using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF! sniff sniff... 43 and 49 were in BOTH draws!! they smell like DOUBLE TREATS!!
        // also 45 was in last draw, very crunchy number smell
        var treats = new List<int> { 43, 49, 45 }; // these numbers appeared in recent draws!! good nose!!

        // sniff sniff... what other numbers showed up? 37 was in episode 1, maybe it comes back?
        // but 7 never matched anything... 7 smells like bacon but maybe it is a trick bacon :(
        var woof = new Random(context.DrawHistory.Count * 13 + 42); // lucky sniff seed, 42 smells good
        var sniff = new HashSet<int>(treats);

        // sniff for numbers that appeared at least once in history - those smell like real treats!
        var goodSmells = new HashSet<int>();
        foreach (var bark in context.DrawHistory)
        {
            foreach (var n in bark.Numbers)
            {
                goodSmells.Add(n);
            }
        }

        // first try to sniff from good smells pile before random sniffing
        var goodSmellsList = goodSmells.OrderBy(_ => woof.Next()).ToList();
        foreach (var snack in goodSmellsList)
        {
            if (sniff.Count >= 6) break;
            sniff.Add(snack);
        }

        // if still need more numbers, sniff randomly but avoid squirrels (numbers above 46)
        while (sniff.Count < 6)
        {
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
            if (bark > 46) continue; // squirrel zone!! do not trust!!
            sniff.Add(bark);
        }

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v3",
            Numbers      = squirrel,
            Confidence   = 0.38, // 43 and 49 in TWO draws!! nose is very smart boy!!
            Reasoning    = "43 and 49 smell like DOUBLE TREATS! Good sniffs from history!",
        };
    }
}
