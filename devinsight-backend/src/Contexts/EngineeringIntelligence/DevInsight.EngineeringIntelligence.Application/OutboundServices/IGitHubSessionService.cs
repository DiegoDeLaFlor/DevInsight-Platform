using DevInsight.EngineeringIntelligence.Application.Contracts;

namespace DevInsight.EngineeringIntelligence.Application.OutboundServices;

public interface IGitHubSessionService
{
    Task<GitHubSession> CreateSessionAsync(string accessToken, string userLogin, CancellationToken cancellationToken);

    Task<string?> GetAccessTokenAsync(string sessionId, CancellationToken cancellationToken);
}