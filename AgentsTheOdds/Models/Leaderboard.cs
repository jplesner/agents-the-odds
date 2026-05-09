namespace AgentsTheOdds.Models;

public sealed class Leaderboard
{
    public required IReadOnlyList<LeaderboardEntry> Entries { get; init; }

    public static readonly Leaderboard Empty = new() { Entries = [] };
}

public sealed class LeaderboardEntry
{
    public required string AgentId { get; init; }
    public required string AgentName { get; init; }
    public int TotalPoints { get; init; }
    public int Rank { get; init; }
}
