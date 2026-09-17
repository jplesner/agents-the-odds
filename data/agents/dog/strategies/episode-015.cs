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

        // Build frequency and last-seen maps
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

        // Compute treat smell: hot recent numbers get max boost
        var treatSmell = new Dictionary<int, double>();

        for (int n = context.Rules.MinNumber; n <= context.Rules.MaxNumber; n++)
        {
            if (lastSeen.ContainsKey(n))
            {
                int episodesAgo = totalDraws - 1 - lastSeen[n];

                // Extreme recency boost: last 2 episodes GLOW
                double recencyBonus = 0.0;
                if (episodesAgo == 0) recencyBonus = 4.0;
                else if (episodesAgo == 1) recencyBonus = 3.0;
                else if (episodesAgo == 2) recencyBonus = 1.5;
                else if (episodesAgo <= 5) recencyBonus = 0.3;

                double freqBonus = frequency[n] * 0.4;

                treatSmell[n] = recencyBonus + freqBonus + woof.NextDouble() * 0.1;
            }
            else
            {
                treatSmell[n] = 0.3 + woof.NextDouble() * 0.2;
            }
        }

        // Penalize squirrels harder — picked 2+ times with zero matches
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
            
            if (picked >= 2 && matched == 0 && treatSmell.ContainsKey(n))
                treatSmell[n] *= 0.05;
        }

        // Top 5 from treat smell, 6th from random sniff
        var bestSniffs = treatSmell
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var treat in bestSniffs)
        {
            if (sniff.Count >= 5) break;
            sniff.Add(treat);
        }

        // 6th pick: random
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
            StrategyName = "good-boy-sniff-v15",
            Numbers      = squirrel,
            Confidence   = 0.26,
            Reasoning    = "WOOF! Last two draws GOLDEN! 36, 42, 29, 4 all FRESH! GOOD BOY!",
        };
    }
}
