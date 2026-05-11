using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface ILeaderboardRepository
{
    Leaderboard Load();
    void Save(Leaderboard leaderboard);
}
