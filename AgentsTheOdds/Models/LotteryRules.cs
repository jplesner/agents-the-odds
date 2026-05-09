namespace AgentsTheOdds.Models;

public sealed class LotteryRules
{
    public int MinNumber { get; init; } = 1;
    public int MaxNumber { get; init; } = 49;
    public int DrawCount { get; init; } = 6;

    public static readonly LotteryRules Standard = new();
}
