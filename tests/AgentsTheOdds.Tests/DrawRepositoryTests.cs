using AgentsTheOdds.Data;
using AgentsTheOdds.Domain;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Tests;

public class DrawRepositoryTests
{
    private static readonly LotteryRules Rules = LotteryRules.Standard;

    [Fact]
    public void GetCurrent_NumbersAreValidPerLotteryRules()
    {
        var repo = new InMemoryDrawRepository();
        var draw = repo.GetCurrent();

        var prediction = new Prediction
        {
            AgentId      = "test",
            StrategyName = "test",
            Numbers      = draw.Numbers,
            Confidence   = 0.5,
            Reasoning    = "test",
        };

        var result = LotteryValidator.Validate(prediction, Rules);
        Assert.True(result.IsValid, result.Error);
    }
}
