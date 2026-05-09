namespace AgentsTheOdds.Models;

public sealed class Prediction
{
    public required string AgentId { get; init; }
    public required string StrategyName { get; init; }
    public required IReadOnlyList<int> Numbers { get; init; }
    public required double Confidence { get; init; }
    public required string Reasoning { get; init; }
}
