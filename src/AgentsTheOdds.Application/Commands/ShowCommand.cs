using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Models;

namespace AgentsTheOdds.Application.Commands;

public sealed class ShowCommand(IEpisodeResultRepository episodeResults, Action<EpisodeResult> present)
{
    public int Execute(int episodeNumber)
    {
        var result = episodeResults.TryGet(episodeNumber);
        if (result is null)
        {
            Console.Error.WriteLine(
                $"No results found for episode {episodeNumber}. " +
                $"Run `score --episode {episodeNumber}` first.");
            return 1;
        }

        present(result);
        return 0;
    }
}
