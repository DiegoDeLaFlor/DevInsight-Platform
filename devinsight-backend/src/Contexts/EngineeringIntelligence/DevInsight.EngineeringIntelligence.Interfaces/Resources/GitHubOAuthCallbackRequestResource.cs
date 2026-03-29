namespace DevInsight.EngineeringIntelligence.Interfaces.Resources;

public sealed record GitHubOAuthCallbackRequestResource(string Code, string State);