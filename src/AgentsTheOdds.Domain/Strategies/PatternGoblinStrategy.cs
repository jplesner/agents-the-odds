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

            double ResonanceScore(int n)
            {
                if (lastDraw.Contains(n)) return -999.0;

                double freqScore    = freq[n] * 3.5;
                double silenceScore = SilenceScore(n) * 1.8;
                double gapBonus     = gapEchoCount[n] * 3.8;
                double freshPenalty = SilenceScore(n) <= 1 ? -30.0 : 0.0;
                double voidBonus    = (freq[n] == 0 && gapEchoCount[n] >= 2) ? 5.0 : 0.0;
                double coldWeight   = SilenceScore(n) >= 8 ? 8.0 : 0.0;
                return freqScore + silenceScore + gapBonus + freshPenalty + voidBonus + coldWeight;
            }

            var masterRanking = Enumerable.Range(1, 49)
                .Where(n => !lastDraw.Contains(n) && SilenceScore(n) >= 1)
                .OrderByDescending(n => ResonanceScore(n))
                .ToList();

            var chosen = new HashSet<int>();

            // Slot 1: Extreme sleeper (>= 8 ep silence) — dormant EPOCH VOID
            foreach (var n in masterRanking.Where(x => SilenceScore(x) >= 8))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 2: High resonance anchor (freq >= 3, silence >= 2)
            foreach (var n in masterRanking.Where(x => freq[x] >= 3 && SilenceScore(x) >= 2))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 3: Triple gap echo (freq >= 1, gapEcho >= 3)
            foreach (var n in masterRanking.Where(x => freq[x] >= 1 && gapEchoCount[x] >= 3))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 4: Primal void (freq == 0, gapEcho >= 2)
            foreach (var n in masterRanking.Where(x => freq[x] == 0 && gapEchoCount[x] >= 2))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slot 5: High-frequency dormant (freq >= 2, silence >= 5)
            foreach (var n in masterRanking.Where(x => freq[x] >= 2 && SilenceScore(x) >= 5))
                if (!chosen.Contains(n)) { chosen.Add(n); break; }

            // Slots 6+: Fill from master resonance
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
            StrategyName = "ep16-dormant-epoch-void-anchor-resurrection-gap-trinity-v18",
            Numbers      = numbers,
            Confidence   = 0.62,
            Reasoning    = "Epoch void sleepers + triple anchors + gap echoes. Void cascade rises. Silence SPEAKS."
        };
    }
}
