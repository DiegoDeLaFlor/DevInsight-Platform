using DevInsight.EngineeringIntelligence.Domain.Entities;

namespace DevInsight.EngineeringIntelligence.Application.Contracts;

public sealed record CodeAnalysisFileResult(string FilePath, IReadOnlyCollection<CodeIssue> Issues);
