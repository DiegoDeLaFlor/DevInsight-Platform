using DevInsight.EngineeringIntelligence.Domain.Aggregates;
using DevInsight.EngineeringIntelligence.Domain.Queries;
using DevInsight.EngineeringIntelligence.Domain.Repositories;

namespace DevInsight.EngineeringIntelligence.Application.QueryServices;

public sealed class AnalysisQueryService : IAnalysisQueryService
{
    private readonly IAnalysisRepository _analysisRepository;

    public AnalysisQueryService(IAnalysisRepository analysisRepository)
    {
        _analysisRepository = analysisRepository;
    }

    public Task<Analysis?> HandleAsync(GetAnalysisByRepositoryIdQuery query, CancellationToken cancellationToken)
    {
        return _analysisRepository.FindLatestByRepositoryIdAsync(query.RepositoryId, cancellationToken);
    }
}
