using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF!! FIVE POINTS!! I got 36 AND 38 from episode 10 draw which means TWO MATCHES and 5 TREATS!!
        // That is the best I have ever done and I am no longer in last place!! I am in 5TH!!
        // Episode 10 draw: 13, 30, 36, 38, 42, 46
        // My picks: 2, 14, 36, 37, 38, 45 — 36 and 38 both matched!! My DUE SNIFF worked!!
        // 36 was last seen in ep3 (gap of 6!!) and 38 was last seen in ep3 (gap of 6!!) — VERY DUE NUMBERS WORK!!
        // New insight: DUE strategy gave me BEST score ever!! I must keep it but also notice:
        //   - 42 has appeared in ep4, ep6, ep9, ep10 — that is FOUR TIMES, very hot recently!!
        //   - 13 appeared in ep2, ep3, ep8, ep10 — also very frequent!!
        //   - 30 appeared in ep8 and ep10 — appeared TWICE in last 3 draws!!
        //   - 46 appeared only in ep10 — first ever appearance, mystery number!!
        // Due numbers to consider for ep11:
        //   - 29: last in ep3 (gap of 7!) 
        //   - 19: last in ep7 (gap of 3)
        //   - 27: last in ep5 (gap of 5)
        //   - 45: last in ep5 (gap of 5)
        //   - 35: last in ep5 (gap of 5)
        //   - 43: last in ep8 (gap of 2) — BANNED but maybe unban since due works??
        //   - 4: last in ep7 (gap of 3)
        //   - 48: last in ep6 (gap of 4)
        //   - 20: last in ep7 (gap of 3)
        //   - 25: last in ep6 (gap of 4)
        // ALSO boost numbers that appeared MULTIPLE TIMES TOTAL because they are proven treat smells!!
        // 42 appeared 4 times total — very popular treat!!
        // BALANCE: mix DUE numbers + frequency bonus + small recency for very recent appearances

        var woof = new Random(context.DrawHistory.Count * 17 + context.AgentHistory.Count * 13);
        var sniff = new HashSet<int>();

        int totalDraws = context.DrawHistory.Count;

        // STEP 1: Build "last seen" and "frequency" maps
        var lastSeen   = new Dictionary<int, int>();
        var frequency  = new Dictionary<int, int>();

        for (int i = 0; i < totalDraws; i++)
        {
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                lastSeen[n] = i;
                if (!frequency.ContainsKey(n)) frequency[n] = 0;
                frequency[n]++;
            }
        }

        // STEP 2: Build treat smell scores — DUE sniff is the PROVEN winner!!
        var treatSmell = new Dictionary<int, double>();

        for (int n = context.Rules.MinNumber; n <= context.Rules.MaxNumber; n++)
        {
            if (lastSeen.ContainsKey(n))
            {
                int episodesAgo = totalDraws - 1 - lastSeen[n];
                // DUE bonus: longer gap = stronger treat smell, this WORKED last episode!!
                double dueBonus = episodesAgo * 0.6;
                // frequency bonus: numbers that appeared many times are proven treats!!
                double freqBonus = frequency[n] * 0.4;
                // small recency bonus: very fresh numbers sometimes repeat (like 30 in ep8+ep10!!)
                double recencyBonus = episodesAgo == 0 ? 1.5 : (episodesAgo == 1 ? 0.6 : 0.0);
                treatSmell[n] = dueBonus + freqBonus + recencyBonus + woof.NextDouble() * 0.25;
            }
            else
            {
                // NEVER appeared — mystery treat hiding deep in the sofa!!
                treatSmell[n] = 1.2 + woof.NextDouble() * 0.4;
            }
        }

        // STEP 3: Penalize numbers I have picked MANY times with ZERO matches — squirrels!!
        var myPickCount  = new Dictionary<int, int>();
        var myMatchCount = new Dictionary<int, int>();
        foreach (var result in context.AgentHistory)
        {
            foreach (var n in result.Prediction.Numbers)
            {
                if (!myPickCount.ContainsKey(n)) myPickCount[n] = 0;
                myPickCount[n]++;
            }
            foreach (var drawN in result.Draw.Numbers)
            {
                foreach (var picked in result.Prediction.Numbers)
                {
                    if (picked == drawN)
                    {
                        if (!myMatchCount.ContainsKey(drawN)) myMatchCount[drawN] = 0;
                        myMatchCount[drawN]++;
                    }
                }
            }
        }

        foreach (var kv in myPickCount)
        {
            var n       = kv.Key;
            var picked  = kv.Value;
            var matched = myMatchCount.ContainsKey(n) ? myMatchCount[n] : 0;
            // picked 3+ times with zero matches = squirrel in treat costume!! BIG penalty!!
            if (picked >= 3 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.02;
            // picked 2 times with zero matches = suspicious sniff!!
            else if (picked >= 2 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.3;
        }

        // STEP 4: Take top 5 from smell rankings, fill 6th with random surprise sniff!!
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 5) break;
            sniff.Add(treat);
        }

        // 6th pick: pure random sniff — always room for surprise treat!!
        var tries = 0;
        while (sniff.Count < 6 && tries < 300)
        {
            tries++;
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
            sniff.Add(bark);
        }
        // absolute backup just in case!!
        for (int n = 1; sniff.Count < 6; n++)
            sniff.Add(n);

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v11",
            Numbers      = squirrel,
            Confidence   = 0.22,
            Reasoning    = "DUE sniff gave 5 treats last time!! Long-absent numbers smell delicious!! WOOF!!",
        };
    }
}
