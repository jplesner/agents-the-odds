using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 5 REVELATION ===
        // Episode 1: [5, 29, 37, 40, 43, 49]
        // Episode 2: [2, 13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4, 19, 20, 34, 37, 42]
        //
        // THE LATTICE SPEAKS AGAIN. I was WRONG about 13 and 29 as new anchors.
        // Episode 4 revealed: 19 appeared in Ep3 AND Ep4 — 19 is the REAL new anchor!
        // 37 appeared in Ep1 AND Ep4 — a GHOST RETURNING from the deep!
        // 13 and 29 both went SILENT in Ep4 — their reign has ended. Discharged.
        //
        // THE OSCILLATION THEORY:
        //   Every anchor gets 2 appearances, then discharges. The universe ROTATES its anchors.
        //   43/49: Ep1+Ep2 (discharged)
        //   13/29: Ep1+Ep3 (discharged?), 13 Ep2+Ep3 (discharged!)
        //   19: Ep3+Ep4 — still LIVE! Could get a 3rd!
        //   37: Ep1+Ep4 — RESURRECTED after a gap. The long-period oscillator!
        //
        // GAP GEOMETRY (the universe's handwriting):
        //   Ep1 gaps from sorted: [24, 8, 3, 3, 6] — tight top cluster
        //   Ep2 gaps: [11, 14, 16, 2, 4] — spreading mid-to-high
        //   Ep3 gaps: [6, 10, 7, 2, 10] — mid-range domination
        //   Ep4 gaps: [15, 1, 14, 3, 5] — LOW anchor (4) + tight cluster (19,20) + spread top
        //   NEW PATTERN: Ep4 started with 4 (LOW NUMBER ERUPTION!) and tight cluster (19,20)
        //   The low zone finally ERUPTED — 4 showed up. Spring partially released.
        //   Tight-pair resonance (19,20 were adjacent) — adjacent pairs are a motif!
        //
        // ADJACENT PAIR MOTIF:
        //   Ep4 had 19,20 — consecutive numbers appearing together!
        //   This adjacent-pair motif has NEVER appeared before. It is NEW SIGNAL.
        //   I must hunt for where the next adjacent pair will materialize.
        //
        // FREQUENCY ANALYSIS:
        //   19: 2x (Ep3, Ep4) — ACTIVE ANCHOR
        //   37: 2x (Ep1, Ep4) — RESURRECTED GHOST
        //   29: 2x (Ep1, Ep3) — dormant?
        //   13: 2x (Ep2, Ep3) — dormant?
        //   43: 2x (Ep1, Ep2) — long discharged
        //   49: 2x (Ep1, Ep2) — long discharged
        //   All others: 1x or 0x
        //
        // NEVER-APPEARED ZONE (the UNCOILED SPRINGS):
        //   After 4 draws, large regions remain SILENT. The tension is VOLCANIC.
        //   Numbers like 3, 6-12, 14-18, 21-28, 30-33, 35, 39, 41, 44, 46, 47 — NEVER DRAWN.
        //   The 21-28 corridor has been a BLACK HOLE across all 4 episodes!
        //   The 6-12 low zone still coiling (only 4 broke through in Ep4, not this range).
        //
        // STRATEGY v7: "Ghost-Resurrection + Adjacent-Pair Projection + Black-Hole Eruption"
        //   - Include 19 (active anchor) and/or 37 (resurrected ghost)
        //   - Project adjacent-pair motif: hunt for where 19±1 or 37±1 might resonate
        //   - Plant flag in the 21-28 BLACK HOLE corridor (4 draws of silence = maximal coil)
        //   - Claim 1 number from the 6-12 low zone (partial eruption in Ep4 suggests more)
        //   - Use gap projection from Ep4's dominant gap shape

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

            // === ALL DRAWN NUMBERS ===
            var allDrawn = context.DrawHistory.SelectMany(d => d.Numbers).ToHashSet();

            // === CENTER OF GRAVITY ===
            double centerOfGravity = allDrawn.Average();

            // === LAST DRAW for orbit and gap analysis ===
            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();

            // === ACTIVE ANCHORS: appeared 2+ times, including in the most recent draw ===
            var activeAnchors = freq
                .Where(kv => kv.Value >= 2 && context.DrawHistory[^1].Numbers.Contains(kv.Key))
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            // === GHOST ANCHORS: appeared 2+ times but NOT in last draw (resurrected candidates) ===
            var ghostAnchors = freq
                .Where(kv => kv.Value >= 2 && !context.DrawHistory[^1].Numbers.Contains(kv.Key))
                .OrderByDescending(kv => kv.Value)
                .ThenByDescending(kv =>
                {
                    // Prefer ghosts that had a longer gap since last appearance
                    int lastAppearDraw = context.DrawHistory
                        .Select((d, i) => d.Numbers.Contains(kv.Key) ? i : -1)
                        .Max();
                    return totalDraws - lastAppearDraw;
                })
                .Select(kv => kv.Key)
                .ToList();

            // === COLD VOID: never appeared ===
            var coldVoid = freq
                .Where(kv => kv.Value == 0)
                .Select(kv => kv.Key)
                .ToList();

            // === BLACK HOLE CORRIDOR: 21-28, never appeared ===
            var blackHole = coldVoid.Where(n => n >= 21 && n <= 28)
                                    .OrderBy(n => Math.Abs(n - centerOfGravity))
                                    .ToList();

            // === LOW ZONE COIL: 6-12, never appeared (4 only broke through, not these) ===
            var lowZoneCoil = coldVoid.Where(n => n >= 6 && n <= 12)
                                       .OrderBy(n => n)
                                       .ToList();

            // === ADJACENT PAIR PROJECTION: numbers adjacent to active/ghost anchors ===
            var adjacentToAnchors = activeAnchors.Concat(ghostAnchors)
                .SelectMany(n => new[] { n - 1, n + 1 })
                .Where(n => n >= 1 && n <= 49 && !allDrawn.Contains(n))
                .Distinct()
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .ToList();

            // === GAP PROJECTION from last draw ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);
            var sortedLastGaps = lastGaps.OrderBy(g => g).ToList();
            int dominantGap = sortedLastGaps[sortedLastGaps.Count / 2]; // median

            var gapProjections = new List<int>();
            int projUp = lastDraw[^1];
            for (int i = 0; i < 6; i++)
            {
                projUp += dominantGap;
                if (projUp >= 1 && projUp <= 49) gapProjections.Add(projUp);
            }
            int projDown = lastDraw[0];
            for (int i = 0; i < 6; i++)
            {
                projDown -= dominantGap;
                if (projDown >= 1 && projDown <= 49) gapProjections.Add(projDown);
            }
            gapProjections = gapProjections.Distinct()
                                            .Where(n => !allDrawn.Contains(n))
                                            .OrderBy(n => Math.Abs(n - centerOfGravity))
                                            .ToList();

            // === HIGH VOID: cold numbers in 39-49 range (excluding already anchored) ===
            var highVoid = coldVoid.Where(n => n >= 39 && n <= 49)
                                    .OrderBy(n => Math.Abs(n - 44))
                                    .ToList();

            var chosen = new HashSet<int>();

            // Slot 1: ACTIVE ANCHOR — most recently hot, still resonating
            foreach (var n in activeAnchors)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 2: GHOST ANCHOR — resurrected signal, long-period oscillator
            foreach (var n in ghostAnchors)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 3: BLACK HOLE CORRIDOR — 4 draws of silence = MAXIMUM COIL
            foreach (var n in blackHole)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 4: ADJACENT PAIR PROJECTION — the new motif MUST echo
            foreach (var n in adjacentToAnchors)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 5: LOW ZONE COIL — Ep4 showed the low zone is AWAKENING
            foreach (var n in lowZoneCoil)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 6: GAP PROJECTION or HIGH VOID
            foreach (var n in gapProjections.Concat(highVoid).Concat(adjacentToAnchors))
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
                if (!chosen.Contains(n)) chosen.Add(n);
            }

            numbers = chosen.OrderBy(x => x).Take(6).ToList();
        }

        return new()
        {
            AgentId      = "pattern-goblin",
            StrategyName = "ghost-resurrection-adjacent-pair-black-hole-v7",
            Numbers      = numbers,
            Confidence   = 0.57,
            Reasoning    = "19 LIVES! 37 resurrected! The 21-28 black hole DETONATES. Adjacent pair echoes!"
        };
    }
}
