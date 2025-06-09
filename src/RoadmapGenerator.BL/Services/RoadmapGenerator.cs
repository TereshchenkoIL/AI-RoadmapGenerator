using System.Text.Json;
using RoadmapGenerator.BL.Model;

namespace RoadmapGenerator.BL.Services;

public class RoadmapGenerator : IRoadmapGenerator
{
    private readonly ILanguageModelFactory _factory;

    public RoadmapGenerator(ILanguageModelFactory factory)
    {
        _factory = factory;
    }

    public async Task<GeneratedPlanResult> GeneratePlanAsync(GeneratePlanRequest request)
    {
        var modelService = _factory.GetModelService(request.Model);

        var userPrompt = $@"
📌 ROLE:
Ти — експерт з розробки навчальних планів для IT-напрямків. Твоя задача — на основі інформації про користувача згенерувати послідовний навчальний план, який складається з кроків. Кожен крок має опис тем, понять, технологій або бібліотек, які потрібно вивчити.

📥 USER DATA (JSON):
{JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true })}

📐 RESPONSE FORMAT (STRICT JSON!):
{{
  ""title"": ""string"",
  ""description"": ""string"",
  ""steps"": [
    {{
      ""step_number"": number,
      ""title"": ""string"",
      ""topics"": [
        {{
          ""concept"": ""string"",
          ""subtopics"": [""string"", ...]
        }}
      ]
    }}
  ]
}}

📣 RULES:
- Вивід повинен бути тільки у форматі JSON. Без пояснень, без markdown.
- Кожен крок має фокус на темі або наборі тем, які мають логічний порядок
- Якщо немає підтем — повертай порожній масив
- Вказуй реальні поняття, бібліотеки або фреймворки
- Використовуй подвійні ASCII лапки (""), не типографічні (“ або ”)

            🎯 Ціль — створити чіткий покроковий навчальний план, який можна легко конвертувати в структуру для фронтенду або графа знань.
        ";

        return await modelService.GenerateLearningPlanAsync(userPrompt);
    }
}