using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IEpisodePredictionRepository
{
    bool Exists(int episodeNumber);
    EpisodePredictionSet Get(int episodeNumber);
    void Save(EpisodePredictionSet set);
}
