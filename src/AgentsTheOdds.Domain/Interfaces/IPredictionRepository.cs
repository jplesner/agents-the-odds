using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IPredictionRepository
{
    void Add(PredictionResult result);
    IReadOnlyList<PredictionResult> GetAll();
}
