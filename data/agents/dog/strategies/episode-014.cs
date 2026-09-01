using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class DogStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        var woof = new Random(context.DrawHistory.Count * 17 + context.AgentHistory.Count * 13 + 42);
        var sniff = new HashSet<int>();

        int totalDraws = context.DrawHistory.Count;

        // Build "last seen" and "frequency" maps
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

        // Build treat smell scores with STRONG recency boost and frequency
        var treatSmell = new Dictionary<int, double>();

        for (int n = context.Rules.MinNumber; n <= context.Rules.MaxNumber; n++)
        {
            if (lastSeen.ContainsKey(n))
            {
                int episodesAgo = totalDraws - 1 - lastSeen[n];

                // Strong boost for recent hits (last 3 episodes)
                double recencyBonus = 0.0;
                if (episodesAgo == 0) recencyBonus = 3.0;
                else if (episodesAgo == 1) recencyBonus = 2.0;
                else if (episodesAgo == 2) recencyBonus = 1.2;
                else if (episodesAgo <= 4) recencyBonus = 0.5;

                double freqBonus = frequency[n] * 0.5;

                treatSmell[n] = recencyBonus + freqBonus + woof.NextDouble() * 0.15;
            }
            else
            {
                treatSmell[n] = 0.4 + woof.NextDouble() * 0.25;
            }
        }

        // Penalize personal squirrels — numbers picked many times with ZERO matches
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
            if (picked >= 3 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.01;
            else if (picked >= 2 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.2;
        }

        // Take top 5 from smell rankings, fill 6th with random sniff
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 5) break;
            sniff.Add(treat);
        }

        // 6th pick: pure random sniff
        var tries = 0;
        while (sniff.Count < 6 && tries < 300)
        {
            tries++;
            var bark = woof.Next(context.Rules.MinNumber, context.Rules.MaxNumber + 1);
            sniff.Add(bark);
        }

        for (int n = 1; sniff.Count < 6; n++)
            sniff.Add(n);

        var squirrel = sniff.OrderBy(n => n).ToList();

        return new()
        {
            AgentId      = "dog",
            StrategyName = "good-boy-sniff-v14",
            Numbers      = squirrel,
            Confidence   = 0.24,
            Reasoning    = "Recent hot treats GLOW bright! Last 3 draws packed with treats! WOOF WOOF!",
        };
    }
}
