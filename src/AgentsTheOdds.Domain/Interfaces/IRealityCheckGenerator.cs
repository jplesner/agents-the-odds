using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IRealityCheckGenerator
{
    string Generate(int episodeNumber, IReadOnlyList<PredictionResult> scores, IReadOnlyList<Agent> agents);
}
