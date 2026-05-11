using AgentsTheOdds.Domain.Models;
using AgentsTheOdds.Domain.Services;
using AgentsTheOdds.Domain.Strategies;

namespace AgentsTheOdds.Tests;

public class LeaderboardMergerTests
{
    private static Agent MakeAgent(string id, string name) => new()
    {
        Id = id,
        Name = name,
        Behavior = string.Empty,
        Strategy = new StatisticianStrategy(),
    };

    private static PredictionResult MakeScore(string agentId, int points) =>
        new()
        {
            Prediction = new Prediction
            {
                AgentId = agentId,
                StrategyName = "test",
                Numbers = [1, 2, 3, 4, 5, 6],
                Confidence = 0.5,
                Reasoning = string.Empty,
            },
            Draw = new DrawResult { DrawNumber = 1, Date = new DateOnly(2025, 1, 1), Numbers = [1, 2, 3, 4, 5, 6] },
            Matches = 6,
            Points = points,
        };

    [Fact]
    public void Merge_EmptyLeaderboard_BuildsFromNewScores()
    {
        var agents = new[] { MakeAgent("a", "Agent A"), MakeAgent("b", "Agent B") };
        var scores = new[] { MakeScore("a", 100), MakeScore("b", 50) };

        var result = LeaderboardMerger.Merge(Leaderboard.Empty, scores, agents);

        Assert.Equal(2, result.Entries.Count);
        Assert.Equal("a", result.Entries[0].AgentId);
        Assert.Equal(100, result.Entries[0].TotalPoints);
        Assert.Equal(1, result.Entries[0].Rank);
        Assert.Equal("b", result.Entries[1].AgentId);
        Assert.Equal(50, result.Entries[1].TotalPoints);
        Assert.Equal(2, result.Entries[1].Rank);
    }

    [Fact]
    public void Merge_ExistingLeaderboard_AccumulatesPoints()
    {
        var agents = new[] { MakeAgent("a", "Agent A") };
        var existing = new Leaderboard
        {
            Entries = [new LeaderboardEntry { AgentId = "a", AgentName = "Agent A", TotalPoints = 50, Rank = 1 }],
        };
        var scores = new[] { MakeScore("a", 10) };

        var result = LeaderboardMerger.Merge(existing, scores, agents);

        Assert.Equal(60, result.Entries[0].TotalPoints);
    }

    [Fact]
    public void Merge_ReranksAfterMerge()
    {
        var agents = new[] { MakeAgent("a", "A"), MakeAgent("b", "B") };
        var existing = new Leaderboard
        {
            Entries =
            [
                new LeaderboardEntry { AgentId = "a", AgentName = "A", TotalPoints = 100, Rank = 1 },
                new LeaderboardEntry { AgentId = "b", AgentName = "B", TotalPoints = 50, Rank = 2 },
            ],
        };
        var scores = new[] { MakeScore("a", 0), MakeScore("b", 1000) };

        var result = LeaderboardMerger.Merge(existing, scores, agents);

        Assert.Equal("b", result.Entries[0].AgentId);
        Assert.Equal(1, result.Entries[0].Rank);
        Assert.Equal(1050, result.Entries[0].TotalPoints);
        Assert.Equal("a", result.Entries[1].AgentId);
        Assert.Equal(2, result.Entries[1].Rank);
    }

    [Fact]
    public void Merge_NewAgentNotInExistingLeaderboard_AddsEntry()
    {
        var agents = new[] { MakeAgent("a", "A"), MakeAgent("new", "Newcomer") };
        var existing = new Leaderboard
        {
            Entries = [new LeaderboardEntry { AgentId = "a", AgentName = "A", TotalPoints = 100, Rank = 1 }],
        };
        var scores = new[] { MakeScore("a", 10), MakeScore("new", 50) };

        var result = LeaderboardMerger.Merge(existing, scores, agents);

        Assert.Equal(2, result.Entries.Count);
        Assert.Contains(result.Entries, e => e.AgentId == "new" && e.TotalPoints == 50);
    }
}
