namespace RoadmapGenerator.BL.Services;

public interface ILanguageModelFactory
{
    ILanguageModelService GetModelService(string modelName);
}