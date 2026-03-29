namespace DevInsight.EngineeringIntelligence.Interfaces.Resources;

public sealed record GitHubSessionResource(string SessionId, DateTime ExpiresAtUtc, string UserLogin);