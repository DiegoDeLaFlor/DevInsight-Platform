using DevInsight.EngineeringIntelligence.Domain.Aggregates;

namespace DevInsight.EngineeringIntelligence.Domain.Repositories;

public interface IRepositoryRepository
{
    Task AddAsync(Repository repository, CancellationToken cancellationToken);
    Task<Repository?> FindByIdAsync(Guid repositoryId, CancellationToken cancellationToken);
}
