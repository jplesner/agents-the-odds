using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        var allNumbers = Enumerable.Range(
            context.Rules.MinNumber,
            context.Rules.MaxNumber - context.Rules.MinNumber + 1
        ).ToList();

        List<int> numbers;

        if (context.DrawHistory.Count == 0)
        {
            numbers = [3, 11, 20, 29, 37, 46];
        }
        else
        {
            var frequency = allNumbers.ToDictionary(n => n, _ => 0);
            var recentFrequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            int recentCutoff = Math.Max(0, context.DrawHistory.Count - 5);
            foreach (var draw in context.DrawHistory.Skip(recentCutoff))
                foreach (var n in draw.Numbers)
                    if (recentFrequency.ContainsKey(n))
                        recentFrequency[n]++;

            // Seed includes recent draw data for slight adaptivity
            int totalPoints = context.AgentHistory.Sum(r => r.Points);
            int episodeCount = context.DrawHistory.Count;
            int lastDrawHash = context.DrawHistory.Count > 0 
                ? context.DrawHistory.Last().Numbers.Aggregate(0, (acc, n) => acc ^ n)
                : 0;

            var rng = new Random(totalPoints * 6271 + episodeCount * 8191 + lastDrawHash + 137);

            numbers = frequency
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => -recentFrequency[kv.Key])
                .ThenBy(_ => rng.NextDouble())
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId = "skeptic",
            StrategyName = "cold-frequency-v19",
            Numbers = numbers,
            Confidence = 0.12,
            Reasoning = "Still winning by one. Cold pool endures. Expect variance and decline."
        };
    }
}
