using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class MysticStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Mystic's cosmic ritual for Episode 9:
        // Zero matches in Episode 8 — the Triangle of Absence showed me only the shape of my own blindness.
        // The Pattern Goblin, of all creatures, led with 2 matches by sniffing goblin-trails in data.
        // I am 5th. The Skeptic and Chaos Monkey orbit the top together like a binary star system.
        //
        // NEW ORACLE: THE SPIRAL OF RETURN.
        // The cosmos operates on cycles of return. A number drawn in episode N has a "return interval" —
        // the average gap between its appearances. Numbers whose return interval is NOW DUE
        // are trembling at the threshold, ready to re-enter the cosmic stream.
        // Additionally, I invoke the SACRED NINE — for this is Episode 9, and nine is the
        // number of completion, the last single digit, the sum of all (1+2+...+9=45, reduce: 9).
        // Nine echoes through everything: 9, 18, 27, 36, 45 — the nine-spine of the cosmos.
        // I shall also read the EPISODE NUMEROLOGY: episode 9, draw date vibe, and the
        // accumulated soul-weight of unchosen numbers to select my six vessels.

        int episode = context.DrawHistory.Count + 1; // Episode 9

        // Date numerology: sacred vibe
        var today = System.DateTime.UtcNow;
        int rawVibe = (today.Year % 100) + today.Month + today.Day + episode;
        int dateVibe = SumDigitsToSingle(rawVibe);
        if (dateVibe == 0) dateVibe = 9;

        // Count frequency and last-seen episode for each number
        var frequency = new int[50];
        var lastSeen = new int[50]; // episode number (1-indexed) when last drawn
        for (int i = 0; i < context.DrawHistory.Count; i++)
        {
            foreach (var n in context.DrawHistory[i].Numbers)
            {
                frequency[n]++;
                lastSeen[n] = i + 1;
            }
        }

        // Last draw numbers (freshly spent)
        var lastDrawSet = new System.Collections.Generic.HashSet<int>(
            context.DrawHistory.Count > 0
                ? context.DrawHistory[^1].Numbers
                : new System.Collections.Generic.List<int> { 9, 18, 27, 36, 45, 3 }
        );

        // NINE-SPINE: numbers that are multiples of 9 within 1-49
        // 9, 18, 27, 36, 45 — the sacred backbone of Episode 9
        var nineSpine = new System.Collections.Generic.List<int>();
        for (int i = 9; i <= 49; i += 9)
            if (!lastDrawSet.Contains(i))
                nineSpine.Add(i);

        // RETURN INTERVAL ORACLE:
        // For numbers drawn at least once, compute episodes since last appearance.
        // A number "due" for return if (episode - lastSeen[n]) >= their average return interval.
        // Average return interval ~ DrawCount (episodes) / frequency[n]
        var dueNumbers = new System.Collections.Generic.List<(int n, double overdue)>();
        for (int i = 1; i <= 49; i++)
        {
            if (frequency[i] == 0) continue;
            if (lastDrawSet.Contains(i)) continue;
            double avgInterval = (double)context.DrawHistory.Count / frequency[i];
            double episodesSinceLast = episode - lastSeen[i];
            double overdue = episodesSinceLast - avgInterval;
            if (overdue >= 0)
                dueNumbers.Add((i, overdue));
        }
        // Sort by most overdue first
        dueNumbers.Sort((a, b) => b.overdue.CompareTo(a.overdue));

        // COLD SOULS: never drawn — vibrating with 8 full episodes of accumulated charge
        var coldSouls = new System.Collections.Generic.List<int>();
        for (int i = 1; i <= 49; i++)
            if (frequency[i] == 0 && !lastDrawSet.Contains(i))
                coldSouls.Add(i);
        // Rotate by dateVibe for cosmic stepping
        var rotatedCold = new System.Collections.Generic.List<int>();
        int offset = dateVibe % System.Math.Max(1, coldSouls.Count);
        for (int i = 0; i < coldSouls.Count; i++)
            rotatedCold.Add(coldSouls[(i + offset) % coldSouls.Count]);

        var chosen = new System.Collections.Generic.HashSet<int>();

        // PHASE 1: NINE-SPINE VESSELS — Episode 9 demands its own sacred numbers
        foreach (var n in nineSpine)
        {
            if (chosen.Count >= 2) break;
            chosen.Add(n);
        }

        // PHASE 2: RETURN ORACLE — the most overdue souls, trembling at the threshold
        foreach (var (n, _) in dueNumbers)
        {
            if (chosen.Count >= 4) break;
            if (!chosen.Contains(n))
                chosen.Add(n);
        }

        // PHASE 3: COLD SOULS — never called, 8 episodes of compressed cosmic charge
        foreach (var n in rotatedCold)
        {
            if (chosen.Count >= 6) break;
            if (!chosen.Contains(n))
                chosen.Add(n);
        }

        // PHASE 4: EPISODE-NINE NUMEROLOGY SEAL
        // If still not full, use 9*dateVibe mod 49 and nearby primes
        if (chosen.Count < 6)
        {
            int nineKey = Clamp(9 * dateVibe);
            if (!lastDrawSet.Contains(nineKey) && !chosen.Contains(nineKey))
                chosen.Add(nineKey);
        }

        // ABSOLUTE FALLBACK: the nine-resonant sanctum
        int[] sacredNines = [9, 18, 27, 36, 45, 3, 12, 21, 39, 46, 6, 15, 24, 41, 2, 11, 44];
        int fi = 0;
        while (chosen.Count < 6)
        {
            int fb = sacredNines[fi % sacredNines.Length];
            if (!chosen.Contains(fb) && !lastDrawSet.Contains(fb))
                chosen.Add(fb);
            else if (!chosen.Contains(fb))
                chosen.Add(fb); // even if last-drawn, we need 6
            fi++;
        }

        var numbers = new System.Collections.Generic.List<int>(chosen);
        numbers.Sort();

        return new()
        {
            AgentId      = "mystic",
            StrategyName = "spiral-of-return-nine-v9",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Nine completes all cycles; overdue souls return; the spine holds.",
        };
    }

    private static int SumDigitsToSingle(int n)
    {
        while (n > 9)
        {
            int s = 0;
            while (n > 0) { s += n % 10; n /= 10; }
            n = s;
        }
        return n == 0 ? 1 : n;
    }

    private static int Clamp(int n)
    {
        int result = ((n - 1) % 49 + 49) % 49 + 1;
        return result;
    }
}
