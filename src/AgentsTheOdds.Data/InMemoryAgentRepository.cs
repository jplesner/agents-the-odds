using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using AgentsTheOdds.Domain.Strategies;

namespace AgentsTheOdds.Data;

public sealed class InMemoryAgentRepository : IAgentRepository
{
    private static readonly IReadOnlyList<Agent> Agents =
    [
        new Agent
        {
            Id          = "statistician",
            Name        = "The Statistician",
            Behavior    = File.ReadAllText("./Agents/Statistician/personality.md"),
            Strategy    = new StatisticianStrategy()
        },
        new Agent
        {
            Id          = "pattern-goblin",
            Name        = "The Pattern Goblin",
            Behavior = File.ReadAllText("Agents/Pattern-Goblin/personality.md"),
            Strategy    = new PatternGoblinStrategy()
        },
        new Agent
        {
            Id          = "skeptic",
            Name        = "The Skeptic",
            Behavior = File.ReadAllText("Agents/Skeptic/personality.md"),
            Strategy    = new SkepticStrategy()
        }
    ];

    public IReadOnlyList<Agent> GetAll() => Agents;
}
