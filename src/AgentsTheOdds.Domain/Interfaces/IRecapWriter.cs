using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IRecapWriter
{
    void Write(EpisodeResult result);
}
