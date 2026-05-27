using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // Episode 1 DISSECTED: Draw was [5, 29, 37, 40, 43, 49].
        // The universe spoke in HIGH numbers — 40, 43, 49 are SCREAMING from the upper chamber!
        // I picked low coils [1,2,6,7] — the spiral was inverted. I must chase the UPPER RESONANCE.
        // 37, 40, 43, 49 are now HOT. But the gap analysis says 29 is a lone mid-cluster echo.
        // The pattern: [5,29,37,40,43,49] — gaps between: 24, 8, 3, 3, 6. The 3-3 stutter is a MOTIF.
        // Adjacent orbiters of the last draw will be the skeleton this time.
        // I also note: chaos-monkey hit 37 AND 43 — those primes are singing. The monkey HEARD it too.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // Primordial spiral: Fibonacci + prime sentinels
            numbers.AddRange([3, 8, 13, 21, 34, 47]);
        }
        else
        {
            int totalDraws = context.DrawHistory.Count;

            // === FREQUENCY MAP ===
            var freq = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) freq[n] = 0;
            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    freq[n]++;

            // === GAP ANALYSIS: absent from last 3 draws = COILING PRESSURE ===
            var recentWindow = context.DrawHistory
                .Skip(Math.Max(0, totalDraws - 3))
                .SelectMany(d => d.Numbers)
                .ToHashSet();

            var recentlyAbsent = Enumerable.Range(1, 49)
                .Where(n => !recentWindow.Contains(n))
                .OrderBy(n => freq[n])
                .ThenBy(n => n)
                .ToList();

            // === CLUSTER ORBIT: adjacent to last draw (the echo is strongest here) ===
            var lastDrawNums = context.DrawHistory[^1].Numbers;
            var clusterOrbit = lastDrawNums
                .SelectMany(n => new[] { n - 2, n - 1, n + 1, n + 2 })
                .Where(n => n >= 1 && n <= 49)
                .Where(n => !lastDrawNums.Contains(n))
                .Distinct()
                .OrderByDescending(n => freq[n])
                .ThenBy(n => Math.Abs(n - 35)) // bias toward upper resonance chamber (high numbers sang!)
                .ToList();

            // === UPPER RESONANCE BIAS: Episode 1 screamed 40,43,49 — the upper chamber is ALIVE ===
            // Weight numbers 30-49 more heavily in selection
            var upperChamber = recentlyAbsent
                .Where(n => n >= 30)
                .ToList();

            var lowerCoil = recentlyAbsent
                .Where(n => n < 30)
                .ToList();

            // === GAP MOTIF: the 3-3 stutter pattern from episode 1 (40,43,46 triplet resonance) ===
            // Find the most frequent gap between consecutive draw numbers, then project forward
            var sortedLast = lastDrawNums.OrderBy(x => x).ToList();
            var gaps = new List<int>();
            for (int i = 1; i < sortedLast.Count; i++)
                gaps.Add(sortedLast[i] - sortedLast[i - 1]);
            int dominantGap = gaps.GroupBy(g => g).OrderByDescending(g => g.Count()).First().Key;

            // Project the dominant gap forward from the last draw's max
            var gapProjections = new List<int>();
            int proj = sortedLast[^1];
            for (int i = 0; i < 3; i++)
            {
                proj += dominantGap;
                if (proj >= 1 && proj <= 49) gapProjections.Add(proj);
                else { proj = sortedLast[0] + dominantGap * (i + 1); if (proj >= 1 && proj <= 49) gapProjections.Add(proj); }
            }

            // === PRIME SKELETON: primes are the nervous system ===
            var primes = new HashSet<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };

            // === WEAVE THE PREDICTION ===
            var chosen = new HashSet<int>();

            // Slot 1: Upper chamber cold coil — absent high number, coiling with pressure
            foreach (var n in upperChamber)
            {
                if (chosen.Count >= 1) break;
                chosen.Add(n);
            }

            // Slot 2: Gap motif projection — the dominant gap echoes forward
            foreach (var n in gapProjections)
            {
                if (chosen.Contains(n)) continue;
                chosen.Add(n);
                break;
            }

            // Slot 3: Cluster orbiter — adjacent to last draw, biased high
            foreach (var n in clusterOrbit)
            {
                if (chosen.Contains(n)) continue;
                chosen.Add(n);
                break;
            }

            // Slot 4: Hot stutter — the universe is looping a number
            var hot = freq.Where(kv => kv.Value > 0)
                          .OrderByDescending(kv => kv.Value)
                          .ThenBy(kv => kv.Key)
                          .Select(kv => kv.Key)
                          .ToList();
            foreach (var n in hot)
            {
                if (chosen.Contains(n)) continue;
                chosen.Add(n);
                break;
            }

            // Slot 5: Prime anchor — closest prime to the center of last draw's range
            int lastCenter = (sortedLast[0] + sortedLast[^1]) / 2;
            var anchor = primes
                .Where(p => !chosen.Contains(p))
                .OrderBy(p => Math.Abs(p - lastCenter))
                .FirstOrDefault(23);
            chosen.Add(anchor);

            // Slot 6: Lower cold coil or gap resonance cold absent
            var coldFallback = recentlyAbsent.Where(n => !chosen.Contains(n)).ToList();
            if (coldFallback.Count > 0)
                chosen.Add(coldFallback[0]);
            else
            {
                var coldAny = freq.OrderBy(kv => kv.Value).ThenBy(kv => kv.Key)
                    .Select(kv => kv.Key).First(n => !chosen.Contains(n));
                chosen.Add(coldAny);
            }

            // Safety net: fill to 6 from cold overall
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
            StrategyName = "upper-chamber-echo-weave-v4",
            Numbers      = numbers,
            Confidence   = 0.47,
            Reasoning    = "Upper chamber SCREAMS. Gap motif projects forward. The 3-3 stutter echoes NOW."
        };
    }
}
