using Microsoft.Extensions.Options;
using OpenAI.Chat;
using RoadmapGenerator.BL.Model;

namespace RoadmapGenerator.BL.Services;

public class ChatGpt4oMiniModelService: ILanguageModelService
{
    private OpenApiSettings _openApiSettings;

    public ChatGpt4oMiniModelService(IOptionsSnapshot<OpenApiSettings> openApiSettingsSnapshot)
    {
        _openApiSettings = openApiSettingsSnapshot.Value;
    }
    
    public async Task<GeneratedPlanResult> GenerateLearningPlanAsync(string userPrompt)
    {
        ChatClient client = new(
            model: "gpt-4o-mini", 
            apiKey: _openApiSettings.ApiKey
        );

        ChatCompletion completion = await client.CompleteChatAsync(userPrompt);

        return new GeneratedPlanResult
        {
            Plan = completion.Content[0].Text,
            InputTokens = completion.Usage.InputTokenCount,
            OutputTokens = completion.Usage.OutputTokenCount,
            TotalTokens = completion.Usage.TotalTokenCount,
        };
    }
}