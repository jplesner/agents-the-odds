using AgentsTheOdds.Application;
using AgentsTheOdds.Cli;
using AgentsTheOdds.Data;
using AgentsTheOdds.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IDrawRepository, InMemoryDrawRepository>();
        services.AddSingleton<IAgentRepository, InMemoryAgentRepository>();
        services.AddSingleton<IPredictionRepository, InMemoryPredictionRepository>();
        services.AddSingleton<IGamePresenter, ConsoleGamePresenter>();
        services.AddTransient<GameRunner>();
    })
    .Build();

await host.Services.GetRequiredService<GameRunner>().RunAsync();
