using AgentsTheOdds.Application;
using AgentsTheOdds.Application.Commands;
using AgentsTheOdds.Application.Services;
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

        // Phase 1 (in-memory, play command only)
        services.AddSingleton<IAgentRepository, InMemoryAgentRepository>();
        services.AddSingleton<IPredictionRepository, InMemoryPredictionRepository>();
        services.AddSingleton<IGamePresenter, ConsoleGamePresenter>();
        services.AddTransient<GameRunner>();

        // Phase 2 (file-based)
        services.AddSingleton<IDrawRepository, JsonDrawRepository>();
        services.AddSingleton<IDrawService, RandomDrawService>();
        services.AddSingleton<IEpisodePredictionRepository, JsonEpisodePredictionRepository>();
        services.AddSingleton<IEpisodeResultRepository, JsonEpisodeResultRepository>();
        services.AddSingleton<ILeaderboardRepository, JsonLeaderboardRepository>();
        services.AddSingleton<IRecapWriter, MarkdownRecapWriter>();
        services.AddSingleton<IRealityCheckGenerator, RealityCheckGenerator>();

        services.AddTransient<DrawCommand>();
        services.AddTransient<PredictCommand>();
        services.AddTransient<ScoreCommand>();
    })
    .Build();

var rootCommand = new RootCommand("Agents the Odds — lottery prediction game");

// draw
var drawEpisodeOption = new Option<int>("--episode", "Episode number") { IsRequired = true };
var drawForceOption = new Option<bool>("--force", "Overwrite existing draw");
var drawCmd = new Command("draw", "Generate and record the draw result for an episode")
{
    drawEpisodeOption,
    drawForceOption,
};
drawCmd.SetHandler((episode, force) =>
{
    var cmd = host.Services.GetRequiredService<DrawCommand>();
    Environment.Exit(cmd.Execute(episode, force));
}, drawEpisodeOption, drawForceOption);

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
var scoreCmd = new Command("score", "Score predictions against the recorded draw for an episode")
{
    scoreEpisodeOption,
};
scoreCmd.SetHandler(episode =>
{
    var cmd = host.Services.GetRequiredService<ScoreCommand>();
    Environment.Exit(cmd.Execute(episode));
}, scoreEpisodeOption);

// play (Phase 1 in-memory simulation)
var playCmd = new Command("play", "Run a single in-memory game round (Phase 1)");
playCmd.SetHandler(async () =>
{
    await host.Services.GetRequiredService<GameRunner>().RunAsync();
});

rootCommand.AddCommand(drawCmd);
rootCommand.AddCommand(predictCmd);
rootCommand.AddCommand(scoreCmd);
rootCommand.AddCommand(playCmd);

return await rootCommand.InvokeAsync(args);
