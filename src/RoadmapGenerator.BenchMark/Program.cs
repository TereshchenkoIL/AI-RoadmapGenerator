// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RoadmapGenerator.BL;
using RoadmapGenerator.BL.Model;
using RoadmapGenerator.BL.Services;

var services = new ServiceCollection();

services.Configure<OpenApiSettings>(opts =>
{
    opts.ApiKey = ""; // Встав свій API ключ або замокай
});

services.AddScoped<ChatGpt3ModelService>();
services.AddScoped<ChatGpt4oModelService>();
services.AddScoped<ChatGpt4oMiniModelService>();
services.AddScoped<ChatGpto3MiniModelService>();
services.AddScoped<ILanguageModelService, ChatGpt3ModelService>();
services.AddScoped<ILanguageModelFactory, LanguageModelFactory>();
services.AddScoped<IRoadmapGenerator, RoadmapGenerator.BL.Services.RoadmapGenerator>();

var provider = services.BuildServiceProvider();
var generator = provider.GetRequiredService<IRoadmapGenerator>();
var results = new List<(string Model, long TimeMs, int InputTokens, int OutputTokens, int TotalTokens, int StepCount, int Score, int Size, double Cost)>();


var models = new[] { "gpt-3.5-turbo", "gpt-4o", "gpt-4o-mini", "o3-mini" };

var requestTemplate = new GeneratePlanRequest(
    new UserInfo
    {
        Name = "Ілля",
        Age = 22,
        Background = new UserBackground
        {
            Education = "Master's in Software Engineering",
            Experience = new List<string> { "C#", "SQL", "Git" },
            Certifications = new List<string> { "Azure Fundamentals" }
        }
    },
    goal: "Become a .NET backend developer",
    difficultyLevel: "Intermediate",
    model: ""
);

foreach (var model in models)
{
    double totalTime = 0, totalIn = 0, totalOut = 0, total = 0, totalSteps = 0, totalScore = 0, totalSize = 0, totalCost = 0;

    for (int i = 1; i <= 6; i++)
    {
        var request = requestTemplate with { Model = model };

        Console.WriteLine($"🧠 Testing model: {model} | Iteration {i}");

        var sw = Stopwatch.StartNew();
        var generatedPlanResult = await generator.GeneratePlanAsync(request);
        sw.Stop();
        var timeMs = sw.ElapsedMilliseconds;
        var size = generatedPlanResult.Plan.Length;

        int stepCount = 0;
        int score = 0;
        int inputTokens = 0;
        int outputTokens = 0;
        int totalTokens = 0;
        double cost = 0.0;

        try
        {
            var doc = JsonDocument.Parse(generatedPlanResult.Plan);
            if (doc.RootElement.TryGetProperty("steps", out var steps))
            {
                stepCount = steps.GetArrayLength();
                score += 2;
            }
            if (doc.RootElement.TryGetProperty("title", out _)) score++;
            if (doc.RootElement.TryGetProperty("description", out _)) score++;
            if (stepCount > 0) score++;
        }
        catch
        {
            Console.WriteLine("⚠️ JSON parse error");
        }

        totalTokens = generatedPlanResult.TotalTokens;
        inputTokens = generatedPlanResult.InputTokens;
        outputTokens = generatedPlanResult.OutputTokens;

        var pricePer1MInput = model switch
        {
            "gpt-3.5-turbo" => 0.50,
            "gpt-4o" => 2.50,
            "gpt-4o-mini" => 0.15,
            "o3-mini" => 1.10,
            _ => 1.00
        } / 1_000_000.0;

        var pricePer1MOutput = model switch
        {
            "gpt-3.5-turbo" => 1.50,
            "gpt-4o" => 10.00,
            "gpt-4o-mini" => 0.60,
            "o3-mini" => 4.40,
            _ => 2.00
        } / 1_000_000.0;


        cost = (inputTokens * pricePer1MInput) + (outputTokens * pricePer1MOutput);

        totalTime += timeMs;
        totalIn += inputTokens;
        totalOut += outputTokens;
        total += totalTokens;
        totalSteps += stepCount;
        totalScore += score;
        totalSize += size;
        totalCost += cost;

        File.WriteAllText($"result_{model}_{i}.json", generatedPlanResult.Plan);
        Console.WriteLine($"✅ Done in {timeMs} ms, Tokens: {totalTokens} (in: {inputTokens}, out: {outputTokens}), Steps: {stepCount}, Score: {score}, Size: {size}, Cost: ${cost:F4}\n");
    }

    results.Add((model,
        (long)totalTime / 6,
        (int)Math.Truncate(totalIn / 6),
        (int)Math.Truncate(totalOut / 6),
        (int)Math.Truncate(total / 6),
        (int)Math.Truncate(totalSteps / 6),
        (int)Math.Truncate(totalScore / 6),
        (int)Math.Truncate(totalSize / 6),
        (int)Math.Truncate(totalCost / 6)));
}

Console.WriteLine("📊 Average Summary (6 runs per model):\n");
Console.WriteLine("Model             | Time (ms) | InTok | OutTok | Total | Steps | Score | Size | Cost (USD)");
Console.WriteLine("------------------|-----------|--------|--------|-------|-------|-------|------|------------");
foreach (var r in results)
{
    Console.WriteLine($"{r.Model.PadRight(18)} | {r.TimeMs,9:F0} | {r.InputTokens,6:F0} | {r.OutputTokens,6:F0} | {r.TotalTokens,5:F0} | {r.StepCount,5:F1} | {r.Score,5:F1} | {r.Size,4:F0} | {r.Cost,10:F4}");
}  

var resultsJson = JsonSerializer.Serialize(results, new JsonSerializerOptions { WriteIndented = true });
File.WriteAllText("benchmark_summary.json", resultsJson);
Console.WriteLine("\n✅ Benchmarking completed.");
