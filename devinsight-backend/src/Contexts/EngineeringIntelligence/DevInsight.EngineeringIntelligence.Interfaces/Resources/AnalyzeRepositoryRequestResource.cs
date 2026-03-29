namespace DevInsight.EngineeringIntelligence.Interfaces.Resources;

public sealed record AnalyzeRepositoryRequestResource(
    string RepositoryName,
    string RepositoryUrl,
    string? Branch,
    string? GitHubSessionId);
