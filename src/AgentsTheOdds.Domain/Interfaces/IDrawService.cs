using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IDrawService
{
    DrawResult Draw(int episodeNumber);
}
