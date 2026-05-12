using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using AgentsTheOdds.Domain.Strategies;

namespace AgentsTheOdds.Data;

public sealed class InMemoryAgentRepository : IAgentRepository
{
    private static readonly IReadOnlyList<Agent> Agents =
    [
        new() { Id = "statistician",   Name = "The Statistician",   Strategy = new StatisticianStrategy()  },
        new() { Id = "pattern-goblin", Name = "The Pattern Goblin", Strategy = new PatternGoblinStrategy() },
        new() { Id = "skeptic",        Name = "The Skeptic",        Strategy = new SkepticStrategy()       },
    ];

    public IReadOnlyList<Agent> GetAll() => Agents;
}
