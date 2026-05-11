using AgentsTheOdds.Data.Storage;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Tests.Storage;

public class JsonDrawRepositoryTests : IDisposable
{
    private readonly string _dataRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private DataRootOptions Options => new() { Path = _dataRoot };

    public void Dispose() { if (Directory.Exists(_dataRoot)) Directory.Delete(_dataRoot, recursive: true); }

    private static DrawResult MakeDraw(int episode) => new()
    {
        DrawNumber = episode,
        Date = new DateOnly(2025, 1, 4),
        Numbers = [3, 17, 22, 34, 41, 48],
    };

    [Fact]
    public void RecordDraw_CreatesFile()
    {
        var repo = new JsonDrawRepository(Options);
        repo.RecordDraw(MakeDraw(1));
        Assert.True(File.Exists(Path.Combine(_dataRoot, "draws", "episode-001.json")));
    }

    [Fact]
    public void GetByEpisode_AfterRecord_RoundTrips()
    {
        var repo = new JsonDrawRepository(Options);
        repo.RecordDraw(MakeDraw(1));

        var result = repo.GetByEpisode(1);

        Assert.Equal(1, result.DrawNumber);
        Assert.Equal(new DateOnly(2025, 1, 4), result.Date);
        Assert.Equal([3, 17, 22, 34, 41, 48], result.Numbers);
    }

    [Fact]
    public void GetByEpisode_Missing_Throws()
    {
        var repo = new JsonDrawRepository(Options);
        Assert.Throws<InvalidOperationException>(() => repo.GetByEpisode(99));
    }

    [Fact]
    public void GetHistory_ReturnsInOrder()
    {
        var repo = new JsonDrawRepository(Options);
        repo.RecordDraw(MakeDraw(1));
        repo.RecordDraw(MakeDraw(2));

        var history = repo.GetHistory();

        Assert.Equal(2, history.Count);
        Assert.Equal(1, history[0].DrawNumber);
        Assert.Equal(2, history[1].DrawNumber);
    }
}
