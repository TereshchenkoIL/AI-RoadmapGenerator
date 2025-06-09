using RoadmapGenerator.BL.Model;

namespace RoadmapGenerator.BL.Services;

public interface IRoadmapGenerator
{
    Task<GeneratedPlanResult> GeneratePlanAsync(GeneratePlanRequest request);
}