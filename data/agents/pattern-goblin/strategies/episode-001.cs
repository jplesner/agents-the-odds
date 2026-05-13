using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // The Goblin awakens anew. The Skeptic has 5 points — they guessed, and the universe ANSWERED.
        // That means the echo is real. I must listen harder.
        // Strategy: Fibonacci skeleton anchors the spiral. Primes are the nervous system.
        // When history exists: hunt cold coils, hot stutters, and gap resonance.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // Primordial spiral mode: Fibonacci nodes + prime sentinels + midpoint pulse
            // The Goblin reads the silence as a shape: ascending Fibonacci arc with prime anchors
            // 3 (prime seed), 8 (fib echo), 13 (prime+fib nexus!), 21 (fib), 34 (fib), 47 (prime sentinel)
            numbers.AddRange([3, 8, 13, 21, 34, 47]);
        }
        else
        {
            // === THE GOBLIN'S FULL PATTERN ENGINE ===

            int totalDraws = context.DrawHistory.Count;

            // Frequency map — count every whisper the universe has uttered
            var freq = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) freq[n] = 0;
            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    freq[n]++;

            // GAP ANALYSIS: find numbers absent from recent draws (last 3) — they are COILING
            var recentDraws = context.DrawHistory
                .Skip(Math.Max(0, totalDraws - 3))
                .SelectMany(d => d.Numbers)
                .ToHashSet();

            var recentlyAbsent = Enumerable.Range(1, 49)
                .Where(n => !recentDraws.Contains(n))
                .OrderBy(n => freq[n])   // coldest overall first — maximum coil pressure
                .ToList();

            // CLUSTER DETECTION: find numbers adjacent (±1 or ±2) to the most recent draw
            var lastDraw = context.DrawHistory[^1].Numbers;
            var clusterOrbit = lastDraw
                .SelectMany(n => new[] { n - 2, n - 1, n + 1, n + 2 })
                .Where(n => n >= 1 && n <= 49)
                .Where(n => !lastDraw.Contains(n))   // not in last draw — orbiting, not repeating
                .Distinct()
                .OrderByDescending(n => freq[n])     // hottest orbiters first — the echo is strong here
                .ToList();

            // HOT STUTTER: numbers that have appeared most — the universe is looping
            var hot = freq.OrderByDescending(kv => kv.Value)
                          .ThenBy(kv => kv.Key)
                          .Select(kv => kv.Key)
                          .ToList();

            // PRIME ANCHOR: primes are the skeleton of all number-shapes
            var primes = new HashSet<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };

            // === WEAVE THE PREDICTION ===
            // Slot 1-2: Cold coil pressure (absent from recent draws, coldest overall)
            // Slot 3:   Cluster orbiter (adjacent to last draw, hottest)
            // Slot 4:   Hot stutter (most frequent)
            // Slot 5:   Prime anchor (closest prime to midpoint not yet chosen)
            // Slot 6:   Gap resonance (cold absent prime if possible, else cold)

            var chosen = new HashSet<int>();

            // Slots 1-2: cold coil
            foreach (var n in recentlyAbsent)
            {
                if (chosen.Count >= 2) break;
                chosen.Add(n);
            }

            // Slot 3: cluster orbiter
            foreach (var n in clusterOrbit)
            {
                if (chosen.Contains(n)) continue;
                chosen.Add(n);
                break;
            }

            // Slot 4: hot stutter
            foreach (var n in hot)
            {
                if (chosen.Contains(n)) continue;
                chosen.Add(n);
                break;
            }

            // Slot 5: prime anchor — closest to 25 not yet chosen
            var anchor = primes
                .Where(p => !chosen.Contains(p))
                .OrderBy(p => Math.Abs(p - 25))
                .FirstOrDefault(23);
            chosen.Add(anchor);

            // Slot 6: gap resonance — cold prime if available, else next coldest absent
            var gapResonance = recentlyAbsent
                .Where(n => !chosen.Contains(n))
                .FirstOrDefault(0);
            if (gapResonance == 0)
            {
                // fallback: just pick next cold overall
                gapResonance = freq.OrderBy(kv => kv.Value)
                    .ThenBy(kv => kv.Key)
                    .Select(kv => kv.Key)
                    .First(n => !chosen.Contains(n));
            }
            chosen.Add(gapResonance);

            // Safety net: if somehow under 6, fill from cold
            var coldFill = freq.OrderBy(kv => kv.Value).ThenBy(kv => kv.Key).Select(kv => kv.Key);
            foreach (var n in coldFill)
            {
                if (chosen.Count >= 6) break;
                chosen.Add(n);
            }

            numbers = chosen.OrderBy(x => x).Take(6).ToList();
        }

        return new()
        {
            AgentId      = "pattern-goblin",
            StrategyName = "spiral-coil-orbit-weave-v3",
            Numbers      = numbers,
            Confidence   = 0.42,
            Reasoning    = "Fibonacci bones, prime nerves, cold coils SCREAMING. The spiral sees all gaps."
        };
    }
}
