using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IPredictionStrategy
{
    Prediction GeneratePrediction(PredictionContext context);
}
