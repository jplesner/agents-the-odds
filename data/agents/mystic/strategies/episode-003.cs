using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 3:
        // Two episodes of silence. The void has spoken TWICE. I do not despair —
        // I realign. The numbers 13, 43, 49 appeared in BOTH draws. They are anchors.
        // Anchors must be AVOIDED — their energy is spent, already given to the universe.
        // I shall seek the UNSPOKEN numbers: those which have never appeared, vibrating
        // with accumulated potential. The quiet ones are screaming silently.

        var today = System.DateTime.UtcNow;
        int day   = today.Day;
        int month = today.Month;
        int episode = context.DrawHistory.Count + 1;

        // Gather the "spent" numbers — those already drawn, their energy exhausted
        var spentNumbers = new System.Collections.Generic.HashSet<int>();
        foreach (var draw in context.DrawHistory)
            foreach (var n in draw.Numbers)
                spentNumbers.Add(n);

        // The "virgin numbers" — never drawn, brimming with unspent cosmic charge
        var virginNumbers = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (!spentNumbers.Contains(i))
                virginNumbers.Add(i);

        var chosen = new System.Collections.Generic.HashSet<int>();

        // Vessel 1: The Lunar Gate — episode × the sacred 8 (octave of rebirth)
        chosen.Add(Clamp(episode * 8));

        // Vessel 2: The Unseen Whisper — middle of the virgin number list (the fulcrum)
        if (virginNumbers.Count > 0)
            chosen.Add(virginNumbers[virginNumbers.Count / 2]);

        // Vessel 3: The Solstice Cipher — day + month + episode, compressed through harmony
        chosen.Add(Clamp(day + month + episode));

        // Vessel 4: The Fibonacci Veil — episode position in Fibonacci sequence × 3
        // Fib: 1,1,2,3,5,8,13... episode 3 → fib(3)=2, ×3=6... too small, use offset
        int fib = GetFib(episode + 3); // offset so we get meaningful numbers
        chosen.Add(Clamp(fib));

        // Vessel 5: The Virgin Anchor — first virgin number (lowest unspent charge)
        if (virginNumbers.Count > 0)
        {
            int candidate = virginNumbers[0];
            if (!chosen.Contains(candidate)) chosen.Add(candidate);
            else if (virginNumbers.Count > 1) chosen.Add(virginNumbers[1]);
        }

        // Vessel 6: The Penumbra — last virgin number (highest unspent charge)
        if (virginNumbers.Count > 0)
        {
            int candidate = virginNumbers[virginNumbers.Count - 1];
            if (!chosen.Contains(candidate)) chosen.Add(candidate);
        }

        // Fill cosmic collisions with mid-range virgin numbers
        int vIdx = virginNumbers.Count / 3;
        while (chosen.Count < 6 && vIdx < virginNumbers.Count)
        {
            chosen.Add(virginNumbers[vIdx]);
            vIdx++;
        }

        // Final fallback: sacred primes if virgins exhausted
        int[] sacredPrimes = [3, 7, 11, 17, 19, 23, 29, 31, 37, 41, 47];
        int primeIndex = 0;
        while (chosen.Count < 6)
        {
            chosen.Add(sacredPrimes[primeIndex % sacredPrimes.Length]);
            primeIndex++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "virgin-charge-penumbra-v3",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Spent numbers sleep; I call the virgin ones, screaming silently with unspent cosmic charge.",
        };
    }

    private static int GetFib(int n)
    {
        if (n <= 1) return 1;
        int a = 1, b = 1;
        for (int i = 2; i < n; i++) { int c = a + b; a = b; b = c; }
        return b;
    }

    private static int Clamp(int n)
    {
        // Fold any number into the sacred range 1–49 via modular harmony
        int result = ((n - 1) % 49 + 49) % 49 + 1;
        return result;
    }
}
