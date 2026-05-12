using AgentsTheOdds.Data;
using AgentsTheOdds.Domain;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Tests;

public class StrategyTests
{
    private static readonly DrawResult SampleDraw = new()
    {
        DrawNumber = 1,
        Date = new DateOnly(2025, 1, 1),
        Numbers = [4, 15, 23, 31, 38, 42],
    };

    private static PredictionContext EmptyContext(string agentId) => new()
    {
        Rules = LotteryRules.Standard,
        DrawHistory = [],
        AgentHistory = [],
        Leaderboard = Leaderboard.Empty,
    };

    private static PredictionContext ContextWithHistory(string agentId) => new()
    {
        Rules = LotteryRules.Standard,
        DrawHistory = [SampleDraw],
        AgentHistory =
        [
            new PredictionResult
            {
                Draw = SampleDraw,
                Prediction = new Prediction
                {
                    AgentId = agentId,
                    StrategyName = "test",
                    Numbers = [1, 2, 3, 4, 5, 6],
                    Confidence = 0.5,
                    Reasoning = "test",
                },
                Matches = 1,
                Points = 1,
            },
        ],
        Leaderboard = Leaderboard.Empty,
    };

    public static IEnumerable<object[]> AllAgentStrategies()
    {
        foreach (var agent in new InMemoryAgentRepository().GetAll())
        {
            yield return [agent.Id, agent.Strategy, EmptyContext(agent.Id)];
            yield return [agent.Id, agent.Strategy, ContextWithHistory(agent.Id)];
        }
    }

    [Theory]
    [MemberData(nameof(AllAgentStrategies))]
    public void GeneratePrediction_ProducesValidPrediction(
        string agentId, IPredictionStrategy strategy, PredictionContext context)
    {
        var prediction = strategy.GeneratePrediction(context);
        var result = LotteryValidator.Validate(prediction, LotteryRules.Standard);
        Assert.True(result.IsValid, $"[{agentId}] {result.Error}");
    }
}
