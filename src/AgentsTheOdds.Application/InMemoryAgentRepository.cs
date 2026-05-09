using AgentsTheOdds.Application.Agents;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Application;

public sealed class InMemoryAgentRepository : IAgentRepository
{
    private static readonly IReadOnlyList<Agent> Agents =
    [
        new Agent
        {
            Id          = "statistician",
            Name        = "The Statistician",
            Personality = "Calm, data-driven, trusts the numbers above all else.",
            Strategy    = new StatisticianStrategy()
        },
        new Agent
        {
            Id          = "pattern-goblin",
            Name        = "The Pattern Goblin",
            Personality = "Sees patterns everywhere. Possibly unhinged.",
            Strategy    = new PatternGoblinStrategy()
        },
        new Agent
        {
            Id          = "skeptic",
            Name        = "The Skeptic",
            Personality = "Doubts everything, including itself.",
            Strategy    = new SkepticStrategy()
        },
        new Agent
        {
            Id          = "random-baseline",
            Name        = "Random Baseline",
            Personality = "No personality. Pure chaos.",
            Strategy    = new RandomBaselineStrategy()
        },
    ];

    public IReadOnlyList<Agent> GetAll() => Agents;
}
