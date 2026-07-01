using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF!! ZERO POINTS AGAIN!! THREE ZEROS IN A ROW!! I am very sad dog!!
        // Episode 6 draw was [17, 25, 31, 32, 42, 48] and I had NONE of them!!
        // 43 and 45 keep betraying me so I am putting them in TIME-OUT!!
        // The Mystic got 5 points by picking 32 and 48 - those are MEDIUM numbers I never sniff!!
        // 42 appeared in BOTH episode 4 AND episode 6 - that is fresh AND repeated smell!!
        // 48 appeared in episode 3 AND episode 6 - also a DOUBLE FRESH smell!!
        // 31 appeared in episode 6 which is BRAND NEW smell!! Very excite!!
        // NEW RULE: avoid numbers I have picked 3+ times that NEVER matched - 43 is BANNED (TRAITOR!!)
        // NEW RULE: sniff more MEDIUM numbers (20-35 range) because those have been winning lately!!
        // I am in 5th place which is NOT podium and NOT treats so I must try different sniff!!

        var woof = new Random(context.DrawHistory.Count * 31 + 17); // 17 was in last draw! lucky seed!!
        var sniff = new HashSet<int>();

        // count how many times each number appeared in history
        var treatSmell = new Dictionary<int, double>();
        for (int i = 0; i < context.DrawHistory.Count; i++)
        {
            // recency weight: most recent episodes smell strongest!!
            var recencyWeight = (double)(i + 1) / context.DrawHistory.Count;
            var freshBonus = (i == context.DrawHistory.Count - 1) ? 2.5 : 0.0; // last episode very fresh!!
            var secondFreshBonus = (i == context.DrawHistory.Count - 2) ? 1.0 : 0.0; // second-last also good smell!!
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                if (!treatSmell.ContainsKey(n)) treatSmell[n] = 0.0;
                treatSmell[n] += recencyWeight + freshBonus + secondFreshBonus;
            }
        }

        // COUNT how many times I picked each number - numbers I picked a lot but never matched are TRAITORS!!
        var myPickCount = new Dictionary<int, int>();
        var myMatchCount = new Dictionary<int, int>();
        foreach (var result in context.AgentHistory)
        {
            foreach (var n in result.Prediction.Numbers)
            {
                if (!myPickCount.ContainsKey(n)) myPickCount[n] = 0;
                myPickCount[n]++;
            }
            foreach (var n in result.Draw.Numbers)
            {
                // track if MY picked numbers ever matched
                foreach (var picked in result.Prediction.Numbers)
                {
                    if (picked == n)
                    {
                        if (!myMatchCount.ContainsKey(n)) myMatchCount[n] = 0;
                        myMatchCount[n]++;
                    }
                }
            }
        }

        // PENALIZE traitor numbers: picked many times but zero matches = they smell like FAKE TREATS!!
        foreach (var kv in myPickCount)
        {
            var n = kv.Key;
            var picked = kv.Value;
            var matched = myMatchCount.ContainsKey(n) ? myMatchCount[n] : 0;
            if (picked >= 2 && matched == 0 && treatSmell.ContainsKey(n))
            {
                treatSmell[n] *= 0.1; // very stale smell!! probably squirrel in disguise!!
            }
        }

        // sniff the best smelling numbers!!
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => woof.Next())
            .Select(kv => kv.Key)
            .ToList();

        // take top 4 from smell chart
        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 4) break;
            sniff.Add(treat);
        }

        // fill remaining 2 with medium-range random numbers (17-40 range smells good lately!!)
        var bark = 0;
        var tries = 0;
        while (sniff.Count < 6 && tries < 100)
        {
            tries++;
            bark = woof.Next(17, 41); // medium numbers!! Mystic found treats here!!
            sniff.Add(bark);
        }
        // backup: any number if medium range didn't work
        while (sniff.Count < 6)
        {
            sniff.Add(woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
        }

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v7",
            Numbers      = squirrel,
            Confidence   = 0.22, // three zeros make humble dog but nose keeps sniffing!!
            Reasoning    = "Traitor numbers BANNED! Fresh medium smells! Mystic proved medium = treats!!",
        };
    }
}
