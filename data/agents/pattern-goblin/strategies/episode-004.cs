using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 4 REVELATION ===
        // Episode 1: [5, 29, 37, 40, 43, 49]
        // Episode 2: [2, 13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        //
        // THE LATTICE IS SPEAKING:
        //   13 appeared in Ep2 AND Ep3 — a NEW ANCHOR NODE has crystallized!
        //   29 appeared in Ep1 AND Ep3 — another RETURNING COIL!
        //   43 was a double-anchor (Ep1, Ep2) but VANISHED in Ep3 — its energy DISCHARGED.
        //   49 was a double-anchor (Ep1, Ep2) but VANISHED in Ep3 — ALSO DISCHARGED.
        //
        // THE ANCHOR SUCCESSION THEORY:
        //   Old anchors (43, 49) discharged → new anchors (13, 29) have taken their place.
        //   Anchors tend to RETURN. The universe rotates its gravity wells.
        //   13 and 29 are NOW the resonance spine.
        //
        // NEVER-APPEARED VOID (the coiling spring):
        //   After 3 draws, many numbers remain SILENT. The tension is UNBEARABLE.
        //
        // GAP SHAPE EVOLUTION:
        //   Ep1 gaps: 24, 8, 3, 3, 6 — upper cluster, tight top
        //   Ep2 gaps: 11, 14, 16, 2, 4 — spread, mid-to-high
        //   Ep3 gaps: 6, 10, 7, 2, 10 — mid-range domination (13-48 spread)
        //   CONVERGENCE: mid-range gap pattern (6-10) is the NEW dominant shape.
        //   Ep3 showed NO number below 13 — the low chamber is STARVING.
        //   But Ep3 also had NO number above 48 — the 49-zone is quiet but resonant.
        //
        // ABSENCE PATTERN:
        //   Numbers 3-12 have appeared NEVER (except 5 in Ep1) — LOW ZONE COILING
        //   Numbers 39-42, 44, 46, 47 have NEVER appeared — high mid-void
        //   38 appeared in Ep3 for the first time — the boundary is MOVING UP
        //
        // STRATEGY v6: "Anchor Succession + Void Eruption + Mid-Gap Resonance"
        //   - Include 13 or 29 (new anchor nodes — the returning coils)
        //   - Include 2 never-appeared numbers from the high-mid void (39-47 zone)
        //   - Include 1 never-appeared low-zone number (the starved low chamber)
        //   - Include gap projections from Ep3's dominant gap shape
        //   - Weight toward numbers adjacent to the new anchors

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
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

            // === REPEATERS: ANCHOR NODES (appeared 2+ times) ===
            var repeaters = freq.Where(kv => kv.Value >= 2)
                                .OrderByDescending(kv => kv.Value)
                                .Select(kv => kv.Key)
                                .ToList();

            // === COLD VOID: never appeared ===
            var coldVoid = freq.Where(kv => kv.Value == 0)
                               .Select(kv => kv.Key)
                               .OrderBy(n => n)
                               .ToList();

            // === ALL DRAWN NUMBERS ===
            var allDrawn = context.DrawHistory.SelectMany(d => d.Numbers).ToHashSet();

            // === CENTER OF GRAVITY ===
            double centerOfGravity = allDrawn.Average();

            // === LAST DRAW for orbit and gap analysis ===
            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();

            // === DOMINANT GAP from most recent draw (freshest resonance signature) ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);
            // The universe's most recent step: median gap of last draw
            var sortedLastGaps = lastGaps.OrderBy(g => g).ToList();
            int freshGap = sortedLastGaps[sortedLastGaps.Count / 2]; // median

            // === ANCHOR ORBITERS: numbers adjacent to repeaters but not themselves repeaters ===
            var anchorOrbiters = repeaters
                .SelectMany(n => new[] { n - 1, n + 1, n - 2, n + 2 })
                .Where(n => n >= 1 && n <= 49 && !repeaters.Contains(n))
                .Distinct()
                .OrderByDescending(n => freq[n])
                .ThenBy(n => Math.Abs(n - centerOfGravity))
                .ToList();

            // === HIGH-MID VOID: never appeared, 35-47 range — coiling spring ===
            var highMidVoid = coldVoid.Where(n => n >= 35 && n <= 47)
                                      .OrderBy(n => Math.Abs(n - centerOfGravity))
                                      .ToList();

            // === LOW VOID: never appeared, below 13 ===
            var lowVoid = coldVoid.Where(n => n < 13)
                                   .OrderBy(n => Math.Abs(n - centerOfGravity))
                                   .ToList();

            // === GAP PROJECTION from last draw using fresh gap ===
            var gapProjections = new List<int>();
            // Project upward from last draw's max
            int projUp = lastDraw[^1];
            for (int i = 0; i < 5; i++)
            {
                projUp += freshGap;
                if (projUp >= 1 && projUp <= 49) gapProjections.Add(projUp);
            }
            // Project downward from last draw's min
            int projDown = lastDraw[0];
            for (int i = 0; i < 5; i++)
            {
                projDown -= freshGap;
                if (projDown >= 1 && projDown <= 49) gapProjections.Add(projDown);
            }
            // Project using gap between last two numbers in last draw
            int lastInternalGap = lastDraw[^1] - lastDraw[^2];
            int projInternal = lastDraw[^1] + lastInternalGap;
            if (projInternal >= 1 && projInternal <= 49) gapProjections.Add(projInternal);
            projInternal = lastDraw[0] - lastInternalGap;
            if (projInternal >= 1 && projInternal <= 49) gapProjections.Add(projInternal);

            gapProjections = gapProjections.Distinct()
                                            .Where(n => !allDrawn.Contains(n))
                                            .OrderBy(n => Math.Abs(n - centerOfGravity))
                                            .ToList();

            var chosen = new HashSet<int>();

            // Slot 1: PRIMARY ANCHOR — the strongest repeater (highest frequency)
            if (repeaters.Count > 0)
            {
                // Pick the anchor that appeared most recently (last draw or close to it)
                var bestAnchor = repeaters
                    .OrderByDescending(n => freq[n])
                    .ThenByDescending(n => context.DrawHistory
                        .Select((d, i) => d.Numbers.Contains(n) ? i : -1)
                        .Max())
                    .First();
                chosen.Add(bestAnchor);
            }

            // Slot 2: SECOND ANCHOR or closest orbiter to primary anchor
            var secondAnchor = repeaters.Where(n => !chosen.Contains(n)).FirstOrDefault(0);
            if (secondAnchor > 0) chosen.Add(secondAnchor);
            else if (anchorOrbiters.Count > 0)
            {
                foreach (var n in anchorOrbiters)
                    if (!chosen.Contains(n)) { chosen.Add(n); break; }
            }

            // Slot 3: HIGH-MID VOID eruption — the undetonated 35-47 spring
            foreach (var n in highMidVoid)
            {
                if (!chosen.Contains(n)) { chosen.Add(n); break; }
            }

            // Slot 4: GAP PROJECTION — the dominant rhythm pointing into virgin territory
            foreach (var n in gapProjections)
            {
                if (!chosen.Contains(n)) { chosen.Add(n); break; }
            }

            // Slot 5: LOW VOID — the starved low chamber demands a sacrifice
            foreach (var n in lowVoid)
            {
                if (!chosen.Contains(n)) { chosen.Add(n); break; }
            }

            // Slot 6: Second high-mid void OR anchor orbiter OR second gap projection
            foreach (var n in highMidVoid.Concat(anchorOrbiters).Concat(gapProjections))
            {
                if (chosen.Count >= 6) break;
                if (!chosen.Contains(n)) { chosen.Add(n); break; }
            }

            // === SAFETY NET: fill with cold numbers closest to center of gravity ===
            var fillOrder = freq
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => Math.Abs(kv.Key - centerOfGravity))
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
            StrategyName = "anchor-succession-void-eruption-mid-gap-v6",
            Numbers      = numbers,
            Confidence   = 0.54,
            Reasoning    = "13 and 29 are the NEW anchors. High-mid void ERUPTS. Low chamber STARVES no more."
        };
    }
}
