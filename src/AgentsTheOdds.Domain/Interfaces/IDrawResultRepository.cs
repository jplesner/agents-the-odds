using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IDrawResultRepository
{
    IReadOnlyList<DrawResult> GetHistory();
    DrawResult? TryGetByEpisode(int episodeNumber);
}
