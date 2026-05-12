using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Cli;

public sealed class ConsoleGamePresenter
{
    public void ShowEpisode(EpisodeResult result)
    {
        var draw = result.DrawResult;

        Console.WriteLine();
        Console.WriteLine($"── Episode {result.EpisodeNumber} ─────────────────────────────────────────");
        Console.WriteLine($"   Draw: [{string.Join(", ", draw.Numbers)}]");
        Console.WriteLine();

        Console.WriteLine("── Predictions ─────────────────────────────────────────────");
        Console.WriteLine();

        foreach (var score in result.Scores.OrderByDescending(s => s.Points).ThenByDescending(s => s.Prediction.Confidence))
        {
            var p = score.Prediction;
            Console.WriteLine($"  {p.AgentId}  ({p.StrategyName})");
            Console.WriteLine($"  Picked:     [{string.Join(", ", p.Numbers)}]");
            Console.WriteLine($"  Confidence: {p.Confidence:F2}");
            Console.WriteLine($"  Reasoning:  \"{p.Reasoning}\"");
            Console.WriteLine($"  Result:     {score.Matches} matches → {score.Points} pts");
            Console.WriteLine();
        }

        Console.WriteLine("── Leaderboard ─────────────────────────────────────────────");
        Console.WriteLine();
        Console.WriteLine($"  {"#",-3} {"Agent",-22} {"Total Pts",9}");
        Console.WriteLine($"  {new string('-', 38)}");
        foreach (var entry in result.Leaderboard.OrderBy(e => e.Rank))
            Console.WriteLine($"  {entry.Rank,-3} {entry.AgentName,-22} {entry.TotalPoints,9}");
        Console.WriteLine();

        Console.WriteLine($"  {result.RealityCheck}");
        Console.WriteLine();
    }
}
