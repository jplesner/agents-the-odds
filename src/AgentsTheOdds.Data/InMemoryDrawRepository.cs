using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Data;

public sealed class InMemoryDrawRepository : IDrawRepository
{
    private static readonly IReadOnlyList<DrawResult> History =
    [
        new DrawResult { DrawNumber = 1, Date = new DateOnly(2025, 1,  4), Numbers = [3, 17, 22, 34, 41, 48] },
        new DrawResult { DrawNumber = 2, Date = new DateOnly(2025, 1, 11), Numbers = [5, 11, 25, 31, 38, 46] },
        new DrawResult { DrawNumber = 3, Date = new DateOnly(2025, 1, 18), Numbers = [7, 14, 21, 28, 35, 42] },
    ];

    public IReadOnlyList<DrawResult> GetHistory() => History;

    public DrawResult GetCurrent()
    {
        var last = History[^1];
        var numbers = Enumerable.Range(1, 49)
            .OrderBy(_ => Random.Shared.Next())
            .Take(6)
            .Order()
            .ToArray();
        return new DrawResult { DrawNumber = last.DrawNumber + 1, Date = DateOnly.FromDateTime(DateTime.Today), Numbers = numbers };
    }
}
