using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain.Interfaces;

public interface IAgentRepository
{
    IReadOnlyList<Agent> GetAll();
}
