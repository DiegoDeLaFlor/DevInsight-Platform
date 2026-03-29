using DevInsight.EngineeringIntelligence.Domain.Aggregates;
using DevInsight.EngineeringIntelligence.Interfaces.Resources;

namespace DevInsight.EngineeringIntelligence.Interfaces.Assemblers;

public static class AnalysisResourceAssembler
{
    public static AnalysisResource ToResource(Analysis analysis)
    {
        var issues = analysis.Issues
            .Select(issue => new CodeIssueResource(issue.Type, issue.Line, issue.Severity, issue.Message, issue.SymbolName))
            .ToArray();

        var insights = analysis.Insights
            .Select(insight => new InsightResource(insight.Title, insight.Explanation, insight.SuggestedImprovement, insight.RiskLevel))
            .ToArray();

        return new AnalysisResource(
            analysis.Id,
            analysis.RepositoryId,
            analysis.StartedAtUtc,
            analysis.CompletedAtUtc,
            issues,
            insights);
    }
}
