using AgentsTheOdds.Application.Commands;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Tests.Commands;

public class DrawCommandTests
{
    private static readonly DrawResult SampleDraw = new()
    {
        DrawNumber = 1,
        Date = new DateOnly(2025, 1, 1),
        Numbers = [3, 17, 22, 34, 41, 48],
    };

    private sealed class FakeDrawService(DrawResult draw, IDrawRepository repo) : IDrawService
    {
        public DrawResult Draw(int episodeNumber)
        {
            repo.RecordDraw(draw);
            return draw;
        }
    }

    private sealed class FakeDrawRepository : IDrawRepository
    {
        private DrawResult? _stored;

        public FakeDrawRepository(DrawResult? existing = null) => _stored = existing;

        public IReadOnlyList<DrawResult> GetHistory() => _stored is null ? [] : [_stored];
        public DrawResult GetCurrent() => _stored ?? throw new InvalidOperationException();
        public void RecordDraw(DrawResult draw) => _stored = draw;
        public DrawResult GetByEpisode(int episodeNumber) =>
            _stored ?? throw new InvalidOperationException($"Draw for episode {episodeNumber} not found.");
    }

    private static DrawCommand Build(FakeDrawRepository repo) =>
        new(new FakeDrawService(SampleDraw, repo), repo);

    [Fact]
    public void Execute_WritesDrawAndReturnsZero()
    {
        var repo = new FakeDrawRepository();
        var exit = Build(repo).Execute(1, false);

        Assert.Equal(0, exit);
        Assert.Equal(SampleDraw.Numbers, repo.GetByEpisode(1).Numbers);
    }

    [Fact]
    public void Execute_AlreadyExists_NoForce_ReturnsOne()
    {
        var repo = new FakeDrawRepository(existing: SampleDraw);
        var exit = Build(repo).Execute(1, false);

        Assert.Equal(1, exit);
    }

    [Fact]
    public void Execute_AlreadyExists_WithForce_Overwrites()
    {
        var repo = new FakeDrawRepository(existing: SampleDraw);
        var exit = Build(repo).Execute(1, force: true);

        Assert.Equal(0, exit);
    }
}
