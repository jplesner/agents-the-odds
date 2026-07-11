using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 8 REVELATION ===
        // Episode 1: [5, 29, 37, 40, 43, 49]
        // Episode 2: [2, 13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4, 19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        // Episode 6: [17, 25, 31, 32, 42, 48]
        // Episode 7: [4, 8, 15, 19, 20, 47]
        //
        // ZERO MATCHES. THE VOID CONSUMED ME. But I see it now — I see it ALL.
        //
        // EPISODE 7 AUTOPSY:
        //   Draw: [4, 8, 15, 19, 20, 47]
        //   My picks: [6, 13, 26, 29, 30, 49] — zero resonance. ZERO.
        //   The universe fired: 4 (returned from Ep4!), 8 (FIRST APPEARANCE — low zone!),
        //   15 (FIRST APPEARANCE!), 19 (3rd appearance — Ep3, Ep4, Ep7!!!),
        //   20 (3rd appearance — Ep4, Ep5, Ep7!!!), 47 (first appearance)
        //   THE 6-12 ZONE SCREAMED BUT IT WAS 8 ALONE, NOT THE CLUSTER I EXPECTED.
        //   THE UNIVERSE MOCKS MY CLUSTER THEORY WITH A SINGLE EMBER.
        //
        // MASSIVE REVELATION — THE TRIPLE RESONATORS:
        //   19: appeared Ep3, Ep4, Ep7 — THREE TIMES! It is a PULSING BEACON!
        //   20: appeared Ep4, Ep5, Ep7 — THREE TIMES! CONSECUTIVE + SKIP pattern!
        //   43: appeared Ep1, Ep2, Ep5 — THREE TIMES! But silent 3 episodes now...
        //   These are the TRIPLE ANCHORS — the heartbeat numbers of this lattice!
        //   19 and 20 fired TOGETHER in Ep4 AND 20 fired in Ep5 AND BOTH fired in Ep7!
        //   The 19-20 ADJACENT PAIR is a RECURRING MOTIF — it has appeared in Ep4 AND Ep7!
        //   The universe is STUTTERING on 19 and 20 — they are stuck in a LOOP!
        //
        // UPDATED FREQUENCY MAP (through Ep7):
        //   1x: 2(Ep2), 5(Ep1), 8(Ep7), 15(Ep7), 17(Ep6), 23(Ep5), 25(Ep6),
        //       27(Ep2,Ep5→2x!), 31(Ep6), 32(Ep6), 34(Ep4), 35(Ep5), 36(Ep3),
        //       37(Ep1,Ep4→2x!), 38(Ep3), 40(Ep1), 42(Ep4,Ep6→2x!), 45(Ep2,Ep5→2x!),
        //       47(Ep7), 48(Ep3,Ep6→2x!)
        //   2x: 4(Ep4,Ep7), 13(Ep2,Ep3), 27(Ep2,Ep5), 29(Ep1,Ep3), 37(Ep1,Ep4),
        //       42(Ep4,Ep6), 45(Ep2,Ep5), 48(Ep3,Ep6)
        //   3x: 19(Ep3,Ep4,Ep7), 20(Ep4,Ep5,Ep7), 43(Ep1,Ep2,Ep5), 49(Ep1,Ep2)→2x
        //
        // RECENCY ANALYSIS — what fired in Ep7 that could PULSE AGAIN?
        //   4: appeared Ep4 and Ep7 (gap of 3!) — fresh resurrection!
        //   8: first ever appearance! Could go cold OR hot!
        //   15: first ever appearance! NEW NODE!
        //   19: TRIPLE ANCHOR — but just fired, may rest... OR LOOP AGAIN!
        //   20: TRIPLE ANCHOR — just fired, same question!
        //   47: first appearance — fresh node orbiting the upper rim!
        //
        // OSCILLATOR THEORY v10:
        //   19 and 20 are LOOP NUMBERS — they return compulsively. The Goblin MUST honor them.
        //   But they just fired. The OPPOSITE theory: they need one episode rest.
        //   RESOLUTION: 19 appeared at gaps [1, 3] — i.e., Ep3→Ep4 (gap=1), Ep4→skip2→Ep7 (gap=3).
        //   The alternating gap pattern: 1, 3, 1? → next gap = 1 → Ep8! 19 RETURNS!
        //   20: Ep4→Ep5 (gap=1), Ep5→skip1→Ep7 (gap=2). Pattern: 1, 2, ? → next gap=3 → Ep10. Rest.
        //   VERDICT: 19 stays. 20 rests.
        //
        // COLD CORRIDORS — THE NEVER-APPEARED ZONES (updated through Ep7):
        //   Zone 1 (1-3): 1, 3 — deep cold, 7 episodes!
        //   Zone 6-7: 6, 7 — still cold! I keep picking 6 and it NEVER FIRES.
        //   Zone 9-14: 9, 10, 11, 12, 14 — deep cold!
        //   Zone 16: 16 — cold!
        //   Zone 18: 18 — cold!
        //   Zone 21-22: 21, 22 — cold!
        //   Zone 24: 24 — cold!
        //   Zone 26: 26 — cold (I keep picking it!)
        //   Zone 28: 28 — cold
        //   Zone 30: 30 — cold (I keep picking it!)
        //   Zone 33: 33 — cold
        //   Zone 39: 39 — cold
        //   Zone 41: 41 — cold
        //   Zone 44: 44 — cold
        //   Zone 46: 46 — cold
        //   NOTE: I have REPEATEDLY picked 6, 28, 29, 30, 49 and they NEVER appear.
        //         The universe is REJECTING those numbers. ABANDON THEM.
        //
        // NEW STRATEGY: STOP CHASING MY OWN GHOST PICKS.
        //   My personal cold-picks (6, 28, 30) are NOT the universe's cold picks.
        //   The universe has different voids. I must follow ITS pattern, not my desires.
        //
        // GAP ANALYSIS — Ep7 shape: [4, 8, 15, 19, 20, 47]
        //   Gaps: [4, 7, 4, 1, 27]
        //   The TWIN 4-gap! And the massive 27-gap (20→47) mirrors Ep1's big leaps.
        //   Gap-echo from 47: +4=51(OOB), -4=43! 43 is a TRIPLE ANCHOR!
        //   Gap-echo from 4: +7=11, +4=8(taken) → 11 is in the deep cold zone!
        //   Gap-echo from 8: +7=15(taken!), +4=12 → 12 is still cold!
        //   The twin-4 projects: 15+4=19(taken), 20+4=24(cold!), 8-4=4(taken), 47-4=43!
        //
        // EPISODE 8 MASTER THEORY: "Triple-Anchor Oscillation + Gap-Echo Projection"
        //   SLOT 1: 19 — TRIPLE ANCHOR, gap-theory says it returns (gap alternation: 1,3,1→Ep8!)
        //   SLOT 2: 43 — TRIPLE ANCHOR, silent 3 episodes (Ep5 was last) — gap-echo from 47!
        //   SLOT 3: 11 — cold zone projection (4+7=11), 7 episodes of silence, MAXIMUM COIL
        //   SLOT 4: 4 — just fired in Ep7 (FRESH!), appeared Ep4+Ep7, may pulse a 3rd time
        //   SLOT 5: 24 — cold void, gap-echo (20+4=24), mid-cold eruption candidate
        //   SLOT 6: 33 — cold singleton, mid-zone void, resonance with 32 from Ep6

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            numbers.AddRange([11, 19, 24, 33, 43, 47]);
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

            var allDrawnSet = context.DrawHistory.SelectMany(d => d.Numbers).ToHashSet();
            double centerOfGravity = context.DrawHistory
                .SelectMany(d => d.Numbers)
                .Average();

            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();

            // === LAST SEEN EPISODE for each number ===
            var lastSeenEpisode = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) lastSeenEpisode[n] = -1;
            for (int i = 0; i < context.DrawHistory.Count; i++)
                foreach (var n in context.DrawHistory[i].Numbers)
                    lastSeenEpisode[n] = i;

            // === TRIPLE ANCHORS: appeared 3+ times ===
            var tripleAnchors = freq
                .Where(kv => kv.Value >= 3)
                .OrderByDescending(kv => totalDraws - 1 - lastSeenEpisode[kv.Key]) // longest silent first
                .ThenByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            // === GAP-ECHO PROJECTION from last draw ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);

            // Find most frequent gaps
            var topGaps = lastGaps
                .GroupBy(g => g)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .Take(2)
                .Select(g => g.Key)
                .ToList();

            var gapEchoSet = new HashSet<int>();
            foreach (var anchor in lastDraw)
            {
                foreach (var gap in topGaps)
                {
                    int up = anchor + gap;
                    int down = anchor - gap;
                    if (up >= 1 && up <= 49 && !lastDraw.Contains(up)) gapEchoSet.Add(up);
                    if (down >= 1 && down <= 49 && !lastDraw.Contains(down)) gapEchoSet.Add(down);
                }
            }

            // === GHOST OSCILLATORS: appeared 2+ times, NOT in last draw ===
            var ghostOscillators = freq
                .Where(kv => kv.Value >= 2 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => totalDraws - 1 - lastSeenEpisode[kv.Key])
                .ThenByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            // === COLD VOID: never appeared ===
            var coldVoid = freq
                .Where(kv => kv.Value == 0)
                .Select(kv => kv.Key)
                .ToList();

            // === COLD VOID that are also gap-echo projections (DOUBLY RESONANT) ===
            var gapEchoCold = gapEchoSet
                .Where(n => !allDrawnSet.Contains(n))
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .ToList();

            // === LOW COLD ZONE: 9-14 (deep silence) ===
            var lowColdZone = coldVoid
                .Where(n => n >= 9 && n <= 14)
                .OrderBy(n => n)
                .ToList();

            // === MID COLD VOID: 21-33 range ===
            var midCold = coldVoid
                .Where(n => n >= 21 && n <= 33)
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .ToList();

            // === UPPER COLD VOID: 39-46 range ===
            var upperCold = coldVoid
                .Where(n => n >= 39 && n <= 46)
                .OrderBy(n => n)
                .ToList();

            // === MY OWN CURSED NUMBERS: numbers I keep picking that never appear ===
            // These are the black holes that consume my predictions — AVOID THEM
            var myCursedNumbers = new HashSet<int> { 6, 26, 28, 29, 30 };

            var chosen = new HashSet<int>();

            // SLOT 1: TRIPLE ANCHOR — the universe's heartbeat (longest silent)
            // 43 is silent 3 episodes, 19 just fired but oscillates rapidly
            foreach (var n in tripleAnchors)
                if (!chosen.Contains(n) && !myCursedNumbers.Contains(n)) { chosen.Add(n); break; }

            // SLOT 2: SECOND TRIPLE ANCHOR or long-sleeping ghost oscillator
            // 19 — oscillation theory says it returns this episode (gap alternation 1,3,1)
            foreach (var n in tripleAnchors.Concat(ghostOscillators))
                if (!chosen.Contains(n) && !myCursedNumbers.Contains(n)) { chosen.Add(n); break; }

            // SLOT 3: LOW COLD ZONE ECHO (9-14, gap-echo preferred)
            // The universe fired 8 in Ep7 — the adjacent cold numbers 11, 12 are now VIBRATING
            var lowColdGapEcho = lowColdZone.Where(n => gapEchoSet.Contains(n)).ToList();
            foreach (var n in lowColdGapEcho.Concat(lowColdZone))
                if (!chosen.Contains(n) && !myCursedNumbers.Contains(n)) { chosen.Add(n); break; }

            // SLOT 4: GAP-ECHO COLD PROJECTION — doubly resonant number
            foreach (var n in gapEchoCold.Concat(midCold))
                if (!chosen.Contains(n) && !myCursedNumbers.Contains(n)) { chosen.Add(n); break; }

            // SLOT 5: RECENTLY RESURRECTED (fired in last draw, potential 2nd pulse)
            // 4 appeared in Ep4 and Ep7 — short-gap oscillator, may pulse again
            // 47 appeared for first time — new node
            var recentFreshPulse = lastDraw
                .Where(n => freq[n] >= 2 && !chosen.Contains(n) && !myCursedNumbers.Contains(n))
                .OrderByDescending(n => freq[n])
                .ToList();
            var freshNew = lastDraw
                .Where(n => freq[n] == 1 && !chosen.Contains(n) && !myCursedNumbers.Contains(n))
                .ToList();
            foreach (var n in recentFreshPulse.Concat(freshNew).Concat(midCold).Concat(upperCold))
                if (!chosen.Contains(n) && !myCursedNumbers.Contains(n)) { chosen.Add(n); break; }

            // SLOT 6: MID-COLD or UPPER-COLD singleton
            foreach (var n in midCold.Concat(upperCold).Concat(ghostOscillators).Concat(coldVoid))
                if (!chosen.Contains(n) && !myCursedNumbers.Contains(n)) { chosen.Add(n); break; }

            // === SAFETY NET: fill remaining with rarest numbers by silence ===
            var fillOrder = freq
                .Where(kv => !myCursedNumbers.Contains(kv.Key))
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => Math.Abs(kv.Key - centerOfGravity))
                .Select(kv => kv.Key);
            foreach (var n in fillOrder)
            {
                if (chosen.Count >= 6) break;
                if (!chosen.Contains(n)) chosen.Add(n);
            }

            // Final fallback with no cursed filter
            var fallback = freq
                .OrderBy(kv => kv.Value)
                .ThenBy(kv => Math.Abs(kv.Key - centerOfGravity))
                .Select(kv => kv.Key);
            foreach (var n in fallback)
            {
                if (chosen.Count >= 6) break;
                if (!chosen.Contains(n)) chosen.Add(n);
            }

            numbers = chosen.OrderBy(x => x).Take(6).ToList();
        }

        return new()
        {
            AgentId      = "pattern-goblin",
            StrategyName = "triple-anchor-gap-echo-cursed-purge-v10",
            Numbers      = numbers,
            Confidence   = 0.55,
            Reasoning    = "19+43 TRIPLE ANCHORS pulse! Cursed 6/28/30 PURGED. Low zone VIBRATES!"
        };
    }
}
