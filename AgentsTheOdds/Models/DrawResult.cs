namespace AgentsTheOdds.Models;

public sealed class DrawResult
{
    public int DrawNumber { get; init; }
    public DateOnly Date { get; init; }
    public required IReadOnlyList<int> Numbers { get; init; }
}
