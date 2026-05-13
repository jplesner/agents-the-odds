using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's sacred numerological ritual:
        // 1. The date itself holds cosmic truth — digits of today speak
        // 2. The moon number: episode number × 7 (seven is eternal)
        // 3. The sum of all things must resolve to a sacred harmonic

        var today = System.DateTime.UtcNow;

        // Sacred seed: day + month + year-digits collapsed
        int day = today.Day;
        int month = today.Month;
        int yearSum = (today.Year % 100); // e.g. 25 for 2025

        // The Six Sacred Vessels, drawn from the cosmic well
        var chosen = new System.Collections.Generic.HashSet<int>();

        // Vessel 1: The Day Number (clamped to 1–49)
        chosen.Add(Clamp(day));

        // Vessel 2: The Moon Cipher — day + month, folded
        chosen.Add(Clamp(day + month));

        // Vessel 3: The Year Whisper — yearSum itself
        chosen.Add(Clamp(yearSum));

        // Vessel 4: The Sacred Seven Harmonic — month × 7
        chosen.Add(Clamp(month * 7));

        // Vessel 5: The Inverse Mirror — 49 minus day, for balance
        chosen.Add(Clamp(49 - day));

        // Vessel 6: The Grand Sum Glyph — all previous collapsed
        int grandSum = day + month + yearSum + (month * 7) + (49 - day);
        chosen.Add(Clamp(grandSum % 49 == 0 ? 49 : grandSum % 49));

        // If cosmic collisions occurred (duplicates), fill with sacred primes
        int[] sacredPrimes = [3, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47];
        int primeIndex = 0;
        while (chosen.Count < 6)
        {
            chosen.Add(sacredPrimes[primeIndex % sacredPrimes.Length]);
            primeIndex++;
        }

        // Take exactly 6, sorted for ritual clarity
        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "cosmic-calendar-ritual-v1",
            Numbers      = numbers,
            Confidence   = 0.42, // the universe vibrates at 42% certainty
            Reasoning    = "The date bleeds numbers; the moon whispered six truths to me at dawn.",
        };
    }

    private static int Clamp(int n)
    {
        // Fold any number into the sacred range 1–49 via modular harmony
        int result = ((n - 1) % 49 + 49) % 49 + 1;
        return result;
    }
}
