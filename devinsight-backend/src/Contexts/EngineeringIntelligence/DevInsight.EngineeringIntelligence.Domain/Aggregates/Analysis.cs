using DevInsight.EngineeringIntelligence.Domain.Entities;

namespace DevInsight.EngineeringIntelligence.Domain.Aggregates;

public sealed class Analysis
{
    private readonly List<CodeIssue> _issues = new();
    private readonly List<Insight> _insights = new();

    private Analysis(Guid id, Guid repositoryId)
    {
        Id = id;
        RepositoryId = repositoryId;
        StartedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public Guid RepositoryId { get; }
    public DateTime StartedAtUtc { get; }
    public DateTime? CompletedAtUtc { get; private set; }
    public IReadOnlyCollection<CodeIssue> Issues => _issues.AsReadOnly();
    public IReadOnlyCollection<Insight> Insights => _insights.AsReadOnly();

    public static Analysis Start(Guid repositoryId) => new(Guid.NewGuid(), repositoryId);

    public void AddIssues(IEnumerable<CodeIssue> issues)
    {
        _issues.AddRange(issues);
    }

    public void AddInsights(IEnumerable<Insight> insights)
    {
        _insights.AddRange(insights);
    }

    public void MarkCompleted()
    {
        CompletedAtUtc = DateTime.UtcNow;
    }
}
