using DevInsight.EngineeringIntelligence.Domain.Aggregates;

namespace DevInsight.EngineeringIntelligence.Domain.Repositories;

public interface IAnalysisRepository
{
    Task AddAsync(Analysis analysis, CancellationToken cancellationToken);
    Task<Analysis?> FindLatestByRepositoryIdAsync(Guid repositoryId, CancellationToken cancellationToken);
}
