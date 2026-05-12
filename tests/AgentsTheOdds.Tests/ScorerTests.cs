using AgentsTheOdds.Domain;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Tests;

public class ScorerTests
{
    private static readonly DrawResult Draw = new()
    {
        DrawNumber = 1,
        Date       = new DateOnly(2025, 1, 1),
        Numbers    = [1, 2, 3, 4, 5, 6],
    };

    private static Prediction MakePrediction(IReadOnlyList<int> numbers) => new()
    {
        AgentId      = "test",
        StrategyName = "test",
        Numbers      = numbers,
        Confidence   = 0.5,
        Reasoning    = "test",
    };

    [Fact]
    public void Score_SixMatches_Returns1000Points()
    {
        var result = Scorer.Score(MakePrediction([1, 2, 3, 4, 5, 6]), Draw);
        Assert.Equal(6, result.Matches);
        Assert.Equal(1000, result.Points);
    }

    [Fact]
    public void Score_FiveMatches_Returns100Points()
    {
        var result = Scorer.Score(MakePrediction([1, 2, 3, 4, 5, 7]), Draw);
        Assert.Equal(5, result.Matches);
        Assert.Equal(100, result.Points);
    }

    [Fact]
    public void Score_FourMatches_Returns50Points()
    {
        var result = Scorer.Score(MakePrediction([1, 2, 3, 4, 8, 9]), Draw);
        Assert.Equal(4, result.Matches);
        Assert.Equal(50, result.Points);
    }

    [Fact]
    public void Score_ThreeMatches_Returns10Points()
    {
        var result = Scorer.Score(MakePrediction([1, 2, 3, 7, 8, 9]), Draw);
        Assert.Equal(3, result.Matches);
        Assert.Equal(10, result.Points);
    }

    [Fact]
    public void Score_TwoMatches_Returns5Points()
    {
        var result = Scorer.Score(MakePrediction([1, 2, 7, 8, 9, 10]), Draw);
        Assert.Equal(2, result.Matches);
        Assert.Equal(5, result.Points);
    }

    [Fact]
    public void Score_OneMatch_Returns1Point()
    {
        var result = Scorer.Score(MakePrediction([1, 7, 8, 9, 10, 11]), Draw);
        Assert.Equal(1, result.Matches);
        Assert.Equal(1, result.Points);
    }

    [Fact]
    public void Score_ZeroMatches_Returns0Points()
    {
        var result = Scorer.Score(MakePrediction([7, 8, 9, 10, 11, 12]), Draw);
        Assert.Equal(0, result.Matches);
        Assert.Equal(0, result.Points);
    }

    [Fact]
    public void Score_AttachesPredictionAndDraw()
    {
        var prediction = MakePrediction([1, 2, 3, 4, 5, 6]);
        var result = Scorer.Score(prediction, Draw);
        Assert.Same(prediction, result.Prediction);
        Assert.Same(Draw, result.Draw);
    }
}
