namespace DevInsight.EngineeringIntelligence.Interfaces.Resources;

public sealed record GitHubRepositoryResource(
    long Id,
    string Name,
    string FullName,
    string DefaultBranch,
    bool IsPrivate,
    string HtmlUrl);