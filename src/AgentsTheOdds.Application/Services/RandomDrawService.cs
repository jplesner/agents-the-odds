using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Application.Services;

public sealed class RandomDrawService(IDrawRepository draws) : IDrawService
{
    public DrawResult Draw(int episodeNumber)
    {
        var rules = LotteryRules.Standard;
        var numbers = Enumerable.Range(rules.MinNumber, rules.MaxNumber - rules.MinNumber + 1)
            .OrderBy(_ => Random.Shared.Next())
            .Take(rules.DrawCount)
            .Order()
            .ToArray();

        var draw = new DrawResult
        {
            DrawNumber = episodeNumber,
            Date = DateOnly.FromDateTime(DateTime.Today),
            Numbers = numbers,
        };

        draws.RecordDraw(draw);
        return draw;
    }
}
