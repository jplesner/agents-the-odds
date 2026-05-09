using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Data;

public sealed class InMemoryPredictionRepository : IPredictionRepository
{
    private readonly List<PredictionResult> _results = [];

    public void Add(PredictionResult result) => _results.Add(result);
    public IReadOnlyList<PredictionResult> GetAll() => _results;
}
