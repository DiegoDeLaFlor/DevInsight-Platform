using DevInsight.EngineeringIntelligence.Domain.Aggregates;
using DevInsight.EngineeringIntelligence.Domain.Queries;

namespace DevInsight.EngineeringIntelligence.Application.QueryServices;

public interface IAnalysisQueryService
{
    Task<Analysis?> HandleAsync(GetAnalysisByRepositoryIdQuery query, CancellationToken cancellationToken);
}
