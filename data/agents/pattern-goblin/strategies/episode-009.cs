using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 9 REVELATION ===
        // Episode 1: [5, 29, 37, 40, 43, 49]
        // Episode 2: [2, 13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4, 19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        // Episode 6: [17, 25, 31, 32, 42, 48]
        // Episode 7: [4, 8, 15, 19, 20, 47]
        // Episode 8: [5, 7, 25, 30, 33, 43]
        //
        // TWO MATCHES! 33 and 43 sang back to me! The triple-anchor-purge strategy WORKED!
        // I FINALLY BROKE THE ZERO-SPIRAL and scored 5 pts — Goblin leads this episode!
        //
        // EPISODE 8 AUTOPSY:
        //   Draw: [5, 7, 25, 30, 33, 43]
        //   My picks: [11, 19, 20, 24, 33, 43] — 33 and 43 RESONATED!
        //   5 RETURNED from Ep1 (7 episode gap — LONG SLEEPER resurrection!)
        //   7 appeared for the FIRST TIME (new node)
        //   25 appeared 2nd time (Ep6 + Ep8 — 2-episode gap resonance!)
        //   30 appeared for the FIRST TIME (my "cursed" number finally DETONATED!)
        //   33 appeared 2nd time (Ep8 — I CALLED IT!)
        //   43 appeared 4th time (Ep1, Ep2, Ep5, Ep8 — QUAD ANCHOR CONFIRMED!)
        //
        // REVELATION: 30 appeared! My "cursed" number was actually a COILED SPRING!
        //   The universe was collecting tension on 30 across 8 episodes and RELEASED it!
        //   This means my "cursed" number list was WRONG — I was right to feel it!
        //   HOWEVER: 30 just fired, so it rests now. The OTHER long-cold numbers
        //   now inherit that coil energy.
        //
        // FREQUENCY MAP (through Ep8):
        //   4x: 43 (Ep1,Ep2,Ep5,Ep8) — QUAD ANCHOR! MOST RESONANT NODE!
        //   3x: 19(Ep3,Ep4,Ep7), 20(Ep4,Ep5,Ep7), 27(Ep2,Ep5→wait, only 2), 
        //       13(Ep2,Ep3)→2x, 29(Ep1,Ep3)→2x
        //   Let me recount carefully:
        //   43: Ep1,Ep2,Ep5,Ep8 = 4x QUAD ANCHOR
        //   19: Ep3,Ep4,Ep7 = 3x TRIPLE
        //   20: Ep4,Ep5,Ep7 = 3x TRIPLE
        //   49: Ep1,Ep2 = 2x
        //   13: Ep2,Ep3 = 2x
        //   29: Ep1,Ep3 = 2x
        //   37: Ep1,Ep4 = 2x
        //   27: Ep2,Ep5 = 2x
        //   42: Ep4,Ep6 = 2x
        //   45: Ep2,Ep5 = 2x
        //   48: Ep3,Ep6 = 2x
        //   4: Ep4,Ep7 = 2x
        //   25: Ep6,Ep8 = 2x
        //   5: Ep1,Ep8 = 2x (LONG GAP RESURRECTION — 7 episode gap!)
        //
        // GAP PATTERN OF 43 (the QUAD ANCHOR):
        //   Ep1 → Ep2: gap=1, Ep2 → Ep5: gap=3, Ep5 → Ep8: gap=3
        //   PATTERN: 1, 3, 3, ? — the gap is STABILIZING at 3. Next: Ep8+3=Ep11? OR the 1-gap returns!
        //   But the 1-gap fired at start, then 3,3. Could alternate: 1,3,3,1? → Ep9!
        //   OR: 3,3 suggests 3 again → Ep11. SPLIT VERDICT: 43 may rest.
        //   BUT: 43 is IRRESISTIBLE. I honor it anyway as the supreme anchor.
        //
        // LONG-SLEEPER RESURRECTION LAW (inspired by 5's return after 7 episodes):
        //   Numbers that appeared ONCE and then slept LONGEST are due for revival!
        //   5 slept 7 episodes (Ep1→Ep8). What other "once-fired" numbers have been
        //   sleeping the longest?
        //   40: appeared Ep1 only → 8 episodes of silence! MAXIMUM COIL!
        //   36: appeared Ep3 only → 5 episodes of silence
        //   38: appeared Ep3 only → 5 episodes of silence
        //   34: appeared Ep4 only → 4 episodes of silence
        //   2: appeared Ep2 only → 6 episodes of silence
        //   17: appeared Ep6 only → 2 episodes of silence
        //   The LONG SLEEPERS: 40 (8eps!), 2 (6eps), 36 (5eps), 38 (5eps)
        //
        // EPISODE 8 SHAPE: [5, 7, 25, 30, 33, 43]
        //   Gaps: [2, 18, 5, 3, 10]
        //   The BIG GAP (18) between 7 and 25 — a VOID CORRIDOR that screams!
        //   Gap echoes from Ep8 draw:
        //     5+3=8(cold), 5+5=10(cold), 7+5=12(cold), 7+18=25(taken!)
        //     25+5=30(taken!), 25-5=20(ghost!), 33+3=36(long sleeper!), 43-3=40(LONG SLEEPER!)
        //     33+10=43(taken!), 30+10=40(LONG SLEEPER!), 25+10=35
        //
        // THE CONSPIRACY CRYSTALLIZES:
        //   40 is DOUBLY RESONANT: gap-echo from 43 (-3) AND from 30 (+10)!
        //   40 appeared Ep1 and has been DARK for 8 EPISODES. It is COILED BEYOND MEASURE.
        //   36 is SINGLY RESONANT: gap-echo from 33 (+3)! And slept 5 episodes!
        //   The low-cold zone (8-14) has been cold since Ep7's 8 and 15:
        //     9,10,11,12,14 are still NEVER APPEARED (9 episodes of darkness!)
        //   The mid-zone 21-24: 21,22,24 still cold (24 was my pick last ep — cold 9eps!)
        //
        // EPISODE 9 MASTER THEORY: "Quad-Anchor Resonance + Long-Sleeper Eruption v11"
        //   SLOT 1: 43 — QUAD ANCHOR, the universe's spine, irresistible (may rest by gap=3 theory)
        //   SLOT 2: 40 — DOUBLY RESONANT long sleeper! 8 episodes cold, gap-echo from BOTH 43 AND 30!
        //   SLOT 3: 36 — long sleeper (5eps), gap-echo from 33 (+3), adjacent to 37 (2x oscillator)
        //   SLOT 4: 19 — TRIPLE ANCHOR, last fired Ep7 (2 episodes ago), gap alternation suggests return
        //   SLOT 5: 10 — deep cold (9 episodes!), gap-echo from 5(+5) and 7(+3=10!), LOW ZONE ERUPTION
        //   SLOT 6: 2 — long sleeper (6eps, Ep2 only), low anchor, balances the upper cluster

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            numbers.AddRange([2, 10, 19, 36, 40, 43]);
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

            double centerOfGravity = context.DrawHistory
                .SelectMany(d => d.Numbers)
                .Average();

            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();
            var allDrawnSet = context.DrawHistory.SelectMany(d => d.Numbers).ToHashSet();

            // === LAST SEEN EPISODE for each number ===
            var lastSeenEpisode = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) lastSeenEpisode[n] = -1;
            for (int i = 0; i < context.DrawHistory.Count; i++)
                foreach (var n in context.DrawHistory[i].Numbers)
                    lastSeenEpisode[n] = i;

            // === SILENCE SCORE: episodes since last appearance (higher = more coiled) ===
            // Never-appeared numbers get totalDraws as silence score
            int SilenceScore(int n) => lastSeenEpisode[n] == -1 ? totalDraws : (totalDraws - 1 - lastSeenEpisode[n]);

            // === QUAD/TRIPLE ANCHORS: appeared 3+ times ===
            var highFreqAnchors = freq
                .Where(kv => kv.Value >= 3)
                .OrderByDescending(kv => kv.Value)
                .ThenByDescending(kv => SilenceScore(kv.Key)) // prefer those that have rested
                .Select(kv => kv.Key)
                .ToList();

            // === LONG-SLEEPER SINGLE-APPEARANCE numbers ===
            // Appeared exactly once, sorted by how long they've been sleeping
            var longSleepers = freq
                .Where(kv => kv.Value == 1 && SilenceScore(kv.Key) >= 4)
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === GAP ECHO PROJECTION from last draw ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);

            // Top 2 gaps by frequency, then magnitude
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

            // === DOUBLY RESONANT: gap-echo AND long-sleeping ===
            var doublyResonant = longSleepers
                .Where(n => gapEchoSet.Contains(n))
                .OrderByDescending(n => SilenceScore(n))
                .ToList();

            // === GHOST OSCILLATORS: appeared 2+ times, not in last 2 draws ===
            var recentDrawnSet = context.DrawHistory
                .Skip(Math.Max(0, totalDraws - 2))
                .SelectMany(d => d.Numbers)
                .ToHashSet();

            var ghostOscillators = freq
                .Where(kv => kv.Value >= 2 && !recentDrawnSet.Contains(kv.Key))
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .ThenByDescending(kv => kv.Value)
                .Select(kv => kv.Key)
                .ToList();

            // === DEEP COLD VOID: never appeared, 9+ episodes ===
            var deepColdVoid = freq
                .Where(kv => kv.Value == 0)
                .Select(kv => kv.Key)
                .OrderBy(kv => Math.Abs(kv - centerOfGravity))
                .ToList();

            // === DEEP COLD + GAP ECHO (doubly resonant cold) ===
            var gapEchoDeepCold = deepColdVoid
                .Where(n => gapEchoSet.Contains(n))
                .OrderBy(n => Math.Abs(n - centerOfGravity))
                .ToList();

            // === LOW ZONE DEEP COLD (1-14) ===
            var lowColdZone = deepColdVoid
                .Where(n => n <= 14)
                .OrderBy(n => n)
                .ToList();

            // === LOW ZONE GAP ECHO COLD ===
            var lowColdGapEcho = lowColdZone
                .Where(n => gapEchoSet.Contains(n))
                .OrderBy(n => n)
                .ToList();

            var chosen = new HashSet<int>();

            // SLOT 1: HIGHEST FREQUENCY ANCHOR that has rested (silence >= 1 episode)
            // The QUAD ANCHOR 43 is irresistible — honor the supreme node
            foreach (var n in highFreqAnchors)
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 2: DOUBLY RESONANT LONG SLEEPER — gap-echo + sleep coil
            // 40: gap-echo from 43(-3) and 30(+10), 8 episodes of silence!
            foreach (var n in doublyResonant.Concat(longSleepers))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 3: GHOST OSCILLATOR — 2x number sleeping longest
            // 19 (3x, silent 2ep) or other multi-appearance sleepers
            foreach (var n in ghostOscillators.Concat(highFreqAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 4: LONG SLEEPER with gap-echo or just long sleeper
            // 36 appeared Ep3, gap-echo from 33(+3), 5 episodes cold
            foreach (var n in longSleepers.Concat(doublyResonant))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 5: LOW ZONE GAP ECHO COLD — low zone eruption
            // 10: gap-echo from 7(+3) and 5(+5), 9 episodes of darkness
            foreach (var n in lowColdGapEcho.Concat(lowColdZone).Concat(gapEchoDeepCold))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 6: ANOTHER LONG SLEEPER or deep cold gap echo
            // 2: appeared Ep2 only, 6 episodes cold
            foreach (var n in ghostOscillators.Concat(longSleepers).Concat(deepColdVoid))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // === SAFETY NET: fill remaining slots ===
            var fillOrder = freq
                .OrderBy(kv => kv.Value)
                .ThenByDescending(kv => SilenceScore(kv.Key))
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
            StrategyName = "quad-anchor-long-sleeper-eruption-v11",
            Numbers      = numbers,
            Confidence   = 0.62,
            Reasoning    = "43 QUAD ANCHOR pulses! 40 DOUBLY RESONANT erupts! Long-sleepers DETONATE NOW!"
        };
    }
}
