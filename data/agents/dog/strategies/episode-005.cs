using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF! Episode 4: I got 1 point from 19 which was in the draw!! My nose found it!!
        // New sniff plan: 19 appeared in BOTH episode 3 AND episode 4 - that is DOUBLE FRESH!!
        // 37 appeared in episodes 1 AND 4 - it keeps coming back like a very persistent treat smell!!
        // 13 appeared in episodes 2 AND 3 - still smells like biscuit, maybe it comes back??
        // Strategy: sniff frequency map + recency bonus (recent = fresher smell = better treat!!)

        var woof = new Random(context.DrawHistory.Count * 31 + 13); // sniff seed, 13 is lucky biscuit number
        var sniff = new HashSet<int>();

        // build treat smell map: more appearances = more delicious, BUT recent = extra bonus sniff points!!
        var treatSmell = new Dictionary<int, double>();
        for (int i = 0; i < context.DrawHistory.Count; i++)
        {
            var recencyBonus = (i + 1.0) / context.DrawHistory.Count; // newer episodes smell fresher!!
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                if (!treatSmell.ContainsKey(n)) treatSmell[n] = 0.0;
                treatSmell[n] += 1.0 + recencyBonus; // frequency + freshness = SUPER SMELL
            }
        }

        // sniff the TOP smelling numbers - these are the most treat-worthy!!
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => woof.Next()) // random tiebreaker, cannot count above 3 anyway
            .Select(kv => kv.Key)
            .ToList();

        // take top 4 from the smell chart (cannot count higher than 3 but I try very hard)
        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 4) break;
            sniff.Add(treat);
        }

        // sniff 2 random numbers from middle range - not squirrel territory (above 45 = squirrels!!)
        while (sniff.Count < 6)
        {
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
            if (bark > 44) continue; // squirrels!! SQUIRRELS!! stay away from squirrel numbers!!
            sniff.Add(bark);
        }

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v5",
            Numbers      = squirrel,
            Confidence   = 0.33, // 19 matched!! nose is recovering!! still humble though!!
            Reasoning    = "19 smells DOUBLE FRESH! 37 keeps coming back! Best treat sniffs only!",
        };
    }
}
