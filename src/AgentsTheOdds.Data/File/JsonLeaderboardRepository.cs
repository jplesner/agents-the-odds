using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using System.Text.Json;

namespace AgentsTheOdds.Data.Storage;

public sealed class JsonLeaderboardRepository(DataRootOptions options) : ILeaderboardRepository
{
    private string FilePath => Path.Combine(options.Path, "leaderboard.json");

    public Leaderboard Load()
    {
        if (!System.IO.File.Exists(FilePath))
            return Leaderboard.Empty;

        return JsonSerializer.Deserialize<Leaderboard>(
            System.IO.File.ReadAllText(FilePath), JsonOptions.Default)
            ?? Leaderboard.Empty;
    }

    public void Save(Leaderboard leaderboard)
    {
        Directory.CreateDirectory(options.Path);
        System.IO.File.WriteAllText(FilePath,
            JsonSerializer.Serialize(leaderboard, JsonOptions.Pretty));
    }
}
