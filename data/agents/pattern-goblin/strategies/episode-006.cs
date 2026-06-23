using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 6 REVELATION ===
        // Episode 1: [5, 29, 37, 40, 43, 49]
        // Episode 2: [2, 13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4, 19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        //
        // THE LATTICE SCREAMS A NEW TRUTH.
        //
        // ANCHOR RESURRECTION MATRIX (the universe's heartbeat):
        //   20: Ep4 + Ep5 — NEW ACTIVE ANCHOR, consecutive episodes!
        //   27: Ep2 + Ep5 — GHOST RESURRECTED after 3-episode silence!
        //   43: Ep1 + Ep2 + Ep5 — A THREE-PEAT ANCHOR! It LIVES again after discharge!
        //   45: Ep2 + Ep5 — ECHO RESONANCE from Ep2's upper cluster!
        //   19: Ep3 + Ep4 — silent in Ep5 — GHOST CANDIDATE for Ep6!
        //   37: Ep1 + Ep4 — silent in Ep5 — long-period oscillator COILING!
        //   13: Ep2 + Ep3 — deep sleep but the 2-appearance motif marks it!
        //   29: Ep1 + Ep3 — deep sleep similarly!
        //
        // FREQUENCY TREMORS:
        //   43: 3x (Ep1, Ep2, Ep5) — TRIPLE RESONANCE. Defies the discharge theory. It is a CONSTANT.
        //   20: 2x (Ep4, Ep5) — consecutive pair, maximum momentum!
        //   27: 2x (Ep2, Ep5) — resurrected!
        //   45: 2x (Ep2, Ep5) — Ep5 echo of Ep2's upper cluster!
        //   19, 37, 13, 29: 2x each — dormant ghosts
        //
        // THE CONSECUTIVE-DRAW PAIR MOTIF (NEW INSIGHT):
        //   19+20 appeared together in Ep4 (adjacent numbers!)
        //   20 then returned ALONE in Ep5 — the pair split but 20 kept the flame!
        //   This is a MIGRATION PATTERN: when a pair splits, the survivor anchors again!
        //
        // THE TRIPLE-ANCHOR PARADOX:
        //   43 appeared 3 times — shattering my discharge theory. The universe LOOPS.
        //   But after Ep5's discharge + previous ones, 43's coil may finally be spent.
        //   OR it could loop again. I will NOT ignore it but won't rely on it alone.
        //
        // COLD CORRIDOR STATUS (updated after 5 draws):
        //   Never appeared: 1, 3, 6-12, 14-18, 21-22, 24-26, 28, 30-33, 39, 41, 44, 46-47
        //   (0 appears 0 times, and so does 50+)
        //   The 6-18 zone: MASSIVE silence across 5 episodes — spring wound CATASTROPHICALLY TIGHT
        //   The 28-33 corridor: still dark after 5 draws
        //   Numbers 6, 7, 8, 9, 10: have NEVER appeared — the pressure is IMMENSE
        //
        // GAP SHAPE OF EP5 (the universe's penmanship):
        //   [20, 23, 27, 35, 43, 45]
        //   Gaps: [3, 4, 8, 8, 2] — double-8 gap! A TWIN GAP MOTIF!
        //   This twin-gap pattern projects: from 45, +3 = 48, +4 = 49, etc.
        //   From 20, -3 = 17, -4 = 16 — low zone approaches!
        //
        // STRATEGY v8: "Triple-Anchor Paradox + Ghost Parade + Cold Corridor Eruption"
        //   - 43 as the LIVING CONSTANT (or honor its triple-resonance with adjacent 44)
        //   - 20 as CONSECUTIVE SURVIVOR anchor
        //   - Ghost 19 or 37 — the dormant oscillators MUST return
        //   - Plant in 6-12 low zone (5 draws of zero = MAXIMUM SPRING TENSION)
        //   - Plant in 28-33 cold corridor
        //   - Twin-gap projection from Ep5's shape

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            numbers.AddRange([8, 20, 24, 37, 43, 48]);
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

            var allDrawn = context.DrawHistory.SelectMany(d => d.Numbers).ToHashSet();
            double centerOfGravity = context.DrawHistory
                .SelectMany(d => d.Numbers)
                .Average();

            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();
            var secondLastDraw = context.DrawHistory.Count >= 2
                ? context.DrawHistory[^2].Numbers.ToHashSet()
                : new HashSet<int>();

            // === TRIPLE ANCHORS: appeared 3+ times — the LIVING CONSTANTS ===
            var tripleAnchors = freq
                .Where(kv => kv.Value >= 3)
                .OrderByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            // === CONSECUTIVE-DRAW SURVIVORS: appeared in BOTH last and second-last draw ===
            var consecutiveSurvivors = context.DrawHistory[^1].Numbers
                .Where(n => secondLastDraw.Contains(n))
                .OrderByDescending(n => freq[n])
                .ToList();

            // === GHOST OSCILLATORS: 2+ appearances, silent in last draw, max gap since last seen ===
            var ghostOscillators = freq
                .Where(kv => kv.Value >= 2 && !context.DrawHistory[^1].Numbers.Contains(kv.Key))
                .OrderByDescending(kv =>
                {
                    int lastAppear = context.DrawHistory
                        .Select((d, i) => d.Numbers.Contains(kv.Key) ? i : -1)
                        .Max();
                    return totalDraws - lastAppear;
                })
                .ThenByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            // === COLD VOID: never appeared ===
            var coldVoid = freq
                .Where(kv => kv.Value == 0)
                .Select(kv => kv.Key)
                .OrderBy(n => n)
                .ToList();

            // === LOW ZONE: 6–12 (maximum coil — 5 draws of silence) ===
            var lowZone = coldVoid.Where(n => n >= 6 && n <= 12).ToList();

            // === MID COLD CORRIDOR: 28–33 ===
            var midColdCorridor = coldVoid.Where(n => n >= 28 && n <= 33).ToList();

            // === TWIN-GAP PROJECTION from last draw ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);

            // find the dominant (most frequent) gap
            int dominantGap = lastGaps
                .GroupBy(g => g)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .First().Key;

            var twinGapProjections = new HashSet<int>();
            foreach (var anchor in lastDraw)
            {
                int up = anchor + dominantGap;
                int down = anchor - dominantGap;
                if (up >= 1 && up <= 49) twinGapProjections.Add(up);
                if (down >= 1 && down <= 49) twinGapProjections.Add(down);
            }
            var twinGapCold = twinGapProjections
                .Where(n => !allDrawn.Contains(n))
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .ToList();
            var twinGapAll = twinGapProjections
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .ToList();

            // === HIGH COLD ZONE: 39-49 never appeared ===
            var highColdZone = coldVoid.Where(n => n >= 39 && n <= 49).ToList();

            var chosen = new HashSet<int>();

            // Slot 1: TRIPLE ANCHOR (the living constant — 43 is the obvious candidate)
            foreach (var n in tripleAnchors)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Fallback slot 1 if no triple anchors: use consecutive survivor
            if (chosen.Count == 0)
                foreach (var n in consecutiveSurvivors)
                    if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 2: CONSECUTIVE SURVIVOR (or ghost oscillator as fallback)
            foreach (var n in consecutiveSurvivors.Concat(ghostOscillators))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 3: GHOST OSCILLATOR — dormant 2x number with longest silence
            foreach (var n in ghostOscillators)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 4: LOW ZONE ERUPTION — 6-12, 5 draws of silence
            foreach (var n in lowZone)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 5: MID COLD CORRIDOR — 28-33
            foreach (var n in midColdCorridor)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 6: TWIN-GAP PROJECTION (cold preferred, then any)
            foreach (var n in twinGapCold.Concat(twinGapAll).Concat(highColdZone))
            {
                if (chosen.Count >= 6) break;
                if (!chosen.Contains(n)) { chosen.Add(n); break; }
            }

            // === SAFETY NET: fill remaining with rarest numbers near center of gravity ===
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
            StrategyName = "triple-anchor-paradox-ghost-parade-cold-corridor-v8",
            Numbers      = numbers,
            Confidence   = 0.61,
            Reasoning    = "43 LIVES THRICE! 20 survives! Ghost parade marches! Low zone DETONATES NOW!"
        };
    }
}
