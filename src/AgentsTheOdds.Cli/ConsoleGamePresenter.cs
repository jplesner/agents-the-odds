using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Cli;

public sealed class ConsoleGamePresenter : IGamePresenter
{
    public void ShowHeader(DrawResult draw)
    {
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║          Agents the Odds             ║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"Draw #{draw.DrawNumber} · {draw.Date}");
        Console.WriteLine($"Numbers: {string.Join("  ", draw.Numbers)}");
        Console.WriteLine();
    }

    public void ShowInvalidPrediction(string agentName, string error)
    {
        Console.WriteLine($"[INVALID] {agentName}: {error}");
    }

    public void ShowPredictions(IReadOnlyList<(Agent Agent, PredictionResult Result)> ranked)
    {
        Console.WriteLine("── Predictions ─────────────────────────────────────────────");
        Console.WriteLine();
        foreach (var (agent, result) in ranked)
        {
            Console.WriteLine($"  {agent.Name}  ({result.Prediction.StrategyName})");
            Console.WriteLine($"  Picked:     {string.Join("  ", result.Prediction.Numbers)}");
            Console.WriteLine($"  Confidence: {result.Prediction.Confidence:F2}");
            Console.WriteLine($"  Reasoning:  \"{result.Prediction.Reasoning}\"");
            Console.WriteLine($"  Result:     {result.Matches} matches → {result.Points} pts");
            Console.WriteLine();
        }
    }

    public void ShowLeaderboard(IReadOnlyList<(Agent Agent, PredictionResult Result)> ranked)
    {
        Console.WriteLine("── Leaderboard ─────────────────────────────────────────────");
        Console.WriteLine();
        Console.WriteLine($"  {"#",-3} {"Agent",-22} {"Pts",5}   {"Matches",7}   {"Confidence",10}");
        Console.WriteLine($"  {new string('-', 55)}");
        for (var i = 0; i < ranked.Count; i++)
        {
            var (agent, result) = ranked[i];
            Console.WriteLine($"  {i + 1,-3} {agent.Name,-22} {result.Points,5}   {result.Matches,7}   {result.Prediction.Confidence,10:F2}");
        }
        Console.WriteLine();
    }
}
