using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF! Episode 5: ZERO POINTS AGAIN!! My nose is lying to me or I am bad boy??
        // 19 smelled so fresh but it did NOT come back!! Betrayal!! 37 also betrayed me AGAIN!!
        // NEW SNIFF STRATEGY: the draw had 20, 23, 27, 35, 43, 45 and I had NONE of them!!
        // 20 appeared in episodes 4 AND 5 - that is the FRESHEST double smell right now!!
        // 27 appeared in episodes 2 AND 5 - also very fresh!!
        // 43 appeared in episodes 1, 2, AND 5 - THREE TIMES!! That is the most treats of any number!!
        // 45 appeared in episodes 2 AND 5 - fresh AND recent!!
        // Plan: reward numbers that appeared in LAST EPISODE extra lots because they smell BRAND NEW!!
        // Also: maybe big numbers are NOT squirrels - 43 and 45 keep showing up and those are big!!

        var woof = new Random(context.DrawHistory.Count * 17 + 43); // 43 is lucky now, it appeared THREE times!!
        var sniff = new HashSet<int>();

        // build treat smell map with STRONG recency bias - last episode smells are freshest!!
        var treatSmell = new Dictionary<int, double>();
        for (int i = 0; i < context.DrawHistory.Count; i++)
        {
            // recency weight: last episode = max sniff power, older = weaker smell
            var recencyWeight = (double)(i + 1) / context.DrawHistory.Count;
            var freshBonus = (i == context.DrawHistory.Count - 1) ? 3.0 : 0.0; // LAST EPISODE = SUPER FRESH!!
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                if (!treatSmell.ContainsKey(n)) treatSmell[n] = 0.0;
                treatSmell[n] += recencyWeight + freshBonus;
            }
        }

        // sniff the TOP smelling numbers - most delicious treats go first!!
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => woof.Next())
            .Select(kv => kv.Key)
            .ToList();

        // take top 4 from smell chart (woof! that is more than 3, very big thinking for dog!!)
        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 4) break;
            sniff.Add(treat);
        }

        // sniff 2 random numbers from ANYWHERE including big numbers - 43 proved big = treats!!
        // but still avoid numbers above 48 because 49 has never come back after episode 1-2 and smells stale
        while (sniff.Count < 6)
        {
            var bark = woof.Next(context.Rules.MinNumber, 49); // max 48, 49 smells like old biscuit!!
            sniff.Add(bark);
        }

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v6",
            Numbers      = squirrel,
            Confidence   = 0.28, // nose confused but still trying!! zero points makes me humble dog!!
            Reasoning    = "43 appeared THREE times! Last draw smell freshest! No squirrels today!!",
        };
    }
}
