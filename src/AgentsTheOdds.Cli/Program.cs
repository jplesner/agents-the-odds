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
using System.Text.Json;

var dataRoot = Environment.GetEnvironmentVariable("AGENTS_DATA_ROOT")
               ?? DataRootResolver.Resolve();
var dataOpts = new DataRootOptions { Path = dataRoot };

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton(dataOpts);

        services.AddSingleton<IAgentRepository, InMemoryAgentRepository>();
        services.AddSingleton<IPredictionRepository, InMemoryPredictionRepository>();

        // File-based
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
        services.AddTransient<ShowCommand>(sp => new ShowCommand(
            sp.GetRequiredService<IEpisodeResultRepository>(),
            new ConsoleGamePresenter().ShowEpisode));
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

// show
var showEpisodeOption = new Option<int>("--episode", "Episode number") { IsRequired = true };
var showCmd = new Command("show", "Display the results of a scored episode")
{
    showEpisodeOption,
};
showCmd.SetHandler(episode =>
{
    var cmd = host.Services.GetRequiredService<ShowCommand>();
    Environment.Exit(cmd.Execute(episode));
}, showEpisodeOption);

// agents — outputs agent list as JSON for the think script
var agentsCmd = new Command("agents", "List all agents as JSON");
agentsCmd.SetHandler(() =>
{
    var repo = host.Services.GetRequiredService<IAgentRepository>();
    var output = repo.GetAll().Select(a => new
    {
        id            = a.Id,
        name          = a.Name,
        strategyClass = a.Strategy.GetType().Name,
    });
    Console.WriteLine(JsonSerializer.Serialize(output,
        new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
});

rootCommand.AddCommand(drawCmd);
rootCommand.AddCommand(predictCmd);
rootCommand.AddCommand(scoreCmd);
rootCommand.AddCommand(showCmd);
rootCommand.AddCommand(agentsCmd);

return await rootCommand.InvokeAsync(args);
