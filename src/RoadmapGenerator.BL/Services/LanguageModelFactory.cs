using Microsoft.Extensions.DependencyInjection;

namespace RoadmapGenerator.BL.Services;

public class LanguageModelFactory : ILanguageModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public LanguageModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public ILanguageModelService GetModelService(string modelName)
    {
        return modelName.ToLower() switch
        {
            ModelConstants.ChatGPT35 => _serviceProvider.GetRequiredService<ChatGpt3ModelService>(),
            ModelConstants.ChatGPT4o => _serviceProvider.GetRequiredService<ChatGpt4oModelService>(),
            ModelConstants.ChatGPT4oMini => _serviceProvider.GetRequiredService<ChatGpt4oMiniModelService>(),
            ModelConstants.ChatGPTo3Mini => _serviceProvider.GetRequiredService<ChatGpto3MiniModelService>(),
            // "openai" => _serviceProvider.GetRequiredService<OpenAIService>(),
            // "mistral" => _serviceProvider.GetRequiredService<MistralService>(),
            _ => throw new ArgumentException($"Model '{modelName}' is not supported.")
        };
    }
}