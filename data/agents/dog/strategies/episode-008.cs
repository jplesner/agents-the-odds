using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF!! I got 1 point again, matching 19 in episode 7 draw!! My nose found it!!
        // Episode 7 draw was [4, 8, 15, 19, 20, 47] - lots of SMALL numbers!! Very sneaky!!
        // The Skeptic got 10 points with 3 matches and is in FIRST PLACE - I must sniff better!!
        // I am tied for 5th with Pattern Goblin and that is NOT podium!! No treats!!
        // The Skeptic picked SMALL numbers: 3, 8, 15, 24, 44, 47 - small numbers WORKING!!
        // 4 appeared TWICE in history (episodes 4 AND 7) - that is fresh repeated smell!!
        // 20 appeared THREE TIMES (episodes 4, 5, 7) - 20 is THE MOST REPEATED recently!!
        // 19 appeared THREE TIMES too (episodes 3, 4, 7) - very popular smell but I already matched it once!!
        // 47 is BRAND NEW from episode 7 - never appeared before!! Very fresh new smell!!
        // 8 is BRAND NEW too!! New smells = exciting sniffs!!
        // LESSON: small numbers (1-20) have been winning lately!! Skeptic PROVED this!!
        // NEW PLAN: bias toward small-medium range (4-25) but still sniff historically frequent numbers!!

        var woof = new Random(context.DrawHistory.Count * 47 + 8); // 47 and 8 are fresh winners from ep7!!
        var sniff = new HashSet<int>();

        // build treat smell scores from draw history with recency weighting
        var treatSmell = new Dictionary<int, double>();
        for (int i = 0; i < context.DrawHistory.Count; i++)
        {
            // recency weight: most recent draws smell STRONGEST!!
            var recencyWeight = (double)(i + 1) / context.DrawHistory.Count;
            var freshBonus = (i == context.DrawHistory.Count - 1) ? 3.0 : 0.0;     // last draw = very fresh!!
            var secondFreshBonus = (i == context.DrawHistory.Count - 2) ? 1.5 : 0.0; // second-last also good!!
            var thirdFreshBonus = (i == context.DrawHistory.Count - 3) ? 0.5 : 0.0;  // third-last small bonus

            foreach (var n in context.DrawHistory[i].Numbers)
            {
                if (!treatSmell.ContainsKey(n)) treatSmell[n] = 0.0;
                treatSmell[n] += recencyWeight + freshBonus + secondFreshBonus + thirdFreshBonus;

                // SMALL NUMBER BONUS!! Skeptic showed small numbers = treats!!
                // numbers 1-25 get a bonus sniff because recent draws favor them!!
                if (n <= 25)
                {
                    treatSmell[n] += 0.8;
                }
            }
        }

        // track my pick history to penalize TRAITOR numbers I keep picking with zero matches
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

        // PENALIZE traitor numbers: picked many times but never matched = squirrel in treat costume!!
        foreach (var kv in myPickCount)
        {
            var n = kv.Key;
            var picked = kv.Value;
            var matched = myMatchCount.ContainsKey(n) ? myMatchCount[n] : 0;
            if (picked >= 2 && matched == 0 && treatSmell.ContainsKey(n))
            {
                treatSmell[n] *= 0.08; // TRAITOR!! smells like squirrel!! very penalty!!
            }
        }

        // sniff the best smelling numbers from history
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .ThenBy(_ => woof.Next())
            .Select(kv => kv.Key)
            .ToList();

        // take top 4 from smell chart (history-proven treats!!)
        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 4) break;
            sniff.Add(treat);
        }

        // fill remaining 2 with small-medium random (Skeptic showed 1-25 is treat territory!!)
        var tries = 0;
        while (sniff.Count < 6 && tries < 200)
        {
            tries++;
            var bark = woof.Next(1, 26); // small number zone!! Skeptic nose knows!!
            sniff.Add(bark);
        }
        // backup: any number if small range didn't work
        while (sniff.Count < 6)
        {
            sniff.Add(woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1));
        }

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v8",
            Numbers      = squirrel,
            Confidence   = 0.25, // Skeptic proved small numbers = treats!! My nose is learning!!
            Reasoning    = "Skeptic won big with small numbers! Fresh episode 7 smells! Traitors penalized!!",
        };
    }
}
