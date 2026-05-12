using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Domain;

public static class Scorer
{
    private static readonly IReadOnlyDictionary<int, int> PointsTable = new Dictionary<int, int>
    {
        [6] = 1000,
        [5] = 100,
        [4] = 50,
        [3] = 10,
        [2] = 5,
        [1] = 1
    };

    public static PredictionResult Score(Prediction prediction, DrawResult draw)
    {
        var matches = prediction.Numbers.Intersect(draw.Numbers).Count();
        var points = PointsFor(matches);

        return new PredictionResult
        {
            Prediction = prediction,
            Draw       = draw,
            Matches    = matches,
            Points     = points,
        };
    }

    public static int PointsFor(int matches) =>
        PointsTable.GetValueOrDefault(matches, 0);
}
