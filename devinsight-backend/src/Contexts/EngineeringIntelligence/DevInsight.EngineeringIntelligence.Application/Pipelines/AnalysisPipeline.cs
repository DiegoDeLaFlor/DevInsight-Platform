using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using DevInsight.EngineeringIntelligence.Domain.Aggregates;
using DevInsight.EngineeringIntelligence.Domain.Repositories;

namespace DevInsight.EngineeringIntelligence.Application.Pipelines;

public sealed class AnalysisPipeline : IAnalysisPipeline
{
    private readonly IGitHubService _gitHubService;
    private readonly ICodeAnalyzerService _codeAnalyzerService;
    private readonly IAIInsightService _aiInsightService;
    private readonly IAnalysisRepository _analysisRepository;

    public AnalysisPipeline(
        IGitHubService gitHubService,
        ICodeAnalyzerService codeAnalyzerService,
        IAIInsightService aiInsightService,
        IAnalysisRepository analysisRepository)
    {
        _gitHubService = gitHubService;
        _codeAnalyzerService = codeAnalyzerService;
        _aiInsightService = aiInsightService;
        _analysisRepository = analysisRepository;
    }

    public async Task<Analysis> ExecuteAsync(Repository repository, CancellationToken cancellationToken)
    {
        var analysis = Analysis.Start(repository.Id);
        var clonedPath = await _gitHubService.CloneRepositoryAsync(repository.SourceUrl, repository.Branch, cancellationToken);
        var fileResults = await _codeAnalyzerService.AnalyzeAsync(clonedPath, cancellationToken);

        var allIssues = fileResults
            .SelectMany(result => result.Issues)
            .ToArray();

        analysis.AddIssues(allIssues);

        var generatedInsights = await _aiInsightService.GenerateInsightsAsync(
            repository.Id,
            repository.SourceUrl,
            fileResults,
            cancellationToken);
        analysis.AddInsights(generatedInsights.Select(insight =>
            new Insight(Guid.NewGuid(), insight.Title, insight.Explanation, insight.SuggestedImprovement, insight.RiskLevel)));

        analysis.MarkCompleted();
        await _analysisRepository.AddAsync(analysis, cancellationToken);

        return analysis;
    }
}
