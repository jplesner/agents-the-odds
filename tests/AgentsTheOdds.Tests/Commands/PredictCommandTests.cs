using AgentsTheOdds.Application.Commands;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using AgentsTheOdds.Domain.Strategies;

namespace AgentsTheOdds.Tests.Commands;

public class PredictCommandTests
{
    // --- fakes ---

    private sealed class FakeAgentRepository(params Agent[] agents) : IAgentRepository
    {
        public IReadOnlyList<Agent> GetAll() => agents;
    }

    private sealed class FakeDrawResultRepository : IDrawResultRepository
    {
        public IReadOnlyList<DrawResult> GetHistory() => [];
        public DrawResult? TryGetByEpisode(int episodeNumber) => null;
    }

    private sealed class FakeEpisodePredictionRepository : IEpisodePredictionRepository
    {
        private readonly Dictionary<int, EpisodePredictionSet> _store = [];

        public bool Exists(int episodeNumber) => _store.ContainsKey(episodeNumber);

        public EpisodePredictionSet Get(int episodeNumber) => _store[episodeNumber];

        public void Save(EpisodePredictionSet set) => _store[set.EpisodeNumber] = set;
    }

    private sealed class FakeLeaderboardRepository : ILeaderboardRepository
    {
        public Leaderboard Load() => Leaderboard.Empty;
        public void Save(Leaderboard leaderboard) { }
    }

    private sealed class FakeEpisodeResultRepository : IEpisodeResultRepository
    {
        public void Save(EpisodeResult result) { }
        public EpisodeResult? TryGet(int episodeNumber) => null;
        public IReadOnlyList<EpisodeResult> GetAll() => [];
    }

    private sealed class InvalidStrategy : IPredictionStrategy
    {
        public Prediction GeneratePrediction(PredictionContext context) => new()
        {
            AgentId = "bad-agent",
            StrategyName = "bad",
            Numbers = [1, 2, 3, 4, 5, 6, 7], // 7 numbers — invalid
            Confidence = 0.5,
            Reasoning = "oops",
        };
    }

    private static Agent ValidAgent(string id) => new()
    {
        Id = id,
        Name = id,
        Behavior = string.Empty,
        Strategy = new StatisticianStrategy(),
    };

    private static Agent InvalidAgent(string id) => new()
    {
        Id = id,
        Name = id,
        Behavior = string.Empty,
        Strategy = new InvalidStrategy(),
    };

    private static PredictCommand BuildCommand(
        IAgentRepository agentRepo,
        FakeEpisodePredictionRepository predRepo) =>
        new(agentRepo, new FakeDrawResultRepository(), new FakeLeaderboardRepository(), predRepo, new FakeEpisodeResultRepository());

    // --- tests ---

    [Fact]
    public void Execute_ValidPredictions_WritesFileAndReturnsZero()
    {
        var predRepo = new FakeEpisodePredictionRepository();
        var cmd = BuildCommand(
            new FakeAgentRepository(ValidAgent("a"), ValidAgent("b")),
            predRepo);

        var exit = cmd.Execute(1, false);

        Assert.Equal(0, exit);
        Assert.True(predRepo.Exists(1));
        Assert.Equal(2, predRepo.Get(1).Predictions.Count);
    }

    [Fact]
    public void Execute_InvalidStrategy_DoesNotWriteFileAndReturnsTwo()
    {
        var predRepo = new FakeEpisodePredictionRepository();
        var cmd = BuildCommand(
            new FakeAgentRepository(ValidAgent("a"), InvalidAgent("bad")),
            predRepo);

        var exit = cmd.Execute(1, false);

        Assert.Equal(2, exit);
        Assert.False(predRepo.Exists(1));
    }

    [Fact]
    public void Execute_FileAlreadyExists_NoForce_ReturnsOneWithoutOverwrite()
    {
        var predRepo = new FakeEpisodePredictionRepository();
        var cmd = BuildCommand(new FakeAgentRepository(ValidAgent("a")), predRepo);
        cmd.Execute(1, false); // first run writes the file

        var secondExisting = predRepo.Get(1);
        var exit = cmd.Execute(1, false); // second run should refuse

        Assert.Equal(1, exit);
        Assert.Equal(secondExisting, predRepo.Get(1)); // unchanged
    }

    [Fact]
    public void Execute_FileAlreadyExists_WithForce_Overwrites()
    {
        var predRepo = new FakeEpisodePredictionRepository();
        var cmd = BuildCommand(new FakeAgentRepository(ValidAgent("a")), predRepo);
        cmd.Execute(1, false);

        var exit = cmd.Execute(1, force: true);

        Assert.Equal(0, exit);
        Assert.True(predRepo.Exists(1));
    }

    [Fact]
    public void Execute_AllInvalidStrategies_DoesNotWriteFile()
    {
        var predRepo = new FakeEpisodePredictionRepository();
        var cmd = BuildCommand(
            new FakeAgentRepository(InvalidAgent("x"), InvalidAgent("y")),
            predRepo);

        var exit = cmd.Execute(1, false);

        Assert.Equal(2, exit);
        Assert.False(predRepo.Exists(1));
    }
}
