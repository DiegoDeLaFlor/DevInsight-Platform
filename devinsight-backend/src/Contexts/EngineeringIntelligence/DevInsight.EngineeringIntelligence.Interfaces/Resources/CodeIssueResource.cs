namespace DevInsight.EngineeringIntelligence.Interfaces.Resources;

public sealed record CodeIssueResource(string Type, int Line, string Severity, string Message, string? SymbolName);
