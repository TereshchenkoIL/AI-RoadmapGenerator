using Microsoft.AspNetCore.Mvc;
using RoadmapGenerator.BL.Model;
using RoadmapGenerator.BL.Services;

namespace RoadmapGenerator.API.Endpoints;

public class GenerateRoadmapEndpoint: IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("roadmap/generate", async (
                [FromBody] GeneratePlanRequest request,
                IRoadmapGenerator generatorService) =>
            {
                var result = await generatorService.GeneratePlanAsync(request);
                return Results.Ok(new { roadmap = result.Plan });
            })
            .WithName("GenerateRoadmap")
            .WithTags("Roadmap");
    }
}