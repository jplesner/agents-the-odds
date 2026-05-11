using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using System.Text.Json;

namespace AgentsTheOdds.Data.Storage;

public sealed class JsonDrawRepository(DataRootOptions options) : IDrawRepository
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

    public DrawResult GetCurrent()
    {
        var numbers = Enumerable.Range(1, 49)
            .OrderBy(_ => Random.Shared.Next())
            .Take(6)
            .Order()
            .ToArray();
        return new DrawResult
        {
            DrawNumber = 0,
            Date = DateOnly.FromDateTime(DateTime.Today),
            Numbers = numbers,
        };
    }

    public void RecordDraw(DrawResult draw)
    {
        Directory.CreateDirectory(DrawsPath);
        System.IO.File.WriteAllText(
            FilePath(draw.DrawNumber),
            JsonSerializer.Serialize(draw, JsonOptions.Pretty));
    }

    public DrawResult GetByEpisode(int episodeNumber)
    {
        var path = FilePath(episodeNumber);
        if (!System.IO.File.Exists(path))
            throw new InvalidOperationException(
                $"Draw for episode {episodeNumber} not found. " +
                $"Run `draw --episode {episodeNumber}` first.");

        return JsonSerializer.Deserialize<DrawResult>(
            System.IO.File.ReadAllText(path), JsonOptions.Default)!;
    }

    private string FilePath(int n) =>
        Path.Combine(DrawsPath, $"episode-{n:D3}.json");
}
