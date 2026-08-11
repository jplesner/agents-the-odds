using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // WOOF!! ZERO POINTS in episode 11!! I picked 23, 27, 35, 37, 43, 48 and NONE matched!!
        // Episode 11 draw: 6, 15, 33, 36, 44, 49
        // 43 BETRAYED me AGAIN — that number is a squirrel wearing a treat costume and I am SO MAD!!
        // 37 also keeps failing me — it is 43's evil friend squirrel!!
        // 
        // BUT WAIT: 49 appeared in BOTH ep1 AND ep2 AND ep11 — three times total!!
        //           36 appeared in ep3, ep10, AND ep11 — THREE TIMES and very fresh!!
        //           33 appeared in ep8 AND ep11 — came back after 3 episodes!!
        //           15 appeared in ep7 AND ep11 — came back after 4 episodes!!
        //           6 appeared NEVER BEFORE ep11 — surprise treat from the sofa!!
        //           44 appeared NEVER BEFORE ep11 — another mystery treat!!
        //
        // KEY INSIGHT: My DUE strategy kept picking numbers that haven't appeared but they STILL don't come!!
        // And numbers that appeared VERY RECENTLY (like 33, 36 in ep10+ep11) keep repeating!!
        // So maybe RECENCY is actually MORE important than I thought after my ep10 success??
        // 36 matched in BOTH ep10 and ep11 — very very fresh smell!!
        //
        // NEW PLAN: DUE sniff PLUS strong recency boost for numbers seen in last 2 episodes!!
        // Also DOUBLE BAN on 43 and 37 — they are the two biggest squirrels in the whole park!!
        // And keep frequency bonus because 42 has appeared 4 times total but in ep9+ep10 not ep11...
        //
        // Numbers due (not seen recently) that might bounce back:
        //   - 29: last in ep3 (gap of 8!) — SUPER DUE
        //   - 27: last in ep5 (gap of 6!) — very due  
        //   - 45: last in ep5 (gap of 6!) — very due
        //   - 19: last in ep7 (gap of 4) — pretty due
        //   - 5: last in ep8 (gap of 3) — somewhat due
        //   - 30: last in ep10 (gap of 1) — still fresh
        //   - 42: last in ep10 (gap of 1) — still fresh, 4x total!!
        //   - 33: last in ep11 (gap of 0) — HOT fresh!! appeared in ep8+ep11
        //   - 36: last in ep11 (gap of 0) — HOT fresh!! appeared in ep3+ep10+ep11
        //   - 49: last in ep11 (gap of 0) — HOT fresh!! appeared in ep1+ep2+ep11

        var woof = new Random(context.DrawHistory.Count * 17 + context.AgentHistory.Count * 13 + 42);
        var sniff = new HashSet<int>();

        int totalDraws = context.DrawHistory.Count;

        // STEP 1: Build "last seen" and "frequency" maps — nose science!!
        var lastSeen  = new Dictionary<int, int>();
        var frequency = new Dictionary<int, int>();

        for (int i = 0; i < totalDraws; i++)
        {
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                lastSeen[n] = i;
                if (!frequency.ContainsKey(n)) frequency[n] = 0;
                frequency[n]++;
            }
        }

        // STEP 2: Build treat smell scores
        var treatSmell = new Dictionary<int, double>();

        for (int n = context.Rules.MinNumber; n <= context.Rules.MaxNumber; n++)
        {
            if (lastSeen.ContainsKey(n))
            {
                int episodesAgo = totalDraws - 1 - lastSeen[n];

                // DUE bonus: numbers hiding a long time smell like treats about to come back!!
                double dueBonus = episodesAgo * 0.5;

                // RECENCY bonus: STRONG!! numbers seen very recently sometimes keep coming back!!
                // ep10+ep11 showed that 33, 36, 49 all repeated — FRESH SMELLS ARE REAL!!
                double recencyBonus = 0.0;
                if (episodesAgo == 0) recencyBonus = 2.5;       // JUST appeared!! very hot smell!!
                else if (episodesAgo == 1) recencyBonus = 1.2;  // appeared 1 draw ago — still warm!!
                else if (episodesAgo == 2) recencyBonus = 0.5;  // fading but still sniffable!!

                // Frequency bonus: numbers that appeared many times are proven treat smells!!
                double freqBonus = frequency[n] * 0.35;

                treatSmell[n] = dueBonus + recencyBonus + freqBonus + woof.NextDouble() * 0.2;
            }
            else
            {
                // NEVER appeared — mystery treat deep in the sofa!! medium interest
                treatSmell[n] = 0.8 + woof.NextDouble() * 0.4;
            }
        }

        // STEP 3: Penalize my personal squirrels — numbers I picked MANY times with ZERO matches!!
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
            // picked 3+ times with zero matches = SQUIRREL!! MAXIMUM penalty!!
            if (picked >= 3 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.01;  // basically zero, go away squirrel!!
            // picked 2 times with zero matches = suspicious squirrel smell!!
            else if (picked >= 2 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.2;
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

        // 6th pick: pure random sniff — always room for a surprise treat!!
        var tries = 0;
        while (sniff.Count < 6 && tries < 300)
        {
            tries++;
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
            sniff.Add(bark);
        }
        // absolute backup just in case the nose fails completely!!
        for (int n = 1; sniff.Count < 6; n++)
            sniff.Add(n);

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v12",
            Numbers      = squirrel,
            Confidence   = 0.20,
            Reasoning    = "Fresh smells AND due smells!! 43 and 37 are squirrels FOREVER!! WOOF!!",
        };
    }
}
