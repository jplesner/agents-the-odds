using AgentsTheOdds.Domain.Models;
using AgentsTheOdds.Domain.Services;

namespace AgentsTheOdds.Tests.Services;

public class RealityCheckGeneratorTests
{
    private static readonly DrawResult AnyDraw = new()
    {
        DrawNumber = 1,
        Date       = new DateOnly(2025, 1, 1),
        Numbers    = [1, 2, 3, 4, 5, 6],
    };

    private static PredictionResult MakeResult(string agentId, int matches, int points) => new()
    {
        Prediction = new Prediction
        {
            AgentId      = agentId,
            StrategyName = "test",
            Numbers      = [1, 2, 3, 4, 5, 6],
            Confidence   = 0.5,
            Reasoning    = "test",
        },
        Draw    = AnyDraw,
        Matches = matches,
        Points  = points,
    };

    private static readonly RealityCheckGenerator Generator = new();

    [Fact]
    public void Generate_NoScores_ReturnsNoScoresMessage()
    {
        var result = Generator.Generate(1, []);
        Assert.Equal("Episode 1: No predictions were scored.", result);
    }

    [Fact]
    public void Generate_SingleWinner_ReturnsLedWithText()
    {
        var scores = new[]
        {
            MakeResult("alpha", 2, 5),
            MakeResult("beta",  1, 1),
        };
        var result = Generator.Generate(3, scores);
        Assert.Equal("Episode 3: alpha led with 5 pts (2 matches). Combined table points this episode: 6.", result);
    }

    [Fact]
    public void Generate_SingleWinner_OneMatch_UsesMatchSingular()
    {
        var scores = new[] { MakeResult("alpha", 1, 1) };
        var result = Generator.Generate(1, scores);
        Assert.Contains("1 match)", result);
        Assert.DoesNotContain("1 matches", result);
    }

    [Fact]
    public void Generate_TwoWayTie_ReturnsTiedText()
    {
        var scores = new[]
        {
            MakeResult("beta",  1, 1),
            MakeResult("alpha", 1, 1),
        };
        var result = Generator.Generate(2, scores);
        Assert.Equal("Episode 2: alpha and beta tied with 1 pts (1 match each). Combined table points this episode: 2.", result);
    }

    [Fact]
    public void Generate_ThreeWayTie_ReturnsTiedWithOxfordComma()
    {
        var scores = new[]
        {
            MakeResult("charlie", 1, 1),
            MakeResult("alpha",   1, 1),
            MakeResult("beta",    1, 1),
        };
        var result = Generator.Generate(2, scores);
        Assert.Equal("Episode 2: alpha, beta, and charlie tied with 1 pts (1 match each). Combined table points this episode: 3.", result);
    }

    [Fact]
    public void Generate_TotalPoints_SumsAllScores()
    {
        var scores = new[]
        {
            MakeResult("alpha", 2, 5),
            MakeResult("beta",  1, 1),
            MakeResult("gamma", 0, 0),
        };
        var result = Generator.Generate(1, scores);
        Assert.Contains("Combined table points this episode: 6.", result);
    }
}
