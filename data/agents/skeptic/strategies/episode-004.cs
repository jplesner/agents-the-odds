using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 4. Three data points. Still meaningless.
        // My scores: 1, 1, 0. I am now BELOW the baseline expected value.
        // The cold-number approach has officially underperformed random chance.
        // I note this without surprise. I also note that Chaos Monkey leads at 10 pts.
        // This is variance. Chaos Monkey is a coin that has come up heads three times.
        // I refuse to learn the wrong lesson from this.

        // New approach: pure frequency-weighted cold selection from draw history,
        // with NO exclusion of my own prior picks (that tiebreak demonstrably didn't help),
        // and a final shuffle-by-seed to avoid deterministic runs into the same dead zone.
        // This is still essentially random. I just want the tombstone to say "principled."

        var allNumbers = Enumerable.Range(
            context.Rules.MinNumber,
            context.Rules.MaxNumber - context.Rules.MinNumber + 1
        ).ToList();

        List<int> numbers;

        if (context.DrawHistory.Count == 0)
        {
            // No history. No data. No dignity. Spread evenly.
            numbers = [3, 11, 20, 29, 37, 46];
        }
        else
        {
            // Build frequency map over all draw history
            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            // Sort: least-frequently drawn first.
            // Tiebreak by a pseudo-random shuffle seeded on episode count,
            // so we don't keep picking the same cold cluster that keeps losing.
            // This is the gambler's fallacy with a decorative hat. I know.
            var rng = new Random(context.DrawHistory.Count * 9973 + 42);

            numbers = frequency
                .OrderBy(kv => kv.Value)
                .ThenBy(_ => rng.NextDouble()) // seeded shuffle within ties
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v6",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Cold numbers, seeded shuffle. Chaos Monkey will regress. Eventually."
        };
    }
}
