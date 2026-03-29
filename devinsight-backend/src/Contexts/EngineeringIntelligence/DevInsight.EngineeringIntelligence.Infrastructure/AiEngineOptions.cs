namespace DevInsight.EngineeringIntelligence.Infrastructure;

public sealed class AiEngineOptions
{
    public const string SectionName = "AiEngine";

    public string BaseUrl { get; init; } = "http://localhost:8000";
}
