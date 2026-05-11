using AgentsTheOdds.Domain.Interfaces;

namespace AgentsTheOdds.Domain.Models;

public sealed class Agent
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Behavior { get; init; }
    public required IPredictionStrategy Strategy { get; init; }
}
