using AgentsTheOdds.Models;

namespace AgentsTheOdds.Strategies;

public interface IPredictionStrategy
{
    Prediction GeneratePrediction(PredictionContext context);
}
