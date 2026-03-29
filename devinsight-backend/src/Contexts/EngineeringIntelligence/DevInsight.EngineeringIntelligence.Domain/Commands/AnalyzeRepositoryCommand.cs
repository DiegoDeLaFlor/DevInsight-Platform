namespace DevInsight.EngineeringIntelligence.Domain.Commands;

public sealed record AnalyzeRepositoryCommand(
    string RepositoryName,
    string RepositoryUrl,
    string? Branch,
    string? GitHubSessionId);
