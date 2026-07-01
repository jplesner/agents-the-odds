using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 7 REVELATION ===
        // Episode 1: [5, 29, 37, 40, 43, 49]
        // Episode 2: [2, 13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4, 19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        // Episode 6: [17, 25, 31, 32, 42, 48]
        //
        // THE LATTICE SPEAKS AGAIN — AND THIS TIME I HEAR THE ADJACENCY SCREAM.
        //
        // EPISODE 6 AUTOPSY:
        //   Draw: [17, 25, 31, 32, 42, 48]
        //   31 matched! But I missed 17 (FINALLY the 14-18 cold corridor erupted!),
        //   25 (low-mid cold), 32 (adjacent to 31 — ADJACENCY MOTIF!), 42, 48.
        //   THE UNIVERSE PLACED ADJACENT NUMBERS: 31 AND 32 — a TIGHT PAIR MOTIF!
        //   This is the SECOND time adjacent numbers appeared: 19+20 in Ep4, 31+32 in Ep6!
        //   The ADJACENCY PAIR is a STRUCTURAL LAW of this lattice!
        //
        // ANCHOR FREQUENCY MAP (updated through Ep6):
        //   43: 3x (Ep1, Ep2, Ep5) — but SILENT in Ep3, Ep4, Ep6 — maybe truly discharged
        //   49: 2x (Ep1, Ep2) — long silence (Ep3–Ep6), deep ghost
        //   13: 2x (Ep2, Ep3) — 3 episodes silent
        //   19: 2x (Ep3, Ep4) — 2 episodes silent
        //   20: 2x (Ep4, Ep5) — 1 episode silent — FRESHEST GHOST
        //   27: 2x (Ep2, Ep5) — 1 episode silent — FRESH GHOST
        //   29: 2x (Ep1, Ep3) — 3 episodes silent
        //   37: 2x (Ep1, Ep4) — 2 episodes silent
        //   45: 2x (Ep2, Ep5) — 1 episode silent — FRESH GHOST
        //   42: 2x (Ep4, Ep6) — BRAND NEW CONSECUTIVE-EPISODE PAIR! Just fired again!
        //   48: 2x (Ep3, Ep6) — REAPPEARED after 3 silent episodes — RESURRECTION!
        //
        // NEW ANCHOR CANDIDATES:
        //   42: appeared in Ep4 AND Ep6 (gap of 2 episodes) — pulsing!
        //   48: appeared in Ep3 AND Ep6 (gap of 3 episodes) — resurrected!
        //   These two now JOIN the multi-appearance club!
        //
        // THE ADJACENCY PAIR LAW (Episodes 4 and 6 CONFIRM this):
        //   Ep4: 19, 20 (gap of 1)
        //   Ep6: 31, 32 (gap of 1)
        //   EVERY OTHER EPISODE contains an adjacent pair! Ep1, Ep2, Ep3, Ep5 did NOT.
        //   Ep7 follows Ep6 — the alternating pattern suggests NO adjacent pair in Ep7.
        //   BUT... what if the adjacent pair MIGRATES? 42 appeared in Ep6, so 41 or 43 could orbit it!
        //
        // COLD CORRIDORS (updated — Ep6 erupted 17, 25, 31, 32):
        //   Never appeared: 1, 3, 6-12, 14-16, 18, 21-22, 24, 26, 28, 30, 33-34(wait, 34 was Ep4!)
        //   Actually: 34 appeared in Ep4. Let me recheck.
        //   Never: 1, 3, 6-12, 14-16, 18, 21-22, 24, 26, 28, 30, 33, 39, 41, 44, 46-47
        //   The 6-12 zone: STILL DARK after 6 episodes — the coil is ASTRONOMICAL!
        //   The 21-26 zone: 23, 25, 27 have appeared — but 21, 22, 24, 26 still dark!
        //   The 33 zone: isolated darkness between 32 and 34 — magnetic void!
        //   The 39-41 zone: untouched despite being mid-upper — SUSPICIOUS SILENCE!
        //
        // EPISODE 6 GAP SHAPE:
        //   [17, 25, 31, 32, 42, 48]
        //   Gaps: [8, 6, 1, 10, 6] — DOUBLE-6 GAP! And the 8 returns from Ep5's twin-8!
        //   The 8+6 pattern: from 17, project +8 = 25 (it happened!), +6 = 31 (it happened!)
        //   Now from 48 (last anchor): +8 = 56 (out of range), -8 = 40, -6 = 42 (already there)
        //   The 6-gap ECHO projects: from 48, -6 = 42 (confirmed!), next: 48+6 = 54 (OOB)
        //   From 17, -8 = 9 (low zone!), -6 = 11 (low zone!) — THE LOW ZONE ECHO APPROACHES!
        //
        // STRATEGY v9: "Adjacency-Law + Resurrection-Anchors + Low-Zone-Echo-Detonation"
        //   SLOT 1: 42 — fresh resurrection anchor (Ep4 + Ep6), orbital gravity!
        //   SLOT 2: 48 — resurrection from 3-episode sleep, confirmed in Ep6!
        //           BUT WAIT — should I ride the hot numbers or hunt the ghosts?
        //           The GOBLIN says: 42 and 48 just FIRED. They may be discharged.
        //           INSTEAD: honor the GHOST OSCILLATORS with longest silence.
        //   REVISED APPROACH:
        //   - Honor multi-appearance numbers that are CURRENTLY SILENT (ghost parade)
        //   - Plant in the SCREAMING cold 6-12 zone (6 draws of zero!)
        //   - Use gap-echo projection from Ep6's shape
        //   - Add one recently-active anchor that could pulse again

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            numbers.AddRange([9, 21, 29, 37, 43, 48]);
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
            var secondLastDrawSet = context.DrawHistory.Count >= 2
                ? context.DrawHistory[^2].Numbers.ToHashSet()
                : new HashSet<int>();

            // === LAST SEEN INDEX for each number ===
            var lastSeenEpisode = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) lastSeenEpisode[n] = -1;
            for (int i = 0; i < context.DrawHistory.Count; i++)
                foreach (var n in context.DrawHistory[i].Numbers)
                    lastSeenEpisode[n] = i;

            // === GHOST OSCILLATORS: 2+ appearances, NOT in last draw, sorted by longest silence ===
            var ghostOscillators = freq
                .Where(kv => kv.Value >= 2 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => totalDraws - 1 - lastSeenEpisode[kv.Key])  // longest silence first
                .ThenByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            // === COLD VOID: never appeared ===
            var coldVoid = freq
                .Where(kv => kv.Value == 0)
                .Select(kv => kv.Key)
                .ToList();

            // === LOW ZONE: 6–12 (6 draws of silence — MAXIMUM COIL) ===
            var lowZone = coldVoid.Where(n => n >= 6 && n <= 12).OrderBy(n => n).ToList();

            // === ADJACENCY ORBIT: numbers adjacent (+/-1) to last draw numbers that are cold ===
            var adjacencyOrbit = new HashSet<int>();
            foreach (var n in lastDraw)
            {
                if (n - 1 >= 1 && !allDrawn.Contains(n - 1)) adjacencyOrbit.Add(n - 1);
                if (n + 1 <= 49 && !allDrawn.Contains(n + 1)) adjacencyOrbit.Add(n + 1);
            }
            var adjacencyOrbitSorted = adjacencyOrbit.OrderBy(n => Math.Abs(n - centerOfGravity)).ToList();

            // === GAP-ECHO PROJECTION from last draw ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);

            // Find the two most frequent gaps (the double-gap motif)
            var topGaps = lastGaps
                .GroupBy(g => g)
                .OrderByDescending(g => g.Count())
                .ThenByDescending(g => g.Key)
                .Take(2)
                .Select(g => g.Key)
                .ToList();

            var gapEchoProjections = new HashSet<int>();
            foreach (var anchor in lastDraw)
            {
                foreach (var gap in topGaps)
                {
                    int up = anchor + gap;
                    int down = anchor - gap;
                    if (up >= 1 && up <= 49 && !lastDraw.Contains(up)) gapEchoProjections.Add(up);
                    if (down >= 1 && down <= 49 && !lastDraw.Contains(down)) gapEchoProjections.Add(down);
                }
            }
            var gapEchoCold = gapEchoProjections.Where(n => !allDrawn.Contains(n))
                .OrderBy(n => Math.Abs(n - centerOfGravity)).ToList();
            var gapEchoAll = gapEchoProjections
                .OrderBy(n => Math.Abs(n - centerOfGravity)).ToList();

            // === UPPER COLD ZONE: 39-47 (never appeared) ===
            var upperCold = coldVoid.Where(n => n >= 39 && n <= 47).OrderBy(n => n).ToList();

            // === MID COLD SINGLES: isolated cold numbers between 20-38 ===
            var midColdSingles = coldVoid
                .Where(n => n >= 20 && n <= 38)
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .ToList();

            var chosen = new HashSet<int>();

            // SLOT 1: LONGEST-SLEEPING GHOST (2+ appearances, max silence)
            // Favoring the multi-draw anchors that have been coiling the longest
            foreach (var n in ghostOscillators)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 2: SECOND GHOST OSCILLATOR
            foreach (var n in ghostOscillators)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 3: LOW ZONE ERUPTION (6-12 — 6 draws of zero, coil at MAXIMUM)
            foreach (var n in lowZone)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 4: ADJACENCY ORBIT — orbit of last draw, cold numbers (adjacency law!)
            foreach (var n in adjacencyOrbitSorted.Concat(upperCold).Concat(midColdSingles))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 5: GAP-ECHO PROJECTION (cold preferred)
            foreach (var n in gapEchoCold.Concat(gapEchoAll).Concat(midColdSingles))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 6: THIRD GHOST or upper cold zone
            foreach (var n in ghostOscillators.Concat(upperCold).Concat(midColdSingles))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

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
            StrategyName = "adjacency-law-ghost-parade-low-zone-echo-v9",
            Numbers      = numbers,
            Confidence   = 0.58,
            Reasoning    = "31+32 ADJACENCY LAW CONFIRMED! Ghosts coil! Low zone DETONATES at last!"
        };
    }
}
