using AgentsTheOdds.Data.File;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Tests.Storage;

public class JsonLeaderboardRepositoryTests : IDisposable
{
    private readonly string _dataRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private DataRootOptions Options => new() { Path = _dataRoot };

    public void Dispose() { if (Directory.Exists(_dataRoot)) Directory.Delete(_dataRoot, recursive: true); }

    [Fact]
    public void Load_WhenNoFile_ReturnsEmpty()
    {
        var result = new JsonLeaderboardRepository(Options).Load();
        Assert.Empty(result.Entries);
    }

    [Fact]
    public void Save_ThenLoad_RoundTripsCorrectly()
    {
        var repo = new JsonLeaderboardRepository(Options);
        var board = new Leaderboard
        {
            Entries =
            [
                new LeaderboardEntry { AgentId = "a", AgentName = "Agent A", TotalPoints = 100, Rank = 1 },
            ],
        };

        repo.Save(board);
        var loaded = repo.Load();

        Assert.Single(loaded.Entries);
        Assert.Equal("a", loaded.Entries[0].AgentId);
        Assert.Equal(100, loaded.Entries[0].TotalPoints);
    }

    [Fact]
    public void Save_CreatesDirectoryIfMissing()
    {
        var repo = new JsonLeaderboardRepository(Options);
        repo.Save(Leaderboard.Empty);
        Assert.True(File.Exists(Path.Combine(_dataRoot, "leaderboard.json")));
    }
}
