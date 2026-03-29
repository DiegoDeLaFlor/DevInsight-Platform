using DevInsight.EngineeringIntelligence.Domain.Aggregates;
using DevInsight.EngineeringIntelligence.Domain.Repositories;
using System.Collections.Concurrent;

namespace DevInsight.EngineeringIntelligence.Infrastructure.Persistence;

public sealed class InMemoryAnalysisRepository : IAnalysisRepository
{
    private readonly ConcurrentDictionary<Guid, List<Analysis>> _storage = new();

    public Task AddAsync(Analysis analysis, CancellationToken cancellationToken)
    {
        _storage.AddOrUpdate(
            analysis.RepositoryId,
            _ => [analysis],
            (_, existing) =>
            {
                existing.Add(analysis);
                return existing;
            });

        return Task.CompletedTask;
    }

    public Task<Analysis?> FindLatestByRepositoryIdAsync(Guid repositoryId, CancellationToken cancellationToken)
    {
        if (!_storage.TryGetValue(repositoryId, out var analyses) || analyses.Count == 0)
        {
            return Task.FromResult<Analysis?>(null);
        }

        var latest = analyses
            .OrderByDescending(x => x.CompletedAtUtc ?? x.StartedAtUtc)
            .FirstOrDefault();

        return Task.FromResult(latest);
    }
}
