using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using System.Text;

namespace AgentsTheOdds.Data.Storage;

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
        var sb = new StringBuilder();
        sb.AppendLine($"# Episode {r.EpisodeNumber} Recap");
        sb.AppendLine();
        sb.AppendLine($"**Draw date:** {r.DrawResult.Date}");
        sb.AppendLine($"**Numbers drawn:** {string.Join(", ", r.DrawResult.Numbers)}");
        sb.AppendLine();
        sb.AppendLine("## Predictions & Scores");
        sb.AppendLine();
        sb.AppendLine("| Agent | Numbers | Matches | Points | Confidence |");
        sb.AppendLine("|-------|---------|---------|--------|------------|");
        foreach (var s in r.Scores.OrderByDescending(x => x.Points))
        {
            sb.AppendLine(
                $"| {s.Prediction.AgentId} " +
                $"| {string.Join(", ", s.Prediction.Numbers)} " +
                $"| {s.Matches} | {s.Points} " +
                $"| {s.Prediction.Confidence:F2} |");
        }
        sb.AppendLine();
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
