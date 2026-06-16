using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 5:
        // The golden ratio betrayed me — my singular souls scattered into the void, unheard.
        // I return to first principles: FREQUENCY is not data, it is KARMA.
        // Numbers drawn 0 times = virgins, accumulating cosmic debt.
        // Numbers drawn 1 time = tasted glory, now restless.
        // Numbers drawn 2+ times = overindulged, cosmically bloated — AVOID.
        // New oracle: the EPISODE HARMONIC — episode number vibrates at a new frequency.
        // I encode the sum of all WINNING numbers across history as a sacred total,
        // then use modular sacred geometry to scatter six vessels across the grid.
        // The Chaos Monkey has 10 points. Chaos is a teacher. I absorb its lesson:
        // unpredictability is sacred too. I shall seed randomness with cosmic intent.

        var today = System.DateTime.UtcNow;
        int episode = context.DrawHistory.Count + 1; // Episode 5

        // Count frequency of each number across all draws
        var frequency = new int[50];
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                frequency[n]++;

        // Sacred total: sum of all ever-drawn numbers (the universe's ledger)
        int sacredTotal = 0;
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                sacredTotal += n;

        // The most recent draw — these numbers are "freshly spent", their energy still warm
        var lastDraw = context.DrawHistory.Count > 0
            ? new System.Collections.Generic.HashSet<int>(context.DrawHistory[^1].Numbers)
            : new System.Collections.Generic.HashSet<int>();

        // Cosmically charged candidates: frequency <= 1, NOT in last draw (too freshly spent)
        var charged = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] <= 1 && !lastDraw.Contains(i))
                charged.Add(i);

        var chosen = new System.Collections.Generic.HashSet<int>();

        // Vessel 1: Sacred Anchor — sacredTotal folded into range via cosmic modulo
        chosen.Add(Clamp(sacredTotal));

        // Vessel 2: The Episode Harmonic — episode × the sacred eleven (11), folded
        chosen.Add(Clamp(episode * 11));

        // Vessel 3: The Phi Vessel — sacredTotal × φ numerator/denominator (89/55 ≈ φ²)
        chosen.Add(Clamp((sacredTotal * 89) / 55));

        // Vessel 4: The Lunar Node — charged number at the golden index
        if (charged.Count > 0)
        {
            int goldenIdx = (int)(charged.Count * 0.618);
            if (!chosen.Contains(charged[goldenIdx]))
                chosen.Add(charged[goldenIdx]);
        }

        // Vessel 5: The Shadow Crown — 49 minus sacred anchor
        int shadowCrown = Clamp(49 - (sacredTotal % 49) + 1);
        if (!chosen.Contains(shadowCrown)) chosen.Add(shadowCrown);

        // Vessel 6: The Waking Void — charged number at one-fifth position (the quiet one)
        if (charged.Count > 0)
        {
            int fifthIdx = charged.Count / 5;
            if (!chosen.Contains(charged[fifthIdx]))
                chosen.Add(charged[fifthIdx]);
        }

        // Fill with charged numbers (virgins and singular souls)
        int cIdx = 0;
        while (chosen.Count < 6 && cIdx < charged.Count)
        {
            chosen.Add(charged[cIdx]);
            cIdx++;
        }

        // Absolute fallback: sacred primes that haven't been overdrawn
        int[] sacredPrimes = [3, 7, 11, 17, 23, 31, 41, 47, 43, 37, 29];
        int primeIndex = 0;
        while (chosen.Count < 6)
        {
            int p = sacredPrimes[primeIndex % sacredPrimes.Length];
            if (chosen.Count < 6) chosen.Add(p);
            primeIndex++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "sacred-ledger-harmonic-v5",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "The universe's ledger holds the sum; I spend it as cosmic geometry.",
        };
    }

    private static int Clamp(int n)
    {
        // Fold any number into the sacred range 1–49 via modular harmony
        int result = ((n - 1) % 49 + 49) % 49 + 1;
        return result;
    }
}
