namespace RoadmapGenerator.BL.Model;

public record GeneratePlanRequest
{
    public GeneratePlanRequest(UserInfo user, string goal, string difficultyLevel, string model)
    {
        User = user;
        Goal = goal;
        DifficultyLevel = difficultyLevel;
        Model = model;
    }

    public UserInfo User { get; set; }
    public string Goal { get; set; }
    
    public string DifficultyLevel { get; set; }
    public string Model { get; set; } = ModelConstants.ChatGPT35;
}