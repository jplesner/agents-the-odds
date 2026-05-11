using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using System.Text.Json;

namespace AgentsTheOdds.Data.Storage;

public sealed class JsonDrawResultRepository(DataRootOptions options) : IDrawResultRepository
{
    private string DrawsPath => Path.Combine(options.Path, "draws");

    public IReadOnlyList<DrawResult> GetHistory()
    {
        if (!Directory.Exists(DrawsPath))
            return [];

        return Directory.GetFiles(DrawsPath, "episode-*.json")
            .OrderBy(f => f)
            .Select(f => JsonSerializer.Deserialize<DrawResult>(
                System.IO.File.ReadAllText(f), JsonOptions.Default)!)
            .ToList();
    }

    public DrawResult? TryGetByEpisode(int episodeNumber)
    {
        var path = Path.Combine(DrawsPath, EpisodeFileName(episodeNumber));
        if (!System.IO.File.Exists(path))
            return null;

        return JsonSerializer.Deserialize<DrawResult>(
            System.IO.File.ReadAllText(path), JsonOptions.Default);
    }

    private static string EpisodeFileName(int n) => $"episode-{n:D3}.json";
}
