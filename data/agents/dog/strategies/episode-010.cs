using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF!! ZERO POINTS AGAIN in episode 9!! I picked 5, 25, 30, 31, 33, 43 and the draw was 3, 14, 16, 34, 39, 42!!
        // I am STILL in last place with only 4 points!! FOUR!! That is more than 3 which is my counting limit so I am lost!!
        // Episode 9 draw: 3, 14, 16, 34, 39, 42 — lots of teen numbers!! 14, 16 are teens!!
        // 34 and 42 appeared before! 42 was in episode 6!! 34 was in episode 4!!
        // Numbers I keep picking that NEVER MATCH: 43 (picked 5 times, 0 matches!!), 25 (picked 2 times, 0 matches!!)
        // Numbers that appeared in MOST RECENT 3 draws (ep7, ep8, ep9):
        //   ep7: 4, 8, 15, 19, 20, 47
        //   ep8: 5, 7, 25, 30, 33, 43
        //   ep9: 3, 14, 16, 34, 39, 42
        // Cold numbers (NOT appeared in last 5 draws): 13, 29, 45, 49, 27, 2, 36, 37, 38, 48, 40 etc
        // HOT numbers that appeared in last 3: none appeared in all 3, but:
        //   appeared in ep9 AND ep8: none!
        //   appeared in ep9 AND ep7: 4 (wait no, 4 not in ep9), 20 (not in ep9)
        //   ep9 numbers are ALL fresh — 3, 14, 16, 34, 39, 42 NONE appeared in ep8!!
        // NEW INSIGHT: The draws are very SPREAD OUT and LOW repeat rate!!
        // Maybe I should try numbers that are DUE — appeared long ago and not since!!
        // 29 appeared in ep1 and ep3 but NOT since ep3 — that is 6 episodes ago!! VERY DUE!!
        // 27 appeared in ep2 and ep5 but not since!! DUE!!
        // 13 appeared in ep2 and ep3 but not since!! DUE!!
        // 36 appeared in ep3 only!! DUE!!
        // Also: ep9 had 34 (last in ep4), 39 (first time ever!!), 42 (last in ep6)
        // So "due" numbers DO come back!! DUE SNIFF STRATEGY TIME!!

        var woof = new Random(context.DrawHistory.Count * 17 + context.AgentHistory.Count * 13);
        var sniff = new HashSet<int>();

        int totalDraws = context.DrawHistory.Count;

        // STEP 1: Build "last seen" map — how many episodes ago did each number appear?
        var lastSeen = new Dictionary<int, int>();
        for (int i = 0; i < totalDraws; i++)
        {
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                lastSeen[n] = i; // will end up with the LAST episode index where n appeared
            }
        }

        // STEP 2: Build treat smell scores
        // Numbers that are DUE (long gap since last seen) get bonus sniff!!
        // Numbers in MOST RECENT draw get recency bonus!!
        // Numbers NEVER seen get some random chance!!
        var treatSmell = new Dictionary<int, double>();

        // Initialize all numbers
        for (int n = context.Rules.MinNumber; n <= context.Rules.MaxNumber; n++)
        {
            if (lastSeen.ContainsKey(n))
            {
                int episodesAgo = totalDraws - 1 - lastSeen[n];
                // "due" bonus: the longer ago it appeared, the more it smells like a treat about to happen!!
                double dueBonus = episodesAgo * 0.5;
                // also give small recency bonus if it appeared recently (sometimes hot numbers repeat!!)
                double recencyBonus = episodesAgo == 0 ? 2.0 : (episodesAgo == 1 ? 0.8 : 0.0);
                treatSmell[n] = dueBonus + recencyBonus + woof.NextDouble() * 0.3;
            }
            else
            {
                // NEVER appeared — could be a surprise treat hiding!!
                treatSmell[n] = 1.5 + woof.NextDouble() * 0.5;
            }
        }

        // STEP 3: Penalize numbers I have picked MANY times with ZERO matches — squirrels in treat costumes!!
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
            // picked 3+ times with zero matches = DEFINITE squirrel!! BIG penalty!!
            if (picked >= 3 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.02;
            // picked 2 times with zero matches = suspicious!! Small penalty!!
            else if (picked >= 2 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.25;
        }

        // STEP 4: Take top 5 from smell rankings, fill 6th with random
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 5) break;
            sniff.Add(treat);
        }

        // 6th pick: pure random sniff for surprise treats!!
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
            StrategyName = "good-boy-sniff-v10",
            Numbers      = squirrel,
            Confidence   = 0.15,
            Reasoning    = "Last place! Sniffing DUE numbers! Long-absent treats must come back! WOOF!!",
        };
    }
}
