using RoadmapGenerator.BL.Model;

namespace RoadmapGenerator.BL.Services;

public interface ILanguageModelService
{
    Task<GeneratedPlanResult> GenerateLearningPlanAsync(string userPrompt);
}