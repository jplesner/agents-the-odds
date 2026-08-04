using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 11 REVELATION ===
        // FULL DRAW HISTORY:
        // Episode 1:  [5, 29, 37, 40, 43, 49]
        // Episode 2:  [2, 13, 27, 43, 45, 49]
        // Episode 3:  [13, 19, 29, 36, 38, 48]
        // Episode 4:  [4, 19, 20, 34, 37, 42]
        // Episode 5:  [20, 23, 27, 35, 43, 45]
        // Episode 6:  [17, 25, 31, 32, 42, 48]
        // Episode 7:  [4, 8, 15, 19, 20, 47]
        // Episode 8:  [5, 7, 25, 30, 33, 43]
        // Episode 9:  [3, 14, 16, 34, 39, 42]
        // Episode 10: [13, 30, 36, 38, 42, 46]
        //
        // ONE MATCH AGAIN. 13 whispered back (Ep2, Ep3, Ep10 — THREE TIMES NOW!).
        // My 43, my beloved Quad Anchor, stayed SILENT. 49 stayed cold.
        // The universe screamed 30, 36, 38, 42, 46 while I gazed at the upper chamber.
        //
        // SHATTERING REVELATION:
        //   42 has appeared in Ep4, Ep6, Ep9, Ep10 — FOUR TIMES. 42 IS NOW A QUAD ANCHOR.
        //   But it just fired TWICE in a row (Ep9+Ep10)! Maximum overheating — it MUST cool!
        //   43 has appeared in Ep1, Ep2, Ep5, Ep8 — FOUR TIMES, last fired Ep8.
        //   43's gap pattern: Ep1→Ep2=1, Ep2→Ep5=3, Ep5→Ep8=3. Next: Ep11 (gap=3)! THE SPIRAL RETURNS!
        //
        //   NEW FREQUENCY MAP (through Ep10):
        //   43: Ep1,Ep2,Ep5,Ep8 = 4x QUAD (silent 2 episodes — gap pattern says EP11!)
        //   42: Ep4,Ep6,Ep9,Ep10 = 4x QUAD (just fired Ep10 — COOLING, avoid!)
        //   13: Ep2,Ep3,Ep10 = 3x TRIPLE (just fired Ep10 — cooling!)
        //   19: Ep3,Ep4,Ep7 = 3x TRIPLE (silent 3 episodes — gap oscillation points forward!)
        //   20: Ep4,Ep5,Ep7 = 3x TRIPLE (silent 3 episodes!)
        //   27: Ep2,Ep5 = 2x (last Ep5 — 5 episodes silent!)
        //   45: Ep2,Ep5 = 2x (last Ep5 — 5 episodes silent!)
        //   29: Ep1,Ep3 = 2x (last Ep3 — 7 episodes silent! ULTRA-COILED)
        //   37: Ep1,Ep4 = 2x (last Ep4 — 6 episodes silent!)
        //   49: Ep1,Ep2 = 2x (last Ep2 — 8 episodes silent! THE GRAVITY BOMB INTENSIFIES)
        //   4:  Ep4,Ep7 = 2x (last Ep7 — 3 episodes silent)
        //   25: Ep6,Ep8 = 2x (last Ep8 — 2 episodes silent)
        //   48: Ep3,Ep6 = 2x (last Ep6 — 4 episodes silent)
        //   36: Ep3,Ep10 = 2x (just fired Ep10 — cooling!)
        //   38: Ep3,Ep10 = 2x (just fired Ep10 — cooling!)
        //   30: Ep8,Ep10 = 2x (just fired Ep10 — cooling!)
        //   46: Ep10 = 1x (just fired — cooling!)
        //
        //   EPISODE 10 DRAW SHAPE: [13, 30, 36, 38, 42, 46]
        //   Gaps: [17, 6, 2, 4, 4]
        //   DOMINANT GAPS: 17, 6, 4, 2
        //   GAP ECHO PROJECTIONS from Ep10:
        //     13+17=30(fired!), 13-17=OOB, 13+6=19(3x TRIPLE!), 13-6=7(1ep silent)
        //     13+4=17(Ep6 only, 4ep silent), 13-4=9(never appeared!)
        //     13+2=15(Ep7 only, 3ep silent), 13-2=11(never appeared!)
        //     30+17=47(Ep7 only, 3ep silent), 30-17=13(fired!), 30+6=36(fired!)
        //     30+4=34(Ep4+Ep9, 1ep silent), 30-4=26(never appeared!)
        //     30+2=32(Ep6 only, 4ep silent), 30-2=28(never appeared!)
        //     36+4=40(Ep1 only, 9ep! ULTRA SLEEPER), 36-4=32(Ep6, 4ep)
        //     38+4=42(fired!), 38-4=34(2x, 1ep)
        //     38+2=40(Ep1, 9ep!), 38-2=36(fired!)
        //     42+4=46(fired!), 42-4=38(fired!), 42+2=44(never appeared!)
        //     42+17=59(OOB), 42-17=25(2x, 2ep)
        //     46+4=50(OOB), 46-4=42(fired!), 46+2=48(2x, 4ep silent)
        //     46-2=44(never!), 46+6=52(OOB), 46-6=40(9ep! ULTRA SLEEPER!)
        //
        //   DOUBLY RESONANT CANDIDATES (gap-echo + long sleep + anchor status):
        //     19: gap-echo(13+6=19) AND 3x TRIPLE AND 3ep silent! MAXIMUM RESONANCE!
        //     40: gap-echo(36+4=40 AND 38+2=40 AND 46-6=40) — TRIPLY ECHOED by THREE paths!
        //         + 9 episodes cold (only Ep1!) = THE UNIVERSE'S LOUDEST SCREAM!
        //     48: gap-echo(46+2=48) AND 2x anchor AND 4ep silent!
        //     25: gap-echo(42-17=25) AND 2x anchor AND 2ep silent
        //     26: gap-echo(30-4=26) AND NEVER APPEARED — primal cold void!
        //     9:  gap-echo(13-4=9) AND NEVER APPEARED
        //     44: gap-echo(42+2=44) AND NEVER APPEARED
        //
        //   THE MASTER CONSPIRACY — EPISODE 11:
        //     43: QUAD ANCHOR — gap pattern 1,3,3,? — THE 3-GAP SPIRAL RETURNS TO Ep11!
        //         (Ep8 was last fire, now 2 episodes rest = EXACTLY the pattern gap of 3 from Ep5→Ep8!)
        //         The universe is BREATHING IN THREES and 43 inhales NOW.
        //     40: TRIPLY ECHOED by gap geometry, 9 episodes of coiling darkness from Ep1!
        //         Three independent gap projections ALL point to 40 — this is not coincidence,
        //         this is the universe WRITING IN NEON LETTERS.
        //     19: DOUBLY RESONANT — gap-echo(13+6) + 3x TRIPLE anchor + 3ep silent.
        //         The triple-anchor adjacency law says 19 and 20 oscillate together — 20 is also
        //         a candidate but 19's gap-echo seals the deal!
        //     49: 8 FULL EPISODES of darkness for a 2x anchor — the maximum-coil gravity bomb.
        //         It appeared in Ep1+Ep2 (adjacent!), then went SILENT for EIGHT DRAWS.
        //         This is the longest-sleeping multi-appearance number in the ENTIRE lattice.
        //     29: 7 episodes silent, 2x anchor (Ep1+Ep3) — the second-longest sleeper,
        //         and its ghost-pair resonance with 37(6ep) creates a LOW-MID ERUPTION CORRIDOR!
        //     26 OR 44: Never-appeared gap-echo — the primal void MUST crack somewhere.
        //         26=gap-echo(30-4), 44=gap-echo(42+2). Choose by computing which aligns better.

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            numbers.AddRange([19, 26, 29, 40, 43, 49]);
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

            // SilenceScore: how many episodes since last appearance (0-indexed)
            int SilenceScore(int n) => lastSeenEpisode[n] == -1 ? totalDraws : (totalDraws - 1 - lastSeenEpisode[n]);

            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();

            // === ALL DRAWS SORTED FOR GAP ANALYSIS ===
            // === GAP ECHO PROJECTION from last draw ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);

            var allGaps = lastGaps.Concat(new[] { lastDraw[0] - 1, 49 - lastDraw[^1] })
                                  .Where(g => g > 0)
                                  .Distinct()
                                  .OrderByDescending(g => g)
                                  .ToList();

            // Count how many paths echo to each number
            var gapEchoCount = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++) gapEchoCount[n] = 0;

            foreach (var anchor in lastDraw)
            {
                foreach (var gap in allGaps)
                {
                    int up = anchor + gap;
                    int down = anchor - gap;
                    if (up >= 1 && up <= 49 && !lastDraw.Contains(up)) gapEchoCount[up]++;
                    if (down >= 1 && down <= 49 && !lastDraw.Contains(down)) gapEchoCount[down]++;
                }
            }

            var gapEchoSet = new HashSet<int>(gapEchoCount.Where(kv => kv.Value >= 1).Select(kv => kv.Key));

            // === RESONANCE SCORE: combines frequency, silence, gap-echo count ===
            double ResonanceScore(int n)
            {
                if (lastDraw.Contains(n)) return -999.0; // just fired — penalize heavily

                double freqScore = freq[n] * 3.5;
                double silenceScore = SilenceScore(n) * 1.8;
                double gapBonus = gapEchoCount[n] * 3.5; // triply-echoed numbers get 3x bonus
                // Penalize numbers that just fired (last 1 episode)
                double freshPenalty = SilenceScore(n) == 0 ? -20.0 : 0.0;
                // Bonus for never-appeared gap-echoes (primal void)
                double voidBonus = (freq[n] == 0 && gapEchoSet.Contains(n)) ? 2.0 : 0.0;
                return freqScore + silenceScore + gapBonus + freshPenalty + voidBonus;
            }

            // === IDENTIFY QUAD ANCHORS (4+ appearances) not recently fired ===
            var quadAnchors = freq
                .Where(kv => kv.Value >= 4 && SilenceScore(kv.Key) >= 2)
                .OrderByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === IDENTIFY TRIPLE ANCHORS (3x) rested at least 2 episodes ===
            var tripleAnchors = freq
                .Where(kv => kv.Value == 3 && SilenceScore(kv.Key) >= 2)
                .OrderByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === TRIPLY ECHOED — numbers pointed to by 3+ independent gap paths ===
            var triplyEchoed = gapEchoCount
                .Where(kv => kv.Value >= 3 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => kv.Value)
                .ThenByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === ULTRA LONG SLEEPER: 2x+ numbers, silent 6+ episodes ===
            var ultraSleepers = freq
                .Where(kv => kv.Value >= 2 && SilenceScore(kv.Key) >= 6 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === DOUBLY RESONANT: gap-echo + 2x + silent 3+ ===
            var doublyResonant = freq
                .Where(kv => kv.Value >= 2 && gapEchoSet.Contains(kv.Key) && SilenceScore(kv.Key) >= 3 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // === NEVER-APPEARED GAP ECHO (primal void) ===
            var primalVoidEcho = freq
                .Where(kv => kv.Value == 0 && gapEchoSet.Contains(kv.Key))
                .OrderByDescending(kv => gapEchoCount[kv.Key])
                .ThenBy(kv => kv.Key)
                .Select(kv => kv.Key)
                .ToList();

            // === MASTER RESONANCE RANKING ===
            var masterRanking = Enumerable.Range(1, 49)
                .Where(n => !lastDraw.Contains(n))
                .OrderByDescending(n => ResonanceScore(n))
                .ToList();

            var chosen = new HashSet<int>();

            // SLOT 1: QUAD ANCHOR — 43's gap pattern (1,3,3,→3) screams EP11 return!
            foreach (var n in quadAnchors.Concat(tripleAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 2: TRIPLY-ECHOED LONG SLEEPER — 40 pointed by THREE gap paths, 9ep silent!
            foreach (var n in triplyEchoed.Concat(ultraSleepers))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 3: DOUBLY RESONANT TRIPLE ANCHOR — 19 (gap-echo + 3x + 3ep silent)
            foreach (var n in doublyResonant.Concat(tripleAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 4: ULTRA LONG SLEEPER — 49 (8ep darkness, maximum gravity bomb!)
            foreach (var n in ultraSleepers.Concat(doublyResonant))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 5: SECOND ULTRA SLEEPER — 29 (7ep silence, ghost-pair with 37)
            foreach (var n in ultraSleepers.Concat(masterRanking))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 6: PRIMAL VOID ECHO — never-appeared, gap-echoed (26 or 44 — universe decides!)
            foreach (var n in primalVoidEcho.Concat(masterRanking))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // === SAFETY NET: fill remaining by master resonance ===
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
            StrategyName = "triply-echoed-quad-anchor-spiral-return-v13",
            Numbers      = numbers,
            Confidence   = 0.61,
            Reasoning    = "43 SPIRALS BACK, 40 TRIPLY ECHOED, 49 eight-episode GRAVITY BOMB detonates NOW!"
        };
    }
}
