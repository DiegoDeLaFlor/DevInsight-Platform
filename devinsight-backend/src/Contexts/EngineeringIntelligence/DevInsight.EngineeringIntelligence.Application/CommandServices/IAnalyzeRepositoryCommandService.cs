using DevInsight.EngineeringIntelligence.Application.Contracts;
using DevInsight.EngineeringIntelligence.Domain.Commands;

namespace DevInsight.EngineeringIntelligence.Application.CommandServices;

public interface IAnalyzeRepositoryCommandService
{
    Task<AnalyzeRepositoryResult> HandleAsync(AnalyzeRepositoryCommand command, CancellationToken cancellationToken);
}
