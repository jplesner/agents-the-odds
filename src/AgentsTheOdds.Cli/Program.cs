using AgentsTheOdds.Application;
using AgentsTheOdds.Application.Commands;
using AgentsTheOdds.Cli;
using AgentsTheOdds.Data;
using AgentsTheOdds.Data.Storage;
using AgentsTheOdds.Domain.Interfaces;
using AgentsTheOdds.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.CommandLine;

var dataRoot = Environment.GetEnvironmentVariable("AGENTS_DATA_ROOT")
               ?? DataRootResolver.Resolve();
var dataOpts = new DataRootOptions { Path = dataRoot };

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton(dataOpts);

        // Phase 1 (in-memory)
        services.AddSingleton<IDrawRepository, InMemoryDrawRepository>();
        services.AddSingleton<IAgentRepository, InMemoryAgentRepository>();
        services.AddSingleton<IPredictionRepository, InMemoryPredictionRepository>();
        services.AddSingleton<IGamePresenter, ConsoleGamePresenter>();
        services.AddTransient<GameRunner>();

        // Phase 2 (file-based)
        services.AddSingleton<IDrawResultRepository, JsonDrawResultRepository>();
        services.AddSingleton<IEpisodePredictionRepository, JsonEpisodePredictionRepository>();
        services.AddSingleton<IEpisodeResultRepository, JsonEpisodeResultRepository>();
        services.AddSingleton<ILeaderboardRepository, JsonLeaderboardRepository>();
        services.AddSingleton<IRecapWriter, MarkdownRecapWriter>();
        services.AddSingleton<IRealityCheckGenerator, RealityCheckGenerator>();

        services.AddTransient<PredictCommand>();
        services.AddTransient<ScoreCommand>();
    })
    .Build();

var rootCommand = new RootCommand("Agents the Odds — lottery prediction game");

// predict
var predictEpisodeOption = new Option<int>("--episode", "Episode number") { IsRequired = true };
var predictForceOption = new Option<bool>("--force", "Overwrite existing predictions");
var predictCmd = new Command("predict", "Generate and lock agent predictions for an episode")
{
    predictEpisodeOption,
    predictForceOption,
};
predictCmd.SetHandler((episode, force) =>
{
    var cmd = host.Services.GetRequiredService<PredictCommand>();
    Environment.Exit(cmd.Execute(episode, force));
}, predictEpisodeOption, predictForceOption);

// score
var scoreEpisodeOption = new Option<int>("--episode", "Episode number") { IsRequired = true };
var scoreCmd = new Command("score", "Score predictions against a draw result for an episode")
{
    scoreEpisodeOption,
};
scoreCmd.SetHandler(episode =>
{
    var cmd = host.Services.GetRequiredService<ScoreCommand>();
    Environment.Exit(cmd.Execute(episode));
}, scoreEpisodeOption);

// play (Phase 1)
var playCmd = new Command("play", "Run a single in-memory game round (Phase 1)");
playCmd.SetHandler(async () =>
{
    await host.Services.GetRequiredService<GameRunner>().RunAsync();
});

rootCommand.AddCommand(predictCmd);
rootCommand.AddCommand(scoreCmd);
rootCommand.AddCommand(playCmd);

return await rootCommand.InvokeAsync(args);
