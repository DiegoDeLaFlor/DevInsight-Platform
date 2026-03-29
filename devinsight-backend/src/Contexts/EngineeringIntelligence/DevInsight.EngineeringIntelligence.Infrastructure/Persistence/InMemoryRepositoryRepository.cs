using DevInsight.EngineeringIntelligence.Domain.Aggregates;
using DevInsight.EngineeringIntelligence.Domain.Repositories;
using System.Collections.Concurrent;

namespace DevInsight.EngineeringIntelligence.Infrastructure.Persistence;

public sealed class InMemoryRepositoryRepository : IRepositoryRepository
{
    private readonly ConcurrentDictionary<Guid, Repository> _storage = new();

    public Task AddAsync(Repository repository, CancellationToken cancellationToken)
    {
        _storage[repository.Id] = repository;
        return Task.CompletedTask;
    }

    public Task<Repository?> FindByIdAsync(Guid repositoryId, CancellationToken cancellationToken)
    {
        _storage.TryGetValue(repositoryId, out var repository);
        return Task.FromResult(repository);
    }
}
