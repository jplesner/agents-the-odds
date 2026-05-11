using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IDrawRepository
{
    IReadOnlyList<DrawResult> GetHistory();
    DrawResult GetCurrent();
    void RecordDraw(DrawResult draw);
    DrawResult GetByEpisode(int episodeNumber);
}
