using DevInsight.EngineeringIntelligence.Application.Contracts;
using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using DevInsight.EngineeringIntelligence.Application.Pipelines;
using DevInsight.EngineeringIntelligence.Domain.Aggregates;
using DevInsight.EngineeringIntelligence.Domain.Commands;
using DevInsight.EngineeringIntelligence.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace DevInsight.EngineeringIntelligence.Application.CommandServices;

public sealed class AnalyzeRepositoryCommandService : IAnalyzeRepositoryCommandService
{
    private readonly IRepositoryRepository _repositoryRepository;
    private readonly IAnalysisPipeline _analysisPipeline;
    private readonly IGitHubSessionService _gitHubSessionService;
    private readonly ILogger<AnalyzeRepositoryCommandService> _logger;

    public AnalyzeRepositoryCommandService(
        IRepositoryRepository repositoryRepository,
        IAnalysisPipeline analysisPipeline,
        IGitHubSessionService gitHubSessionService,
        ILogger<AnalyzeRepositoryCommandService> logger)
    {
        _repositoryRepository = repositoryRepository;
        _analysisPipeline = analysisPipeline;
        _gitHubSessionService = gitHubSessionService;
        _logger = logger;
    }

    public async Task<AnalyzeRepositoryResult> HandleAsync(AnalyzeRepositoryCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var repository = Repository.Create(command.RepositoryName, command.RepositoryUrl, command.Branch);
            await _repositoryRepository.AddAsync(repository, cancellationToken);

            string? accessToken = null;
            if (!string.IsNullOrWhiteSpace(command.GitHubSessionId))
            {
                accessToken = await _gitHubSessionService.GetAccessTokenAsync(command.GitHubSessionId, cancellationToken);
            }

            var analysis = await _analysisPipeline.ExecuteAsync(repository, accessToken, cancellationToken);

            _logger.LogInformation(
                "Repository {RepositoryId} analyzed successfully with analysis {AnalysisId}.",
                repository.Id,
                analysis.Id);

            return new AnalyzeRepositoryResult(repository.Id, analysis.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to analyze repository {RepositoryUrl}.", command.RepositoryUrl);
            throw;
        }
    }
}
