using AgentsTheOdds.Data.Storage;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Tests.Storage;

public class JsonEpisodePredictionRepositoryTests : IDisposable
{
    private readonly string _dataRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    private DataRootOptions Options => new() { Path = _dataRoot };

    public void Dispose() { if (Directory.Exists(_dataRoot)) Directory.Delete(_dataRoot, recursive: true); }

    private static EpisodePredictionSet MakeSet(int episode) => new()
    {
        EpisodeNumber = episode,
        PredictionDate = new DateOnly(2025, 1, 4),
        Predictions =
        [
            new Prediction
            {
                AgentId = "statistician",
                StrategyName = "test",
                Numbers = [7, 14, 21, 28, 35, 44],
                Confidence = 0.61,
                Reasoning = "test reasoning",
            },
        ],
    };

    [Fact]
    public void Exists_WhenNoFile_ReturnsFalse()
    {
        Assert.False(new JsonEpisodePredictionRepository(Options).Exists(1));
    }

    [Fact]
    public void Save_CreatesFileAndExistsReturnsTrue()
    {
        var repo = new JsonEpisodePredictionRepository(Options);
        repo.Save(MakeSet(1));
        Assert.True(repo.Exists(1));
    }

    [Fact]
    public void Get_AfterSave_RoundTripsCorrectly()
    {
        var repo = new JsonEpisodePredictionRepository(Options);
        var original = MakeSet(1);
        repo.Save(original);

        var loaded = repo.Get(1);

        Assert.Equal(1, loaded.EpisodeNumber);
        Assert.Equal(new DateOnly(2025, 1, 4), loaded.PredictionDate);
        Assert.Single(loaded.Predictions);
        Assert.Equal("statistician", loaded.Predictions[0].AgentId);
        Assert.Equal([7, 14, 21, 28, 35, 44], loaded.Predictions[0].Numbers);
    }

    [Fact]
    public void Get_MissingFile_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() =>
            new JsonEpisodePredictionRepository(Options).Get(99));
    }
}
