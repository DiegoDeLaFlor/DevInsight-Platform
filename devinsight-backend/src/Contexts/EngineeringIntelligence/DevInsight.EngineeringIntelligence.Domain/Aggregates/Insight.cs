namespace DevInsight.EngineeringIntelligence.Domain.Aggregates;

public sealed class Insight
{
    public Insight(Guid id, string title, string explanation, string suggestedImprovement, string riskLevel)
    {
        Id = id;
        Title = title;
        Explanation = explanation;
        SuggestedImprovement = suggestedImprovement;
        RiskLevel = riskLevel;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Explanation { get; }
    public string SuggestedImprovement { get; }
    public string RiskLevel { get; }
}
