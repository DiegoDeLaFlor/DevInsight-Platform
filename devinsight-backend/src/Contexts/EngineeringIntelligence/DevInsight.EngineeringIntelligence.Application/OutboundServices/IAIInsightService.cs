using DevInsight.EngineeringIntelligence.Application.Contracts;

namespace DevInsight.EngineeringIntelligence.Application.OutboundServices;

public interface IAIInsightService
{
    Task<IReadOnlyCollection<GeneratedInsight>> GenerateInsightsAsync(
        Guid repositoryId,
        string repositoryUrl,
        IReadOnlyCollection<CodeAnalysisFileResult> fileResults,
        CancellationToken cancellationToken);
}
