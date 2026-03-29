namespace DevInsight.EngineeringIntelligence.Application.Contracts;

public sealed record GitHubOAuthStartResult(string AuthorizationUrl, string State);

public sealed record GitHubOAuthCompletionResult(string AccessToken, string UserLogin);

public sealed record GitHubSession(string SessionId, DateTime ExpiresAtUtc, string UserLogin);

public sealed record GitHubRepositorySummary(
    long Id,
    string Name,
    string FullName,
    string DefaultBranch,
    bool IsPrivate,
    string HtmlUrl);