using System.Net.Http.Json;
using DevInsight.EngineeringIntelligence.Application.Contracts;
using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevInsight.EngineeringIntelligence.Infrastructure.Services;

public sealed class AiEngineInsightService : IAIInsightService
{
    private readonly HttpClient _httpClient;
    private readonly AiEngineOptions _options;
    private readonly ILogger<AiEngineInsightService> _logger;

    public AiEngineInsightService(
        HttpClient httpClient,
        IOptions<AiEngineOptions> options,
        ILogger<AiEngineInsightService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<GeneratedInsight>> GenerateInsightsAsync(
        Guid repositoryId,
        string repositoryUrl,
        IReadOnlyCollection<CodeAnalysisFileResult> fileResults,
        CancellationToken cancellationToken)
    {
        var issues = fileResults.SelectMany(x => x.Issues).ToArray();
        if (issues.Length == 0)
        {
            return [
                new GeneratedInsight(
                    "No major issues detected",
                    "No threshold violations were found for the current analysis.",
                    "Keep periodic analysis enabled and monitor trends for regressions.",
                    "low")
            ];
        }

        var request = new AiEngineInsightRequest(
            repositoryId.ToString(),
            repositoryUrl,
            fileResults.Select(file =>
                new AiEngineFileResult(
                    file.FilePath,
                    file.Issues.Select(issue =>
                        new AiEngineIssue(
                            issue.Type,
                            issue.Line,
                            issue.Severity,
                            issue.Message,
                            issue.SymbolName)).ToArray()))
                .ToArray());

        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/insights", request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("AI Engine returned non-success status code {StatusCode}. Falling back to local insights.", response.StatusCode);
            return BuildFallbackInsights(issues);
        }

        var result = await response.Content.ReadFromJsonAsync<AiEngineInsightResponse>(cancellationToken);
        if (result?.Insights is null || result.Insights.Count == 0)
        {
            return BuildFallbackInsights(issues);
        }

        return result.Insights
            .Select(x => new GeneratedInsight(x.Title, x.Explanation, x.SuggestedImprovement, x.RiskLevel))
            .ToArray();
    }

    private static IReadOnlyCollection<GeneratedInsight> BuildFallbackInsights(IReadOnlyCollection<DevInsight.EngineeringIntelligence.Domain.Entities.CodeIssue> issues)
    {
        return issues
            .GroupBy(issue => issue.Type)
            .Select(group => new GeneratedInsight(
                $"{group.Key} detected",
                $"Detected {group.Count()} issue(s) of type {group.Key}.",
                "Apply targeted refactoring and enforce static quality checks in CI.",
                group.Key == "LargeFile" ? "high" : "medium"))
            .ToArray();
    }

    private sealed record AiEngineInsightRequest(string RepositoryId, string RepositoryUrl, IReadOnlyCollection<AiEngineFileResult> Files);
    private sealed record AiEngineFileResult(string FilePath, IReadOnlyCollection<AiEngineIssue> Issues);
    private sealed record AiEngineIssue(string Type, int Line, string Severity, string Message, string? SymbolName);
    private sealed record AiEngineInsightResponse(string RepositoryId, IReadOnlyCollection<AiEngineInsightItem> Insights);
    private sealed record AiEngineInsightItem(string Title, string Explanation, string SuggestedImprovement, string RiskLevel);
}
