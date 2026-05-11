using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IEpisodeResultRepository
{
    void Save(EpisodeResult result);
    EpisodeResult? TryGet(int episodeNumber);
    IReadOnlyList<EpisodeResult> GetAll();
}
