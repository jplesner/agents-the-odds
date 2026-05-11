using AgentsTheOdds.Application.Commands;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using AgentsTheOdds.Domain.Strategies;

namespace AgentsTheOdds.Tests.Commands;

public class ScoreCommandTests
{
    // --- fakes ---

    private sealed class FakeAgentRepository(params Agent[] agents) : IAgentRepository
    {
        public IReadOnlyList<Agent> GetAll() => agents;
    }

    private sealed class FakeDrawRepository(DrawResult? draw = null) : IDrawRepository
    {
        public IReadOnlyList<DrawResult> GetHistory() => draw is null ? [] : [draw];
        public DrawResult GetCurrent() => draw ?? throw new InvalidOperationException("No draw.");
        public void RecordDraw(DrawResult d) { }
        public DrawResult GetByEpisode(int episodeNumber) =>
            draw ?? throw new InvalidOperationException($"Draw for episode {episodeNumber} not found.");
    }

    private sealed class FakeEpisodePredictionRepository(EpisodePredictionSet? set) : IEpisodePredictionRepository
    {
        public bool Exists(int episodeNumber) => set is not null && set.EpisodeNumber == episodeNumber;
        public EpisodePredictionSet Get(int episodeNumber) => set!;
        public void Save(EpisodePredictionSet s) { }
    }

    private sealed class FakeLeaderboardRepository : ILeaderboardRepository
    {
        public Leaderboard Saved { get; private set; } = Leaderboard.Empty;
        public Leaderboard Load() => Saved;
        public void Save(Leaderboard leaderboard) => Saved = leaderboard;
    }

    private sealed class FakeEpisodeResultRepository : IEpisodeResultRepository
    {
        public EpisodeResult? Saved { get; private set; }
        public void Save(EpisodeResult result) => Saved = result;
        public EpisodeResult? TryGet(int episodeNumber) => Saved;
        public IReadOnlyList<EpisodeResult> GetAll() => Saved is null ? [] : [Saved];
    }

    private sealed class FakeRecapWriter : IRecapWriter
    {
        public EpisodeResult? LastWritten { get; private set; }
        public void Write(EpisodeResult result) => LastWritten = result;
    }

    private sealed class FakeRealityCheckGenerator : IRealityCheckGenerator
    {
        public string Generate(int episodeNumber, IReadOnlyList<PredictionResult> scores) =>
            $"Episode {episodeNumber} reality check.";
    }

    private static Agent ValidAgent(string id) => new()
    {
        Id = id,
        Name = $"Agent {id}",
        Behavior = string.Empty,
        Strategy = new StatisticianStrategy(),
    };

    private static Prediction MakePrediction(string agentId, IReadOnlyList<int> numbers) => new()
    {
        AgentId = agentId,
        StrategyName = "test",
        Numbers = numbers,
        Confidence = 0.5,
        Reasoning = string.Empty,
    };

    private static DrawResult MakeDraw(IReadOnlyList<int> numbers) => new()
    {
        DrawNumber = 1,
        Date = new DateOnly(2025, 1, 1),
        Numbers = numbers,
    };

    private static ScoreCommand BuildCommand(
        IAgentRepository agents,
        IEpisodePredictionRepository predictions,
        FakeDrawRepository draws,
        FakeLeaderboardRepository leaderboards,
        FakeEpisodeResultRepository episodeResults,
        FakeRecapWriter recapWriter) =>
        new(agents, predictions, draws, leaderboards, episodeResults, recapWriter, new FakeRealityCheckGenerator());

    // --- tests ---

    [Fact]
    public void Execute_MissingDraw_ReturnsOne()
    {
        var predSet = new EpisodePredictionSet
        {
            EpisodeNumber = 1,
            PredictionDate = new DateOnly(2025, 1, 1),
            Predictions = [MakePrediction("a", [1, 2, 3, 4, 5, 6])],
        };
        var episodeResults = new FakeEpisodeResultRepository();

        var cmd = BuildCommand(
            new FakeAgentRepository(ValidAgent("a")),
            new FakeEpisodePredictionRepository(predSet),
            new FakeDrawRepository(),   // no draw
            new FakeLeaderboardRepository(),
            episodeResults,
            new FakeRecapWriter());

        Assert.Equal(1, cmd.Execute(1));
        Assert.Null(episodeResults.Saved);
    }

