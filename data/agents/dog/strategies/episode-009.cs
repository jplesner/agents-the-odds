using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF!! ZERO POINTS AGAIN in episode 8!! I picked 4, 13, 15, 16, 19, 20 and the draw was 5, 7, 25, 30, 33, 43!!
        // Pattern Goblin got 5 points!! GOBLIN!! With 33 and 43 which are medium-BIG numbers!! NOT small!!
        // I keep switching strategies and getting zero!! Maybe I need to sniff EVERYTHING equally!!
        // Episode 8 draw had 5, 7, 25, 30, 33, 43 — lots of medium-big!! My small-bias was WRONG again!!
        // Numbers that appeared in LAST 3 draws: 5(ep8), 7(ep8), 25(ep6,ep8), 30(ep8), 33(ep8), 43(ep2,ep5,ep8)
        // 43 keeps coming back!! I BANNED it but it appeared AGAIN in ep8!! Maybe I was wrong to ban it??
        // 25 appeared in ep6 AND ep8 — that is DOUBLE FRESH smell!!
        // 5 appeared in ep1 AND ep8 — it has been hiding for a long time then came back!!
        // 7 appeared in ep8 for first time — brand new fresh smell!!
        // I am in LAST PLACE with 4 points!! LAST PLACE!! No treats for last place dogs!!
        // NEW STRATEGY: Stop being biased!! Sniff the WHOLE range!! Trust recency MOST!!
        // Also: Pattern Goblin picked 33 and 43 — maybe sniff around the 30-45 range too!!

        var woof = new Random(context.DrawHistory.Count * 43 + context.AgentHistory.Count * 7);
        var sniff = new HashSet<int>();

        // build treat smell scores from FULL draw history with heavy recency weighting
        var treatSmell = new Dictionary<int, double>();
        int totalDraws = context.DrawHistory.Count;

        for (int i = 0; i < totalDraws; i++)
        {
            // recency weight: most recent = STRONGEST smell!! Old smells fade fast!!
            var recencyWeight = Math.Pow((double)(i + 1) / totalDraws, 2.0); // quadratic boost for recent!!
            var freshBonus = (i == totalDraws - 1) ? 4.0 : 0.0;      // LAST draw = hottest treat smell!!
            var secondFresh = (i == totalDraws - 2) ? 2.0 : 0.0;     // second-last = warm treat smell!!
            var thirdFresh  = (i == totalDraws - 3) ? 0.8 : 0.0;     // third-last = slightly warm!!

            foreach (var n in context.DrawHistory[i].Numbers)
            {
                if (!treatSmell.ContainsKey(n)) treatSmell[n] = 0.0;
                treatSmell[n] += recencyWeight + freshBonus + secondFresh + thirdFresh;
            }
        }

        // penalize numbers I have picked MANY times with ZERO matches — those are traitor squirrels!!
        var myPickCount  = new Dictionary<int, int>();
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

        foreach (var kv in myPickCount)
        {
            var n       = kv.Key;
            var picked  = kv.Value;
            var matched = myMatchCount.ContainsKey(n) ? myMatchCount[n] : 0;
            // picked 3+ times with zero matches = definite squirrel in treat costume!!
            if (picked >= 3 && matched == 0 && treatSmell.ContainsKey(n))
            {
                treatSmell[n] *= 0.05;
            }
            // picked 2 times with zero matches = suspicious smell, small penalty
            else if (picked >= 2 && matched == 0 && treatSmell.ContainsKey(n))
            {
                treatSmell[n] *= 0.3;
            }
        }

        // add small random noise so I do not always pick EXACTLY same numbers!!
        foreach (var key in treatSmell.Keys.ToList())
        {
            treatSmell[key] += woof.NextDouble() * 0.2;
        }

        // also add some fresh smell to numbers NOT in history — surprise treats!!
        // sprinkle a few unexplored numbers into the mix with low but nonzero score
        for (int n = context.Rules.MinNumber; n <= context.Rules.MaxNumber; n++)
        {
            if (!treatSmell.ContainsKey(n))
            {
                // unexplored number — give it a small random sniff chance!!
                treatSmell[n] = woof.NextDouble() * 0.3;
            }
        }

        // take top 5 from smell rankings
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => woof.Next())
            .Select(kv => kv.Key)
            .ToList();

        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 5) break;
            sniff.Add(treat);
        }

        // fill last spot with pure random sniff — nose says try something unexpected!!
        var tries = 0;
        while (sniff.Count < 6 && tries < 300)
        {
            tries++;
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
            sniff.Add(bark);
        }
        // absolute backup
        for (int n = 1; sniff.Count < 6; n++)
            sniff.Add(n);

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v9",
            Numbers      = squirrel,
            Confidence   = 0.18, // last place means treats are hiding EVERYWHERE!! Must sniff better!!
            Reasoning    = "Last place! No bias! Recency sniff whole range! Traitors penalized! Find treats!!",
        };
    }
}
