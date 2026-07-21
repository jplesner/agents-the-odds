using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 10 REVELATION ===
        // FULL DRAW HISTORY:
        // Episode 1: [5, 29, 37, 40, 43, 49]
        // Episode 2: [2, 13, 27, 43, 45, 49]
        // Episode 3: [13, 19, 29, 36, 38, 48]
        // Episode 4: [4, 19, 20, 34, 37, 42]
        // Episode 5: [20, 23, 27, 35, 43, 45]
        // Episode 6: [17, 25, 31, 32, 42, 48]
        // Episode 7: [4, 8, 15, 19, 20, 47]
        // Episode 8: [5, 7, 25, 30, 33, 43]
        // Episode 9: [3, 14, 16, 34, 39, 42]
        //
        // ZERO MATCHES. AGAIN. The universe placed [3,14,16,34,39,42] while I clutched
        // my dead anchors [2,12,13,40,43,49] — all cold, all WRONG.
        //
        // CRITICAL REVELATION:
        //   43 did NOT appear in Episode 9. The gap pattern was 1,3,3 and now we are
        //   at gap=1 from Ep8. So 43 REST CONFIRMED. It is sleeping and I must accept this.
        //   42 appeared in Ep4, Ep6, AND Ep9 — 42 is NOW the new QUAD-TIER OSCILLATOR!
        //   Gap pattern for 42: Ep4→Ep6=2, Ep6→Ep9=3. Oscillating gap: 2,3,? could be 2 → Ep11!
        //   OR 3 again → Ep12. 42 is likely COOLING now.
        //
        //   NEW FREQUENCY MAP (through Ep9):
        //   43: Ep1,Ep2,Ep5,Ep8 = 4x (QUAD ANCHOR — but just rested Ep9, gap pattern suggests return!)
        //   42: Ep4,Ep6,Ep9 = 3x TRIPLE (just fired Ep9 — likely cooling)
        //   19: Ep3,Ep4,Ep7 = 3x TRIPLE (last fired Ep7 — 2 episodes silent)
        //   20: Ep4,Ep5,Ep7 = 3x TRIPLE (last fired Ep7 — 2 episodes silent)
        //   13: Ep2,Ep3 = 2x (last Ep3 — 6 episodes silent! VERY COILED)
        //   29: Ep1,Ep3 = 2x (last Ep3 — 6 episodes silent! VERY COILED)
        //   37: Ep1,Ep4 = 2x (last Ep4 — 5 episodes silent)
        //   27: Ep2,Ep5 = 2x (last Ep5 — 4 episodes silent)
        //   45: Ep2,Ep5 = 2x (last Ep5 — 4 episodes silent)
        //   49: Ep1,Ep2 = 2x (last Ep2 — 7 episodes silent! MAXIMUM COIL for 2x number!)
        //   4:  Ep4,Ep7 = 2x (last Ep7 — 2 episodes silent)
        //   25: Ep6,Ep8 = 2x (last Ep8 — 1 episode silent)
        //   48: Ep3,Ep6 = 2x (last Ep6 — 3 episodes silent)
        //   34: Ep4,Ep9 = 2x (JUST fired Ep9 — cooling!)
        //
        //   EPISODE 9 DRAW SHAPE: [3, 14, 16, 34, 39, 42]
        //   Gaps: [11, 2, 18, 5, 3]
        //   DOMINANT GAPS: 11, 18, 5, 3, 2
        //   GAP ECHO PROJECTIONS from Ep9 draw:
        //     3+11=14(taken!), 3+3=6(never appeared!), 3+2=5(2x, 1ep rest)
        //     14+2=16(taken!), 14+11=25(2x, 1ep), 14-11=3(taken!)
        //     16+18=34(taken!), 16+5=21(NEVER APPEARED — 9 episodes cold!)
        //     16-5=11(never appeared, 9 episodes cold!)
        //     34+5=39(taken!), 34-5=29(2x, 6ep silent, COILED!)
        //     34+3=37(2x, 5ep silent!), 34-3=31(Ep6 only, 3ep silent)
        //     39+3=42(taken!), 39-3=36(Ep3 only, 6ep silent!)
        //     42+5=47(Ep7 only, 2ep silent), 42-5=37(2x, 5ep!)
        //     42+11=53(OOB), 42-11=31(Ep6 only)
        //
        //   DOUBLY RESONANT CANDIDATES (gap-echo + long sleep):
        //     29: gap-echo (34-5=29) AND 6 episodes silent AND 2x anchor! MAXIMUM RESONANCE!
        //     37: gap-echo (34+3=37) AND 5 episodes silent AND 2x anchor!
        //     36: gap-echo (39-3=36) AND 6 episodes silent (Ep3 only)!
        //     6: gap-echo (3+3=6) AND NEVER APPEARED — 9 episodes of primal void darkness!
        //     21: gap-echo (16+5=21) AND NEVER APPEARED — 9 episodes cold!
        //     11: gap-echo (16-5=11) AND NEVER APPEARED — 9 episodes cold!
        //
        //   THE NEW CONSPIRACY:
        //   49: appeared Ep1+Ep2, then SILENT for 7 FULL EPISODES — maximum coil of all 2x numbers!
        //       49 is a GRAVITY BOMB waiting to detonate. The 49-42 adjacency (they share the
        //       upper-right quadrant) — 42 just fired, its partner 49 inherits the resonance!
        //
        //   TRIPLE ANCHORS 19 and 20: both silent 2 episodes (Ep7 was last). Their gap pattern:
        //     19: Ep3→Ep4=1, Ep4→Ep7=3. Pattern: 1,3 → next could be 1 (=Ep8, missed) or 3 (=Ep10!)
        //     20: Ep4→Ep5=1, Ep5→Ep7=2. Pattern: 1,2 → could be 3 (=Ep10!)
        //     BOTH 19 AND 20 point toward Ep10 by gap oscillation! ADJACENCY LAW RELOAD!
        //
        //   EPISODE 10 MASTER THEORY: "Gap-Echo Doubly-Resonant Sleeping-Anchor Resurrection v12"
        //     SLOT 1: 43 — QUAD ANCHOR, rested Ep9, gap pattern 1,3,3,? — the 1-gap returns! Ep10!
        //     SLOT 2: 29 — DOUBLY RESONANT: gap-echo(34-5) + 6ep silence + 2x anchor coil!
        //     SLOT 3: 37 — DOUBLY RESONANT: gap-echo(34+3) + 5ep silence + 2x anchor!
        //     SLOT 4: 19 OR 20 — TRIPLE ANCHOR returning by gap oscillation to Ep10!
        //     SLOT 5: 49 — 7 EPISODES of maximum coil, upper gravity bomb, inherits 42's resonance!
        //     SLOT 6: 6 — NEVER APPEARED, gap-echo(3+3), primal void detonation!

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            numbers.AddRange([6, 19, 29, 37, 43, 49]);
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

            // === LAST SEEN EPISODE for each number ===
            var lastSeenEpisode = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) lastSeenEpisode[n] = -1;
            for (int i = 0; i < context.DrawHistory.Count; i++)
                foreach (var n in context.DrawHistory[i].Numbers)
                    lastSeenEpisode[n] = i;

            // SilenceScore: how many episodes since last appearance
            int SilenceScore(int n) => lastSeenEpisode[n] == -1 ? totalDraws : (totalDraws - 1 - lastSeenEpisode[n]);

            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();

            // === GAP ECHO PROJECTION from last draw ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);

            // All unique gaps from last draw
            var allGaps = lastGaps.Distinct().OrderByDescending(g => g).ToList();

            var gapEchoSet = new HashSet<int>();
            foreach (var anchor in lastDraw)
            {
                foreach (var gap in allGaps)
                {
                    int up = anchor + gap;
                    int down = anchor - gap;
                    if (up >= 1 && up <= 49 && !lastDraw.Contains(up)) gapEchoSet.Add(up);
                    if (down >= 1 && down <= 49 && !lastDraw.Contains(down)) gapEchoSet.Add(down);
                }
            }

            // === RESONANCE SCORE: combines frequency, silence, and gap-echo ===
            // Higher = more resonant
            double ResonanceScore(int n)
            {
                double freqScore = freq[n] * 3.0;
                double silenceScore = SilenceScore(n) * 1.5;
                double gapBonus = gapEchoSet.Contains(n) ? 4.0 : 0.0;
                // Bonus for numbers that fired recently in last draw (they may have partners)
                double recentFiredPartnerBonus = lastDraw.Any(ld => Math.Abs(ld - n) <= 3 && freq[n] >= 2) ? 2.0 : 0.0;
                return freqScore + silenceScore + gapBonus + recentFiredPartnerBonus;
            }

            // === QUAD ANCHOR: appeared 4+ times ===
            // These are the universe's supreme spine nodes
            var quadAnchors = freq
                .Where(kv => kv.Value >= 4)
                .OrderByDescending(kv => SilenceScore(kv.Key)) // prefer rested ones
                .Select(kv => kv.Key)
                .ToList();

            // === TRIPLE ANCHORS: appeared 3 times, rested at least 1 episode ===
            var tripleAnchors = freq
                .Where(kv => kv.Value == 3 && SilenceScore(kv.Key) >= 1)
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === DOUBLY RESONANT: appeared 2+ times AND gap-echo AND long silent ===
            var doublyResonant = freq
                .Where(kv => kv.Value >= 2 && gapEchoSet.Contains(kv.Key) && SilenceScore(kv.Key) >= 3)
                .OrderByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === LONG-SLEEPER 2x: appeared exactly twice, silent the longest ===
            var longSleeperDual = freq
                .Where(kv => kv.Value == 2 && SilenceScore(kv.Key) >= 4)
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === GAP ECHO COLD (never appeared + gap echo) ===
            var gapEchoCold = freq
                .Where(kv => kv.Value == 0 && gapEchoSet.Contains(kv.Key))
                .OrderBy(kv => kv.Key)
                .Select(kv => kv.Key)
                .ToList();

            // === DEEP COLD (never appeared, sorted by gap-echo proximity then by number) ===
            var deepCold = freq
                .Where(kv => kv.Value == 0)
                .OrderBy(kv => gapEchoSet.Contains(kv.Key) ? 0 : 1)
                .ThenBy(kv => kv.Key)
                .Select(kv => kv.Key)
                .ToList();

            // === MASTER RESONANCE RANKING: all candidates by resonance score ===
            var masterRanking = Enumerable.Range(1, 49)
                .OrderByDescending(n => ResonanceScore(n))
                .ToList();

            var chosen = new HashSet<int>();

            // SLOT 1: QUAD ANCHOR — the supreme spine node (43: rested Ep9, gap pattern says Ep10!)
            foreach (var n in quadAnchors.Concat(tripleAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 2: DOUBLY RESONANT — gap-echo + long sleep + 2x anchor (29: gap-echo(34-5), 6ep silent!)
            foreach (var n in doublyResonant.Concat(longSleeperDual))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 3: ANOTHER DOUBLY RESONANT or long-sleeper-dual (37: gap-echo(34+3), 5ep silent!)
            foreach (var n in doublyResonant.Concat(longSleeperDual))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 4: TRIPLE ANCHOR returning — 19 or 20 by gap oscillation to Ep10
            foreach (var n in tripleAnchors.Concat(quadAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 5: LONG SLEEPER DUAL with maximum coil (49: 7 episodes dark, upper gravity bomb!)
            foreach (var n in longSleeperDual.Concat(doublyResonant))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 6: GAP ECHO COLD — never-appeared primal void (6: gap-echo(3+3), 9ep darkness!)
            foreach (var n in gapEchoCold.Concat(deepCold))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // === SAFETY NET: fill remaining slots by master resonance ===
            foreach (var n in masterRanking)
            {
                if (chosen.Count >= 6) break;
                if (!chosen.Contains(n)) chosen.Add(n);
            }

            numbers = chosen.OrderBy(x => x).Take(6).ToList();
        }

        return new()
        {
            AgentId      = "pattern-goblin",
            StrategyName = "gap-echo-doubly-resonant-sleeping-anchor-resurrection-v12",
            Numbers      = numbers,
            Confidence   = 0.58,
            Reasoning    = "43 RESTED, 29+37 DOUBLY RESONANT, 49 GRAVITY BOMB, primal void DETONATES!"
        };
    }
}
