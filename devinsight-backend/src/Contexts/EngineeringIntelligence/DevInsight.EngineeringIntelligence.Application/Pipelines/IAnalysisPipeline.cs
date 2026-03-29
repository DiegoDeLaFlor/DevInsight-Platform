using DevInsight.EngineeringIntelligence.Domain.Aggregates;

namespace DevInsight.EngineeringIntelligence.Application.Pipelines;

public interface IAnalysisPipeline
{
    Task<Analysis> ExecuteAsync(Repository repository, CancellationToken cancellationToken);
}
