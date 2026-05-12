using System.Text;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Data.File;

public sealed class MarkdownRecapWriter(DataRootOptions options) : IRecapWriter
{
    private string EpisodesPath => Path.Combine(options.Path, "episodes");

    public void Write(EpisodeResult result)
    {
        Directory.CreateDirectory(EpisodesPath);
        var path = Path.Combine(EpisodesPath, $"episode-{result.EpisodeNumber:D3}.md");
        System.IO.File.WriteAllText(path, BuildMarkdown(result));
    }

    private static string BuildMarkdown(EpisodeResult r)
    {
        var nameById = r.Leaderboard.ToDictionary(e => e.AgentId, e => e.AgentName);
        string AgentName(string id) => nameById.GetValueOrDefault(id, id);

        var sb = new StringBuilder();
        sb.AppendLine($"# Episode {r.EpisodeNumber} Recap");
        sb.AppendLine();
        sb.AppendLine($"**Draw date:** {r.DrawResult.Date}");
        sb.AppendLine($"**Numbers drawn:** {string.Join(", ", r.DrawResult.Numbers)}");
        sb.AppendLine();
        sb.AppendLine("## Predictions & Scores");
        sb.AppendLine();
        sb.AppendLine("| Agent | Numbers | Strategy | Matches | Points | Confidence |");
        sb.AppendLine("|-------|---------|----------|---------|--------|------------|");
        foreach (var s in r.Scores.OrderByDescending(x => x.Points))
        {
            sb.AppendLine(
                $"| {AgentName(s.Prediction.AgentId)} " +
                $"| {string.Join(", ", s.Prediction.Numbers)} " +
                $"| {s.Prediction.StrategyName} " +
                $"| {s.Matches} | {s.Points} " +
                $"| {s.Prediction.Confidence:F2} |");
        }
        sb.AppendLine();
        sb.AppendLine("## Agent Reasoning");
        sb.AppendLine();
        foreach (var s in r.Scores.OrderByDescending(x => x.Points))
        {
            sb.AppendLine($"**{AgentName(s.Prediction.AgentId)}**");
            sb.AppendLine($"> {s.Prediction.Reasoning}");
            sb.AppendLine();
        }
        sb.AppendLine("## Leaderboard");
        sb.AppendLine();
        sb.AppendLine("| Rank | Agent | Total Points |");
        sb.AppendLine("|------|-------|--------------|");
        foreach (var e in r.Leaderboard)
            sb.AppendLine($"| {e.Rank} | {e.AgentName} | {e.TotalPoints} |");
        sb.AppendLine();
        sb.AppendLine("## Reality Check");
        sb.AppendLine();
        sb.AppendLine(r.RealityCheck);
        return sb.ToString();
    }
}
