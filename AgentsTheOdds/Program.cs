using AgentsTheOdds.Data;
using AgentsTheOdds.Domain;
using AgentsTheOdds.Models;

var rules = SampleData.Rules;
var draw = SampleData.CurrentDraw;
var agents = SampleData.Agents;

var context = new PredictionContext
{
    Rules        = rules,
    DrawHistory  = SampleData.DrawHistory,
    AgentHistory = [],
    Leaderboard  = Leaderboard.Empty,
};

Console.WriteLine("╔══════════════════════════════════════╗");
Console.WriteLine("║          Agents the Odds             ║");
Console.WriteLine("╚══════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine($"Draw #{draw.DrawNumber} · {draw.Date}");
Console.WriteLine($"Numbers: {string.Join("  ", draw.Numbers)}");
Console.WriteLine();

var agentById = agents.ToDictionary(a => a.Id);
var results = new List<PredictionResult>();

foreach (var agent in agents)
{
    var prediction = agent.Strategy.GeneratePrediction(context);
    var validation = LotteryValidator.Validate(prediction, rules);

    if (!validation.IsValid)
    {
        Console.WriteLine($"[INVALID] {agent.Name}: {validation.Error}");
        continue;
    }

    results.Add(Scorer.Score(prediction, draw));
}

var ranked = results
    .OrderByDescending(r => r.Points)
    .ThenByDescending(r => r.Prediction.Confidence)
    .ToList();

Console.WriteLine("── Predictions ─────────────────────────────────────────────");
Console.WriteLine();
foreach (var result in ranked)
{
    var agent = agentById[result.Prediction.AgentId];
    Console.WriteLine($"  {agent.Name}  ({result.Prediction.StrategyName})");
    Console.WriteLine($"  Picked:     {string.Join("  ", result.Prediction.Numbers)}");
    Console.WriteLine($"  Confidence: {result.Prediction.Confidence:F2}");
    Console.WriteLine($"  Reasoning:  \"{result.Prediction.Reasoning}\"");
    Console.WriteLine($"  Result:     {result.Matches} matches → {result.Points} pts");
    Console.WriteLine();
}

Console.WriteLine("── Leaderboard ─────────────────────────────────────────────");
Console.WriteLine();
Console.WriteLine($"  {"#",-3} {"Agent",-22} {"Pts",5}   {"Matches",7}   {"Confidence",10}");
Console.WriteLine($"  {new string('-', 55)}");
for (var i = 0; i < ranked.Count; i++)
{
    var r = ranked[i];
    var name = agentById[r.Prediction.AgentId].Name;
    Console.WriteLine($"  {i + 1,-3} {name,-22} {r.Points,5}   {r.Matches,7}   {r.Prediction.Confidence,10:F2}");
}

Console.WriteLine();
