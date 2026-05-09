namespace AgentsTheOdds.Models;

public sealed class PredictionResult
{
    public required Prediction Prediction { get; init; }
    public required DrawResult Draw { get; init; }
    public int Matches { get; init; }
    public int Points { get; init; }
}
