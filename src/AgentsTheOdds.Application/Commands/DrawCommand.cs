using AgentsTheOdds.Domain.Interfaces;

namespace AgentsTheOdds.Application.Commands;

public sealed class DrawCommand(IDrawService drawService, IDrawRepository draws)
{
    public int Execute(int episodeNumber, bool force)
    {
        try
        {
            draws.GetByEpisode(episodeNumber);
            if (!force)
            {
                Console.Error.WriteLine(
                    $"Draw for episode {episodeNumber} already exists. " +
                    "Use --force to overwrite.");
                return 1;
            }
        }
        catch (InvalidOperationException) { }

        var draw = drawService.Draw(episodeNumber);
        Console.WriteLine(
            $"Episode {episodeNumber} draw: {string.Join(", ", draw.Numbers)}");
        return 0;
    }
}
