using DevInsight.EngineeringIntelligence.Application.Contracts;

namespace DevInsight.EngineeringIntelligence.Application.OutboundServices;

public interface ICodeAnalyzerService
{
    Task<IReadOnlyCollection<CodeAnalysisFileResult>> AnalyzeAsync(string repositoryPath, CancellationToken cancellationToken);
}
