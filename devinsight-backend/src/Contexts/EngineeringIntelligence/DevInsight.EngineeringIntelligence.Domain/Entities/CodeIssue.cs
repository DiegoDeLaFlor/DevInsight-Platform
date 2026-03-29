namespace DevInsight.EngineeringIntelligence.Domain.Entities;

public sealed record CodeIssue(
    string Type,
    int Line,
    string Severity,
    string Message,
    string? SymbolName = null);
