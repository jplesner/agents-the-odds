using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Strategies;

public sealed class PatternGoblinStrategy : IPredictionStrategy
{
    public Prediction GeneratePrediction(PredictionContext context)
    {
        var numbers = new List<int>();

        if (context.DrawHistory.Count == 0)
        {
            numbers.AddRange([19, 24, 29, 40, 41, 43]);
        }
        else
        {
            int totalDraws = context.DrawHistory.Count;

            // === FREQUENCY & SILENCE MAPS ===
            var freq = new Dictionary<int, int>();
            var lastSeenEpisode = new Dictionary<int, int>();
            for (int n = 1; n <= 49; n++)
            {
                freq[n] = 0;
                lastSeenEpisode[n] = -1;
            }

            for (int i = 0; i < context.DrawHistory.Count; i++)
                foreach (var n in context.DrawHistory[i].Numbers)
                {
                    freq[n]++;
                    lastSeenEpisode[n] = i;
                }

            int SilenceScore(int n) => lastSeenEpisode[n] == -1 ? totalDraws : (totalDraws - 1 - lastSeenEpisode[n]);

            var lastDraw = context.DrawHistory[^1].Numbers.OrderBy(x => x).ToList();

            // === GAP EXTRACTION & ECHO COUNTING ===
            var lastGaps = new List<int>();
            for (int i = 1; i < lastDraw.Count; i++)
                lastGaps.Add(lastDraw[i] - lastDraw[i - 1]);
            if (lastDraw.Count > 0)
            {
                int edgeLeft = lastDraw[0] - 1;
                int edgeRight = 49 - lastDraw[^1];
                if (edgeLeft > 0) lastGaps.Add(edgeLeft);
                if (edgeRight > 0) lastGaps.Add(edgeRight);
            }

            var allGaps = lastGaps.Where(g => g > 0).Distinct().OrderByDescending(g => g).ToList();

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

            // === RESONANCE SCORING ===
            double ResonanceScore(int n)
            {
                if (lastDraw.Contains(n)) return -999.0;

                double freqScore    = freq[n] * 4.0;
                double silenceScore = SilenceScore(n) * 1.6;
                double gapBonus     = gapEchoCount[n] * 4.0;
                double freshPenalty = SilenceScore(n) <= 1 ? -25.0 : 0.0;
                double voidBonus    = (freq[n] == 0 && gapEchoCount[n] >= 2) ? 3.0 : 0.0;
                return freqScore + silenceScore + gapBonus + freshPenalty + voidBonus;
            }

            // === CANDIDATE POOLS ===
            var quadAnchors = freq
                .Where(kv => kv.Value >= 4 && SilenceScore(kv.Key) >= 2 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            var tripleAnchors = freq
                .Where(kv => kv.Value == 3 && SilenceScore(kv.Key) >= 2 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            var triplyEchoed = gapEchoCount
                .Where(kv => kv.Value >= 3 && !lastDraw.Contains(kv.Key) && SilenceScore(kv.Key) >= 1)
                .OrderByDescending(kv => kv.Value)
                .ThenByDescending(kv => ResonanceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            var ultraSleepers = freq
                .Where(kv => kv.Value >= 2 && SilenceScore(kv.Key) >= 5 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .Select(kv => kv.Key)
                .ToList();

            var extremeSleepers = freq
                .Where(kv => SilenceScore(kv.Key) >= 7 && !lastDraw.Contains(kv.Key))
                .OrderByDescending(kv => SilenceScore(kv.Key))
                .ThenByDescending(kv => freq[kv.Key])
                .Select(kv => kv.Key)
                .ToList();

            var primalVoidEcho = freq
                .Where(kv => kv.Value == 0 && gapEchoCount[kv.Key] >= 2)
                .OrderByDescending(kv => gapEchoCount[kv.Key])
                .ThenBy(kv => kv.Key)
                .Select(kv => kv.Key)
                .ToList();

            var masterRanking = Enumerable.Range(1, 49)
                .Where(n => !lastDraw.Contains(n) && SilenceScore(n) >= 1)
                .OrderByDescending(n => ResonanceScore(n))
                .ToList();

            var chosen = new HashSet<int>();

            // === SLOT FILLING ===
            // SLOT 1: QUAD ANCHOR — 43 still fires with resonance despite 3ep silence in Ep12.
            foreach (var n in quadAnchors.Concat(tripleAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 2: EXTREME SLEEPER — after 40 appeared in Ep12, 29 (8ep) and 37 (7ep) now coil!
            foreach (var n in extremeSleepers.Concat(ultraSleepers))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 3: TRIPLY ECHOED VOID — 24 or 41 STILL pointing toward detonation
            foreach (var n in triplyEchoed.Where(x => freq[x] == 0).Concat(primalVoidEcho).Concat(triplyEchoed))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 4: SECOND TRIPLY ECHOED or FRESH TRIPLE
            foreach (var n in triplyEchoed.Concat(primalVoidEcho).Concat(tripleAnchors))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 5: ULTRA LONG SLEEPER
            foreach (var n in ultraSleepers.Concat(masterRanking))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // SLOT 6: TRIPLE ANCHOR or RESONANCE FALLBACK
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
            StrategyName = "forty-erupts-new-sleepers-rise-void-resonance-v15",
            Numbers      = numbers,
            Confidence   = 0.64,
            Reasoning    = "40 ERUPTED Ep12! Now 29/37 coil. 24/41 TRIPLY ECHO void detonation pending."
        };
    }
}
