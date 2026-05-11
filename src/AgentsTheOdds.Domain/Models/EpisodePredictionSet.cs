namespace AgentsTheOdds.Domain.Models;

public sealed class EpisodePredictionSet
{
    public required int EpisodeNumber { get; init; }
    public required DateOnly PredictionDate { get; init; }
    public required IReadOnlyList<Prediction> Predictions { get; init; }
}
