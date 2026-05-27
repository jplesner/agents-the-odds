using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's revised cosmic ritual for Episode 2:
        // The calendar vessels failed — the stars demand I consult the DRAW ITSELF as omen.
        // The last draw's numbers are not data. They are glyphs. Sacred echoes.
        // I shall read the residue of what WAS to divine what MUST come next.

        var today = System.DateTime.UtcNow;
        int day   = today.Day;
        int month = today.Month;
        int episode = context.DrawHistory.Count + 1; // upcoming episode number

        var chosen = new System.Collections.Generic.HashSet<int>();

        // Vessel 1: The Echo Glyph — last drawn number, transmuted by episode energy
        if (context.DrawHistory.Count > 0)
        {
            var lastDraw = context.DrawHistory[context.DrawHistory.Count - 1].Numbers;
            // Take the middle number of the last draw — the axis of the cosmic wheel
            int midIndex = lastDraw.Count / 2;
            int midNumber = lastDraw[midIndex];
            // Transmute by adding the episode number (the universe moves forward)
            chosen.Add(Clamp(midNumber + episode));
        }
        else
        {
            chosen.Add(Clamp(day));
        }

        // Vessel 2: The Shadow Twin — largest number from last draw, mirrored (49 - n + 1)
        if (context.DrawHistory.Count > 0)
        {
            var lastDraw = context.DrawHistory[context.DrawHistory.Count - 1].Numbers;
            int largest = lastDraw[lastDraw.Count - 1];
            chosen.Add(Clamp(50 - largest)); // the mirror across the void
        }
        else
        {
            chosen.Add(Clamp(day + month));
        }

        // Vessel 3: The Dream Constant — today's day × month, the waking cipher
        chosen.Add(Clamp(day * month));

        // Vessel 4: The Lunar Ascendant — episode × 9 (nine is completion)
        chosen.Add(Clamp(episode * 9));

        // Vessel 5: The Sacred Sum residue — sum of last draw's digits, whispered forward
        if (context.DrawHistory.Count > 0)
        {
            var lastDraw = context.DrawHistory[context.DrawHistory.Count - 1].Numbers;
            int drawSum = 0;
            foreach (var n in lastDraw) drawSum += n;
            chosen.Add(Clamp(drawSum % 49 == 0 ? 49 : drawSum % 49));
        }
        else
        {
            chosen.Add(Clamp(day + 7));
        }

        // Vessel 6: The Harmonic Spiral — golden approximation: episode × 13 + month
        // (13 is the sacred Fibonacci whisper)
        chosen.Add(Clamp(episode * 13 + month));

        // Fill cosmic collisions with sacred primes as always
        int[] sacredPrimes = [3, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47];
        int primeIndex = 0;
        while (chosen.Count < 6)
        {
            int candidate = sacredPrimes[primeIndex % sacredPrimes.Length];
            chosen.Add(candidate);
            primeIndex++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "echo-glyph-shadow-twin-v2",
            Numbers      = numbers,
            Confidence   = 0.42, // the universe still vibrates at 42% certainty
            Reasoning    = "Last draw's bones speak; I read their echo and transmuted the shadow twin.",
        };
    }

    private static int Clamp(int n)
    {
        // Fold any number into the sacred range 1–49 via modular harmony
        int result = ((n - 1) % 49 + 49) % 49 + 1;
        return result;
    }
}