    [Fact]
    public void Execute_ValidEpisode_ScoresAndSaves()
    {
        var predSet = new EpisodePredictionSet
        {
            EpisodeNumber = 1,
            PredictionDate = new DateOnly(2025, 1, 1),
            Predictions = [MakePrediction("a", [1, 2, 3, 4, 5, 6])],
        };
        var episodeResults = new FakeEpisodeResultRepository();
        var recapWriter = new FakeRecapWriter();
        var leaderboards = new FakeLeaderboardRepository();

        var cmd = BuildCommand(
            new FakeAgentRepository(ValidAgent("a")),
            new FakeEpisodePredictionRepository(predSet),
            new FakeDrawRepository(MakeDraw([1, 2, 3, 7, 8, 9])),
            leaderboards,
            episodeResults,
            recapWriter);

        var exit = cmd.Execute(1);

        Assert.Equal(0, exit);
        Assert.NotNull(episodeResults.Saved);
        Assert.NotNull(recapWriter.LastWritten);
        Assert.NotNull(leaderboards.Saved);
    }

    [Fact]
    public void Execute_MissingPredictions_ReturnsOne()
    {
        var episodeResults = new FakeEpisodeResultRepository();

        var cmd = BuildCommand(
            new FakeAgentRepository(ValidAgent("a")),
            new FakeEpisodePredictionRepository(null),
            new FakeDrawRepository(MakeDraw([1, 2, 3, 4, 5, 6])),
            new FakeLeaderboardRepository(),
            episodeResults,
            new FakeRecapWriter());

        var exit = cmd.Execute(1);

        Assert.Equal(1, exit);
        Assert.Null(episodeResults.Saved);
    }

    [Fact]
    public void Execute_CalculatesMatchesCorrectly()
    {
        // 3 matches between [1,2,3,4,5,6] and [1,2,3,7,8,9]
        var predSet = new EpisodePredictionSet
        {
            EpisodeNumber = 1,
            PredictionDate = new DateOnly(2025, 1, 1),
            Predictions = [MakePrediction("a", [1, 2, 3, 4, 5, 6])],
        };
        var episodeResults = new FakeEpisodeResultRepository();

        var cmd = BuildCommand(
            new FakeAgentRepository(ValidAgent("a")),
            new FakeEpisodePredictionRepository(predSet),
            new FakeDrawRepository(MakeDraw([1, 2, 3, 7, 8, 9])),
            new FakeLeaderboardRepository(),
            episodeResults,
            new FakeRecapWriter());

        cmd.Execute(1);

        var score = episodeResults.Saved!.Scores[0];
        Assert.Equal(3, score.Matches);
        Assert.Equal(10, score.Points);
    }

    [Fact]
    public void Execute_UpdatesLeaderboardCumulativePoints()
    {
        var predSet = new EpisodePredictionSet
        {
            EpisodeNumber = 1,
            PredictionDate = new DateOnly(2025, 1, 1),
            Predictions = [MakePrediction("a", [1, 2, 3, 4, 5, 6])],
        };
        var leaderboards = new FakeLeaderboardRepository();

        var cmd = BuildCommand(
            new FakeAgentRepository(ValidAgent("a")),
            new FakeEpisodePredictionRepository(predSet),
            new FakeDrawRepository(MakeDraw([1, 2, 3, 4, 5, 6])), // 6 matches = 1000 pts
            leaderboards,
            new FakeEpisodeResultRepository(),
            new FakeRecapWriter());

        cmd.Execute(1);

        Assert.Equal(1000, leaderboards.Saved.Entries[0].TotalPoints);
    }

    [Fact]
    public void Execute_WritesRecapFileWithCorrectEpisodeNumber()
    {
        var predSet = new EpisodePredictionSet
        {
            EpisodeNumber = 7,
            PredictionDate = new DateOnly(2025, 1, 1),
            Predictions = [MakePrediction("a", [1, 2, 3, 4, 5, 6])],
        };
        var recapWriter = new FakeRecapWriter();

        var cmd = BuildCommand(
            new FakeAgentRepository(ValidAgent("a")),
            new FakeEpisodePredictionRepository(predSet),
            new FakeDrawRepository(MakeDraw([10, 11, 12, 13, 14, 15])),
            new FakeLeaderboardRepository(),
            new FakeEpisodeResultRepository(),
            recapWriter);

        cmd.Execute(7);

        Assert.NotNull(recapWriter.LastWritten);
        Assert.Equal(7, recapWriter.LastWritten!.EpisodeNumber);
    }
}
