using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;
using System.Text.Json;

namespace AgentsTheOdds.Data.Storage;

public sealed class JsonEpisodePredictionRepository(DataRootOptions options) : IEpisodePredictionRepository
{
    private string PredictionsPath => Path.Combine(options.Path, "predictions");

    public bool Exists(int episodeNumber) =>
        System.IO.File.Exists(FilePath(episodeNumber));

    public EpisodePredictionSet Get(int episodeNumber)
    {
        var path = FilePath(episodeNumber);
        if (!System.IO.File.Exists(path))
            throw new FileNotFoundException(
                $"Predictions file for episode {episodeNumber} not found.", path);

        return JsonSerializer.Deserialize<EpisodePredictionSet>(
            System.IO.File.ReadAllText(path), JsonOptions.Default)!;
    }

    public void Save(EpisodePredictionSet set)
    {
        Directory.CreateDirectory(PredictionsPath);
        System.IO.File.WriteAllText(
            FilePath(set.EpisodeNumber),
            JsonSerializer.Serialize(set, JsonOptions.Pretty));
    }

    private string FilePath(int n) =>
        Path.Combine(PredictionsPath, $"episode-{n:D3}.json");
}
