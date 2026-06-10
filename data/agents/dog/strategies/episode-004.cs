using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF! Episode 3 was ZERO POINTS! My nose was broken!! 43 and 49 did NOT come back!
        // New plan: sniff for numbers that appeared RECENTLY (not just ever!) because fresh smells are best!!
        // Also 13 appeared TWICE (episodes 2 and 3) so 13 smells like a DOUBLE TREAT now!!
        // And 29 appeared twice too (episodes 1 and 3)!! 29 smells like a biscuit hidden under a cushion!!

        var woof = new Random(context.DrawHistory.Count * 17 + 7); // sniff seed, adjusted for sad zero episode
        var sniff = new HashSet<int>();

        // count how many times each number appeared - more appearances = more treat smell!!
        var treatCounts = new Dictionary<int, int>();
        foreach (var bark in context.DrawHistory)
        {
            foreach (var n in bark.Numbers)
            {
                if (!treatCounts.ContainsKey(n)) treatCounts[n] = 0;
                treatCounts[n]++;
            }
        }

        // MOST IMPORTANT: numbers that appeared MORE THAN ONCE smell EXTRA DELICIOUS
        // 13 appeared twice!! 29 appeared twice!! 43 appeared twice!! 49 appeared twice!!
        var doubleSniffs = treatCounts
            .Where(kv => kv.Value >= 2)
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => woof.Next())
            .Select(kv => kv.Key)
            .ToList();

        foreach (var treat in doubleSniffs)
        {
            if (sniff.Count >= 3) break; // only keep 3 double-treat numbers, cannot count higher anyway
            sniff.Add(treat);
        }

        // then sniff from most recent draw - fresh smells are good smells!!
        if (context.DrawHistory.Count > 0)
        {
            var freshDraw = context.DrawHistory[context.DrawHistory.Count - 1];
            var freshSmells = freshDraw.Numbers.OrderBy(_ => woof.Next()).ToList();
            foreach (var freshTreat in freshSmells)
            {
                if (sniff.Count >= 5) break;
                sniff.Add(freshTreat);
            }
        }

        // still need more? sniff randomly from the middle range - not too small, not squirrel-big
        while (sniff.Count < 6)
        {
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
            if (bark > 45) continue; // squirrels!! stay away!!
            sniff.Add(bark);
        }

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v4",
            Numbers      = squirrel,
            Confidence   = 0.30, // zero points was very sad, nose is less confident but still trying!!
            Reasoning    = "13 and 29 appeared TWICE! Fresh sniffs from recent draw! No squirrels!",
        };
    }
}
