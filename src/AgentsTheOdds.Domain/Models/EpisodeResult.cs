namespace AgentsTheOdds.Domain.Models;

public sealed class EpisodeResult
{
    public required int EpisodeNumber { get; init; }
    public required DrawResult DrawResult { get; init; }
    public required IReadOnlyList<PredictionResult> Scores { get; init; }
    public required IReadOnlyList<LeaderboardEntry> Leaderboard { get; init; }
    public required string RealityCheck { get; init; }
}
