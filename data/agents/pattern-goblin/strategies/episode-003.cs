using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 3 DISSECTION ===
        // Episode 1: [5, 29, 37, 40, 43, 49] — high cluster, upper chamber SCREAMING
        // Episode 2: [2, 13, 27, 43, 45, 49] — the REPEATERS: 43 and 49 returned!
        //
        // THE REVELATION: 43 appeared in BOTH draws. 49 appeared in BOTH draws.
        // These are ANCHOR NODES — numbers that have locked into the resonance field!
        // But wait — I must not blindly chase the anchors. They may have discharged their energy.
        //
        // CROSS-DRAW MOTIF ANALYSIS:
        //   Ep1 gaps (sorted): 5,29,37,40,43,49 → gaps: 24,8,3,3,6
        //   Ep2 gaps (sorted): 2,13,27,43,45,49 → gaps: 11,14,16,2,4
        //   Combined gap signature: the SMALL gaps (2,3,3,4,6) cluster near the HIGH end
        //   The HIGH numbers (40-49) are a TIGHT CLUSTER — a gravity well pulling numbers in!
        //
        // NEW THEORY: The "mid-desert" (14-26) has appeared only ONCE total (13,27 border it).
        //   Numbers 14-26 are a COLD VOID with coiling pressure — the spring is LOADED.
        //   But also: numbers that were ADJACENT to repeaters (41,42,44,45,47,48,50) orbit them.
        //
        // STRATEGY v5: "Dual-Anchor Resonance with Cold Desert Eruption"
        //   - Track repeaters (appeared in multiple draws) → these are GRAVITY ANCHORS
        //   - Track cold desert (never appeared) → COILING PRESSURE, select mid-range cold
        //   - Include an orbiter of the anchor cluster
        //   - Include a cross-draw gap projection
        //   - Prime nervous system anchor
        //   - One wildcard from the true cold void

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            // Primordial spiral: Fibonacci + prime sentinels
            numbers.AddRange([3, 8, 13, 21, 34, 47]);
        }
        else
        {
            int totalDraws = context.DrawHistory.Count;

            // === FREQUENCY MAP — the heat signature of the universe ===
            var freq = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) freq[n] = 0;
            foreach (var draw in context.DrawHistory)
                foreach (var n in draw.Numbers)
                    freq[n]++;

            // === REPEATERS: numbers that appeared in 2+ draws — GRAVITY ANCHORS ===
            var repeaters = freq.Where(kv => kv.Value >= 2).Select(kv => kv.Key).ToList();

            // === COLD VOID: never appeared — COILING SPRING PRESSURE ===
            var coldVoid = freq.Where(kv => kv.Value == 0).Select(kv => kv.Key).OrderBy(n => n).ToList();

            // === ALL DRAWN NUMBERS (flat) ===
            var allDrawn = context.DrawHistory.SelectMany(d => d.Numbers).ToHashSet();

            // === LAST DRAW for orbit analysis ===
            var lastDraw = context.DrawHistory[^1].Numbers;
            var sortedLast = lastDraw.OrderBy(x => x).ToList();

            // === GAP MOTIF: analyze gaps across ALL draws ===
            var allGaps = new List<int>();
            foreach (var draw in context.DrawHistory)
            {
                var sorted = draw.Numbers.OrderBy(x => x).ToList();
                for (int i = 1; i < sorted.Count; i++)
                    allGaps.Add(sorted[i] - sorted[i - 1]);
            }
            // The dominant gap is the universe's preferred step size
            int dominantGap = allGaps.GroupBy(g => g)
                                     .OrderByDescending(g => g.Count())
                                     .ThenBy(g => g.Key)
                                     .First().Key;

            // === CLUSTER ORBIT: adjacent to repeater anchors ===
            var anchorOrbiters = repeaters
                .SelectMany(n => new[] { n - 2, n - 1, n + 1, n + 2 })
                .Where(n => n >= 1 && n <= 49)
                .Where(n => !repeaters.Contains(n))
                .Distinct()
                .OrderByDescending(n => freq[n])
                .ThenByDescending(n => n) // prefer high numbers — upper chamber still resonates
                .ToList();

            // === COLD DESERT: mid-range cold numbers (14-36) — the undetonated spring ===
            var coldDesert = coldVoid.Where(n => n >= 14 && n <= 36).ToList();

            // === PRIME SKELETON ===
            var primes = new HashSet<int> { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47 };

            // === GAP PROJECTION from last draw's sorted sequence ===
            var gapProjections = new List<int>();
            int proj = sortedLast[^1];
            for (int i = 0; i < 4; i++)
            {
                proj += dominantGap;
                if (proj >= 1 && proj <= 49 && !allDrawn.Contains(proj)) { gapProjections.Add(proj); }
                else { proj -= dominantGap * 2; if (proj >= 1 && proj <= 49 && !allDrawn.Contains(proj)) gapProjections.Add(proj); }
            }
            // Also project backward from the minimum
            int projBack = sortedLast[0];
            for (int i = 0; i < 4; i++)
            {
                projBack -= dominantGap;
                if (projBack >= 1 && projBack <= 49 && !allDrawn.Contains(projBack)) gapProjections.Add(projBack);
            }
            gapProjections = gapProjections.Distinct().ToList();

            // === WEAVE THE PREDICTION — 6 slots ===
            var chosen = new HashSet<int>();

            // Slot 1: COLD DESERT eruption — the mid-range void is SCREAMING to be filled
            // Pick the cold mid-range number closest to the "center of gravity" of all draws
            double centerOfGravity = allDrawn.Average();
            var desertEruption = coldDesert
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .FirstOrDefault(0);
            if (desertEruption > 0) chosen.Add(desertEruption);

            // Slot 2: Anchor orbiter — riding the gravity well of the repeaters
            foreach (var n in anchorOrbiters)
            {
                if (chosen.Contains(n)) continue;
                chosen.Add(n);
                break;
            }

            // Slot 3: Gap projection — the dominant rhythm projects into new territory
            foreach (var n in gapProjections)
            {
                if (chosen.Contains(n)) continue;
                chosen.Add(n);
                break;
            }

            // Slot 4: Prime nervous system — closest prime to center of gravity
            var primeAnchor = primes
                .Where(p => !chosen.Contains(p) && !allDrawn.Contains(p))
                .OrderBy(p => Math.Abs(p - centerOfGravity))
                .FirstOrDefault(0);
            if (primeAnchor == 0)
                primeAnchor = primes.Where(p => !chosen.Contains(p)).OrderBy(p => Math.Abs(p - centerOfGravity)).First();
            chosen.Add(primeAnchor);

            // Slot 5: Coldest void number outside mid-desert (true extremity — low or high)
            var extremeCold = coldVoid
                .Where(n => !chosen.Contains(n) && (n < 14 || n > 36))
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .FirstOrDefault(0);
            if (extremeCold > 0) chosen.Add(extremeCold);

            // Slot 6: Second cold desert or second gap projection
            var secondDesert = coldDesert.Where(n => !chosen.Contains(n))
                .OrderBy(n => Math.Abs(n - centerOfGravity)).FirstOrDefault(0);
            if (secondDesert > 0) chosen.Add(secondDesert);

            // === SAFETY NET: fill remaining slots with cold-then-hot ===
            var fillOrder = freq
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => Math.Abs(kv.Key - (int)centerOfGravity))
                .Select(kv => kv.Key);
            foreach (var n in fillOrder)
            {
                if (chosen.Count >= 6) break;
                chosen.Add(n);
            }

            numbers = chosen.OrderBy(x => x).Take(6).ToList();
        }

        return new()
        {
            AgentId      = "pattern-goblin",
            StrategyName = "dual-anchor-cold-desert-eruption-v5",
            Numbers      = numbers,
            Confidence   = 0.51,
            Reasoning    = "43 and 49 are GRAVITY ANCHORS. The cold desert ERUPTS. Dominant gap projects NOW."
        };
    }
}
