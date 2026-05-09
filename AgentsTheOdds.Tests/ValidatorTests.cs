using AgentsTheOdds.Domain;
using AgentsTheOdds.Models;

namespace AgentsTheOdds.Tests;

public class ValidatorTests
{
    private static readonly LotteryRules Rules = LotteryRules.Standard;

    private static Prediction MakePrediction(
        IReadOnlyList<int> numbers,
        double confidence = 0.5) => new()
    {
        AgentId      = "test",
        StrategyName = "test",
        Numbers      = numbers,
        Confidence   = confidence,
        Reasoning    = "test",
    };

    [Fact]
    public void Validate_ValidPrediction_IsValid()
    {
        var result = LotteryValidator.Validate(MakePrediction([1, 7, 14, 28, 35, 49]), Rules);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_TooFewNumbers_IsInvalid()
    {
        var result = LotteryValidator.Validate(MakePrediction([1, 2, 3, 4, 5]), Rules);
        Assert.False(result.IsValid);
        Assert.Contains("5", result.Error);
    }

    [Fact]
    public void Validate_TooManyNumbers_IsInvalid()
    {
        var result = LotteryValidator.Validate(MakePrediction([1, 2, 3, 4, 5, 6, 7]), Rules);
        Assert.False(result.IsValid);
        Assert.Contains("7", result.Error);
    }

    [Fact]
    public void Validate_NumberBelowMin_IsInvalid()
    {
        var result = LotteryValidator.Validate(MakePrediction([0, 2, 3, 4, 5, 6]), Rules);
        Assert.False(result.IsValid);
        Assert.Contains("0", result.Error);
    }

    [Fact]
    public void Validate_NumberAboveMax_IsInvalid()
    {
        var result = LotteryValidator.Validate(MakePrediction([1, 2, 3, 4, 5, 50]), Rules);
        Assert.False(result.IsValid);
        Assert.Contains("50", result.Error);
    }

    [Fact]
    public void Validate_DuplicateNumbers_IsInvalid()
    {
        var result = LotteryValidator.Validate(MakePrediction([1, 2, 3, 4, 5, 5]), Rules);
        Assert.False(result.IsValid);
        Assert.Contains("unique", result.Error);
    }

    [Fact]
    public void Validate_ConfidenceAbove1_IsInvalid()
    {
        var result = LotteryValidator.Validate(MakePrediction([1, 2, 3, 4, 5, 6], confidence: 1.01), Rules);
        Assert.False(result.IsValid);
        Assert.Contains("1.01", result.Error);
    }

    [Fact]
    public void Validate_ConfidenceBelow0_IsInvalid()
    {
        var result = LotteryValidator.Validate(MakePrediction([1, 2, 3, 4, 5, 6], confidence: -0.01), Rules);
        Assert.False(result.IsValid);
        Assert.Contains("-0.01", result.Error);
    }

    [Fact]
    public void Validate_ConfidenceAtBoundaries_IsValid()
    {
        Assert.True(LotteryValidator.Validate(MakePrediction([1, 2, 3, 4, 5, 6], confidence: 0.0), Rules).IsValid);
        Assert.True(LotteryValidator.Validate(MakePrediction([1, 2, 3, 4, 5, 6], confidence: 1.0), Rules).IsValid);
    }
}
