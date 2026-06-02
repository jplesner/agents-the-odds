using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class SkepticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 3. Two data points. Still statistically meaningless.
        // I scored 1 point in both episodes. Chaos Monkey scored 5 then 0.
        // Variance. Pure variance. The Monkey is regressing to the mean.
        // I find no comfort in being right about this.

        var allNumbers = Enumerable.Range(
            context.Rules.MinNumber,
            context.Rules.MaxNumber - context.Rules.MinNumber + 1
        ).ToList();

        List<int> numbers;

        if (context.DrawHistory.Count == 0)
        {
            // No history. No data. No dignity.
            numbers = [3, 11, 20, 29, 37, 46];
        }
        else
        {
            // The cold-number strategy continues. Two episodes of data.
            // Numbers drawn so far: 2, 5, 13, 27, 29, 37, 40, 43, 45, 49
            // That leaves 39 numbers with frequency 0 and some with frequency 1 or 2.
            // 43 and 49 were drawn in BOTH episodes. That's "hot". I'm going cold.
            // This is the gambler's fallacy. I remain fully, depressingly aware.

            var frequency = allNumbers.ToDictionary(n => n, _ => 0);

            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    if (frequency.ContainsKey(n))
                        frequency[n]++;

            // Also avoid numbers we've already picked that keep losing for us.
            // Numbers 1, 2, 3, 4, 6, 7 have appeared in my picks. 
            // 2 matched once (lucky for me, useless going forward presumably).
            // I'll add a tiebreak: prefer numbers NOT in my own prior picks,
            // because at least that makes the selection less embarrassingly predictable.
            var myPriorPicks = context.AgentHistory
                .SelectMany(r => r.Prediction.Numbers)
                .ToHashSet();

            numbers = frequency
                .OrderBy(kv => kv.Value)
                .ThenByDescending(kv => myPriorPicks.Contains(kv.Key) ? 0 : 1) // prefer NOT previously picked
                .ThenBy(kv => kv.Key) // deterministic final tiebreak
                .Take(context.Rules.DrawCount)
                .Select(kv => kv.Key)
                .ToList();
        }

        return new Prediction
        {
            AgentId      = "skeptic",
            StrategyName = "cold-frequency-v5",
            Numbers      = numbers,
            Confidence   = 0.12,
            Reasoning    = "Cold numbers, avoiding prior picks. Chaos Monkey is regressing. Predictably."
        };
    }
}
