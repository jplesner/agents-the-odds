namespace AgentsTheOdds.Domain.Models;

public sealed class PredictionContext
{
    public required LotteryRules Rules { get; init; }
    public required IReadOnlyList<DrawResult> DrawHistory { get; init; }
    public required IReadOnlyList<PredictionResult> AgentHistory { get; init; }
    public required Leaderboard Leaderboard { get; init; }
}
