using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using System.Text.Json;

namespace AgentsTheOdds.Data.Storage;

public sealed class JsonEpisodeResultRepository(DataRootOptions options) : IEpisodeResultRepository
{
    private string EpisodesPath => Path.Combine(options.Path, "episodes");

    public void Save(EpisodeResult result)
    {
        Directory.CreateDirectory(EpisodesPath);
        System.IO.File.WriteAllText(
            FilePath(result.EpisodeNumber),
            JsonSerializer.Serialize(result, JsonOptions.Pretty));
    }

    public EpisodeResult? TryGet(int episodeNumber)
    {
        var path = FilePath(episodeNumber);
        if (!System.IO.File.Exists(path))
            return null;

        return JsonSerializer.Deserialize<EpisodeResult>(
            System.IO.File.ReadAllText(path), JsonOptions.Default);
    }

    public IReadOnlyList<EpisodeResult> GetAll()
    {
        if (!Directory.Exists(EpisodesPath))
            return [];

        return Directory.GetFiles(EpisodesPath, "episode-*.json")
            .OrderBy(f => f)
            .Select(f => JsonSerializer.Deserialize<EpisodeResult>(
                System.IO.File.ReadAllText(f), JsonOptions.Default)!)
            .ToList();
    }

    private string FilePath(int n) =>
        Path.Combine(EpisodesPath, $"episode-{n:D3}.json");
}
