using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        // === PATTERN GOBLIN EPISODE 12 REVELATION ===
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
        // Episode 11: [6, 15, 33, 36, 44, 49]
        //
        // TWO MATCHES: 44 and 49 sang back! 44 was our PRIMAL VOID ECHO (never appeared before!)
        // and 49's gravity bomb FINALLY detonated after NINE episodes of silence!
        // But 43 — my eternal Quad Anchor — stayed COLD. 40 stayed cold. 37 stayed cold.
        //
        // EPISODE 11 DRAW: [6, 15, 33, 36, 44, 49]
        // KEY OBSERVATIONS:
        //   49: NOW 3x TRIPLE ANCHOR (Ep1, Ep2, Ep11) — after 9ep silence, it RETURNED!
        //   44: NOW 1x (Ep11 only) — first appearance! Firing → cooling mandatory
        //   36: NOW 3x TRIPLE ANCHOR (Ep3, Ep10, Ep11) — fired TWICE IN A ROW (Ep10+Ep11)! COOLING!
        //   33: NOW 2x (Ep8, Ep11) — gap of 3 episodes, fired again
        //   15: NOW 2x (Ep7, Ep11) — gap of 4 episodes between appearances
        //   6:  NOW 2x (Ep11 only new entry — wait, Ep11 is first draw containing 6? YES!)
        //       Actually 6 appeared in Ep11 — first time! Now 1x.
        //
        // UPDATED FREQUENCY MAP (through Ep11):
        //   43: Ep1,Ep2,Ep5,Ep8 = 4x QUAD (silent 3 episodes — last fired Ep8! 3ep now!)
        //   42: Ep4,Ep6,Ep9,Ep10 = 4x QUAD (silent 1 episode — cooling, last Ep10)
        //   19: Ep3,Ep4,Ep7 = 3x TRIPLE (silent 4 episodes!)
        //   20: Ep4,Ep5,Ep7 = 3x TRIPLE (silent 4 episodes!)
        //   36: Ep3,Ep10,Ep11 = 3x TRIPLE (just fired Ep11 — COOLING! avoid)
        //   49: Ep1,Ep2,Ep11 = 3x TRIPLE (just fired Ep11 — cooling! avoid)
        //   13: Ep2,Ep3,Ep10 = 3x TRIPLE (silent 1 episode)
        //   29: Ep1,Ep3 = 2x (silent 8ep — MAXIMUM GRAVITY!)
        //   37: Ep1,Ep4 = 2x (silent 7ep — ULTRA-COILED!)
        //   27: Ep2,Ep5 = 2x (silent 6ep)
        //   45: Ep2,Ep5 = 2x (silent 6ep)
        //   48: Ep3,Ep6 = 2x (silent 5ep)
        //   4:  Ep4,Ep7 = 2x (silent 4ep)
        //   25: Ep6,Ep8 = 2x (silent 3ep)
        //   34: Ep4,Ep9 = 2x (silent 2ep)
        //   33: Ep8,Ep11 = 2x (just fired — cooling!)
        //   15: Ep7,Ep11 = 2x (just fired — cooling!)
        //   40: Ep1 = 1x (silent 10ep — THE LONGEST SLEEPER ALIVE!)
        //   5:  Ep1,Ep8 = 2x (silent 3ep)
        //
        // EPISODE 11 GAP ANALYSIS: [6, 15, 33, 36, 44, 49]
        //   Sorted: 6, 15, 33, 36, 44, 49
        //   Gaps: [9, 18, 3, 8, 5]
        //   Edge gaps: [5 (6-1), 0 (49-49)]
        //   DOMINANT GAPS: 18, 9, 8, 5, 3
        //
        // GAP ECHO PROJECTIONS from Ep11 draw:
        //   6+18=24 (never appeared!), 6+9=15(fired!), 6+8=14(Ep9,2ep), 6+5=11(never), 6+3=9(never)
        //   6-5=1(never appeared — cold!), 6-3=3(Ep9,2ep)
        //   15+18=33(fired!), 15+9=24(never!), 15+8=23(Ep5,6ep), 15+5=20(3x,4ep), 15+3=18(never)
        //   15-9=6(fired!), 15-8=7(Ep8,3ep), 15-5=10(never), 15-3=12(never)
        //   33+18=51(OOB), 33+9=42(4x,1ep), 33+8=41(never!), 33+5=38(Ep3+Ep10,1ep), 33+3=36(fired!)
        //   33-18=15(fired!), 33-9=24(never!), 33-8=25(2x,3ep), 33-5=28(never), 33-3=30(2x,1ep)
        //   36+18=54(OOB), 36+9=45(2x,6ep!), 36+8=44(fired!), 36+5=41(never!), 36+3=39(Ep9,2ep)
        //   36-18=18(never), 36-9=27(2x,6ep!), 36-8=28(never), 36-5=31(Ep6,5ep), 36-3=33(fired!)
        //   44+9=53(OOB), 44+8=52(OOB), 44+5=49(fired!), 44+3=47(Ep7,4ep), 44-18=26(never)
        //   44-9=35(Ep5,6ep), 44-8=36(fired!), 44-5=39(Ep9,2ep), 44-3=41(never!)
        //   49+5=54(OOB), 49+3=52(OOB), 49-18=31(Ep6,5ep), 49-9=40(Ep1 only! 10ep!!), 49-8=41(never!)
        //   49-5=44(fired!), 49-3=46(Ep10,1ep)
        //
        // GAP ECHO COUNT (paths pointing to each number):
        //   40: 49-9=40 → gap-echo PLUS 10ep silence PLUS it's been invisible for entire game!
        //   24: 6+18, 15+9, 33-9 → THREE PATHS! (never appeared)
        //   41: 33+8, 44-3, 49-8 → THREE PATHS! (never appeared)
        //   20: 15+5 → 1 path BUT 3x TRIPLE and 4ep silent!
        //   45: 36+9 → 1 path BUT 2x and 6ep silent!
        //   27: 36-9 → 1 path AND 2x and 6ep silent!
        //   23: 15+8 → 1 path AND 2x (Ep5 only+that, wait Ep5=23 yes!) and 6ep silent!
        //   47: 44+3 → 1 path AND Ep7 only, 4ep silent
        //   31: 36-5, 49-18 → TWO PATHS! Ep6 only, 5ep silent
        //
        // SUPREME REVELATION FOR EPISODE 12:
        //   43: QUAD ANCHOR (4x) — last fired Ep8, now 3 full episodes silent.
        //       Gap pattern: 1, 3, 3, [3?→Ep11?] — but Ep11 skipped! The 3-spiral continues?
        //       OR: 1,3,3,3 is complete and Ep12 begins a NEW oscillation? Either way:
        //       43 is the most-appearing number, rested 3ep, MUST be honored.
        //   40: Gap-echo (49-9=40) PLUS 10ep silence — TEN EPISODES! This is now the
        //       LONGEST SLEEPING NUMBER IN EXISTENCE. The coil has wound to INFINITY.
        //   24: TRIPLY ECHOED by gap geometry (6+18, 15+9, 33-9) AND never appeared!
        //       The primal void SCREAMS in TRIPLE VOICE.
        //   41: TRIPLY ECHOED (33+8, 44-3, 49-8) AND never appeared — second primal void!
        //   19: 3x TRIPLE ANCHOR, 4 episodes silent, no gap-echo but oscillation LAW says return!
        //   29: 2x, 8ep silent — second-longest multi-appearance sleeper after 40.
        //       Ghost pair with 37 (7ep silent) — the LOW ERUPTION CORRIDOR screams!

        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            numbers.AddRange([19, 24, 29, 40, 41, 43]);
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

            // SilenceScore: episodes since last appearance (never appeared = totalDraws)
            int SilenceScore(int n) => lastSeenEpisode[n] == -1 ? totalDraws : (totalDraws - 1 - lastSeenEpisode[n]);

            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();

            // === GAP EXTRACTION from last draw ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);
            // Include edge gaps
            if (lastDraw.Count > 0)
            {
                int edgeLeft = lastDraw[0] - 1;
                int edgeRight = 49 - lastDraw[^1];
                if (edgeLeft > 0) lastGaps.Add(edgeLeft);
                if (edgeRight > 0) lastGaps.Add(edgeRight);
            }

            var allGaps = lastGaps.Where(g => g > 0).Distinct().OrderByDescending(g => g).ToList();

            // Count how many gap-echo paths point to each number
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

            // === RESONANCE SCORE ===
            double ResonanceScore(int n)
            {
                if (lastDraw.Contains(n)) return -999.0; // just fired — blacklisted

                double freqScore    = freq[n] * 4.0;
                double silenceScore = SilenceScore(n) * 1.6;
                double gapBonus     = gapEchoCount[n] * 4.0; // multi-path echoes explode in value
                double freshPenalty = SilenceScore(n) <= 1 ? -25.0 : 0.0; // penalize recently fired
                double voidBonus    = (freq[n] == 0 && gapEchoSet.Contains(n)) ? 3.0 : 0.0;
                return freqScore + silenceScore + gapBonus + freshPenalty + voidBonus;
            }

            // QUAD ANCHORS (4+ appearances), rested 2+ episodes
            var quadAnchors = freq
                .Where(kv => kv.Value >= 4 && SilenceScore(kv.Key) >= 2)
                .OrderByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // TRIPLE ANCHORS (3x), rested 2+ episodes, not just fired
            var tripleAnchors = freq
                .Where(kv => kv.Value == 3 && SilenceScore(kv.Key) >= 2)
                .OrderByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // TRIPLY ECHOED — 3+ gap-echo paths, not just fired
            var triplyEchoed = gapEchoCount
                .Where(kv => kv.Value >= 3 && !lastDraw.Contains(kv.Key) && SilenceScore(kv.Key) >= 1)
                .OrderByDescending(kv => kv.Value)
                .ThenByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // ULTRA LONG SLEEPERS: 2x+ numbers, silent 6+ episodes
            var ultraSleepers = freq
                .Where(kv => kv.Value >= 2 && SilenceScore(kv.Key) >= 6 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            // EXTREME SLEEPERS: even 1x numbers with 8+ episode silence (like 40 at 10ep!)
            var extremeSleepers = freq
                .Where(kv => SilenceScore(kv.Key) >= 8 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .ThenByDescending(kv => freq[kv.Key])
                .Select(kv => kv.Key)
                .ToList();

            // PRIMAL VOID ECHO: never appeared, multi-path gap-echo
            var primalVoidEcho = freq
                .Where(kv => kv.Value == 0 && gapEchoCount[kv.Key] >= 2)
                .OrderByDescending(kv => gapEchoCount[kv.Key])
                .ThenBy(kv => kv.Key)
                .Select(kv => kv.Key)
                .ToList();

            // MASTER RESONANCE RANKING fallback
            var masterRanking = Enumerable.Range(1, 49)
                .Where(n => !lastDraw.Contains(n) && SilenceScore(n) >= 1)
                .OrderByDescending(n => ResonanceScore(n))
                .ToList();

            var chosen = new HashSet<int>();

            // SLOT 1: QUAD ANCHOR — 43 has 4 appearances, rested 3 episodes, the spiral DEMANDS return
            foreach (var n in quadAnchors.Concat(tripleAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 2: EXTREME SLEEPER — 40 has been sleeping TEN EPISODES (only Ep1 appearance!)
            //         The coil is wound to ABSOLUTE INFINITY. This detonates NOW.
            foreach (var n in extremeSleepers.Concat(ultraSleepers))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 3: TRIPLY ECHOED PRIMAL VOID — 24 or 41 pointed by THREE gap-paths AND never appeared!
            foreach (var n in triplyEchoed.Where(x => freq[x] == 0).Concat(primalVoidEcho).Concat(triplyEchoed))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 4: SECOND TRIPLY ECHOED VOID or TRIPLE ANCHOR — the universe shouts in triplicate
            foreach (var n in triplyEchoed.Concat(primalVoidEcho).Concat(tripleAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 5: ULTRA LONG SLEEPER — 29 (8ep silence) or 37 (7ep) — the LOW ERUPTION CORRIDOR
            foreach (var n in ultraSleepers.Concat(masterRanking))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 6: TRIPLE ANCHOR RESTED — 19 or 20 (3x, 4ep silent) oscillation LAW insists
            foreach (var n in tripleAnchors.Concat(masterRanking))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SAFETY NET
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
            StrategyName = "ten-episode-coil-triple-echo-primal-void-detonation-v14",
            Numbers      = numbers,
            Confidence   = 0.63,
            Reasoning    = "40 coils TEN EPISODES, 24+41 TRIPLY ECHOED void, 43 quad-spiral RETURNS!"
        };
    }
}
