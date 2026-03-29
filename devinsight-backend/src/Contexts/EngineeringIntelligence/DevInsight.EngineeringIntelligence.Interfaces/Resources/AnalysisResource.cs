namespace DevInsight.EngineeringIntelligence.Interfaces.Resources;

public sealed record AnalysisResource(
    Guid AnalysisId,
    Guid RepositoryId,
    DateTime StartedAtUtc,
    DateTime? CompletedAtUtc,
    IReadOnlyCollection<CodeIssueResource> Issues,
    IReadOnlyCollection<InsightResource> Insights);
