using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain;

public static class LotteryValidator
{
    public static ValidationResult Validate(Prediction prediction, LotteryRules rules)
    {
        if (prediction.Numbers.Count != rules.DrawCount)
            return ValidationResult.Fail(
                $"Expected {rules.DrawCount} numbers, got {prediction.Numbers.Count}.");

        var outOfRange = prediction.Numbers
            .Where(n => n < rules.MinNumber || n > rules.MaxNumber)
            .ToList();
        if (outOfRange.Count > 0)
            return ValidationResult.Fail(
                $"Numbers out of range [{rules.MinNumber},{rules.MaxNumber}]: {string.Join(", ", outOfRange)}.");

        if (prediction.Numbers.Distinct().Count() != prediction.Numbers.Count)
            return ValidationResult.Fail("Numbers must be unique.");

        if (prediction.Confidence is < 0.0 or > 1.0)
            return ValidationResult.Fail(
                $"Confidence {prediction.Confidence:F2} is outside [0.0, 1.0].");

        return ValidationResult.Ok();
    }
}
