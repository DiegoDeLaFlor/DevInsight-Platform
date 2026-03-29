using DevInsight.EngineeringIntelligence.Application.Contracts;

namespace DevInsight.EngineeringIntelligence.Application.OutboundServices;

public interface IGitHubOAuthService
{
    Task<GitHubOAuthStartResult> StartAuthorizationAsync(CancellationToken cancellationToken);

    Task<GitHubOAuthCompletionResult> CompleteAuthorizationAsync(
        string code,
        string state,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GitHubRepositorySummary>> GetUserRepositoriesAsync(
        string accessToken,
        CancellationToken cancellationToken);
}