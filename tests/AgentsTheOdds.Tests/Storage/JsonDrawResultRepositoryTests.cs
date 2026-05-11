using AgentsTheOdds.Data.Storage;
using AgentsTheOdds.Domain.Models;
using System.Text.Json;

namespace AgentsTheOdds.Tests.Storage;

public class JsonDrawResultRepositoryTests : IDisposable
{
    private readonly string _dataRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private DataRootOptions Options => new() { Path = _dataRoot };

    public void Dispose() { if (Directory.Exists(_dataRoot)) Directory.Delete(_dataRoot, recursive: true); }

    private void WriteDrawFile(int episode, DrawResult draw)
    {
        var dir = Path.Combine(_dataRoot, "draws");
        Directory.CreateDirectory(dir);
        File.WriteAllText(
            Path.Combine(dir, $"episode-{episode:D3}.json"),
            JsonSerializer.Serialize(draw, new JsonSerializerOptions { WriteIndented = true }));
    }

    [Fact]
    public void TryGetByEpisode_MissingFile_ReturnsNull()
    {
        var repo = new JsonDrawResultRepository(Options);
        Assert.Null(repo.TryGetByEpisode(1));
    }

    [Fact]
    public void TryGetByEpisode_ExistingFile_DeserializesCorrectly()
    {
        var draw = new DrawResult
        {
            DrawNumber = 1,
            Date = new DateOnly(2025, 1, 4),
            Numbers = [3, 17, 22, 34, 41, 48],
        };
        WriteDrawFile(1, draw);

        var result = new JsonDrawResultRepository(Options).TryGetByEpisode(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.DrawNumber);
        Assert.Equal(new DateOnly(2025, 1, 4), result.Date);
        Assert.Equal([3, 17, 22, 34, 41, 48], result.Numbers);
    }

    [Fact]
    public void GetHistory_EmptyDirectory_ReturnsEmpty()
    {
        var result = new JsonDrawResultRepository(Options).GetHistory();
        Assert.Empty(result);
    }

    [Fact]
    public void GetHistory_MultipleFiles_ReturnsInOrder()
    {
        WriteDrawFile(1, new DrawResult { DrawNumber = 1, Date = new DateOnly(2025, 1, 4), Numbers = [1, 2, 3, 4, 5, 6] });
        WriteDrawFile(2, new DrawResult { DrawNumber = 2, Date = new DateOnly(2025, 1, 11), Numbers = [7, 8, 9, 10, 11, 12] });

        var history = new JsonDrawResultRepository(Options).GetHistory();

        Assert.Equal(2, history.Count);
        Assert.Equal(1, history[0].DrawNumber);
        Assert.Equal(2, history[1].DrawNumber);
    }
}
