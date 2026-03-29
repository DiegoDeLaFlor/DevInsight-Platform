using System.Collections.Concurrent;
using DevInsight.EngineeringIntelligence.Application.Contracts;
using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using Microsoft.Extensions.Options;

namespace DevInsight.EngineeringIntelligence.Infrastructure.Services;

public sealed class InMemoryGitHubSessionService : IGitHubSessionService
{
    private readonly ConcurrentDictionary<string, GitHubSessionEntry> _sessions = new(StringComparer.Ordinal);
    private readonly GitHubOptions _options;

    public InMemoryGitHubSessionService(IOptions<GitHubOptions> options)
    {
        _options = options.Value;
    }

    public Task<GitHubSession> CreateSessionAsync(string accessToken, string userLogin, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sessionId = Guid.NewGuid().ToString("N");
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(Math.Max(15, _options.SessionTtlMinutes));

        _sessions[sessionId] = new GitHubSessionEntry(accessToken, userLogin, expiresAtUtc);

        return Task.FromResult(new GitHubSession(sessionId, expiresAtUtc, userLogin));
    }

    public Task<string?> GetAccessTokenAsync(string sessionId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Task.FromResult<string?>(null);
        }

        if (!_sessions.TryGetValue(sessionId, out var entry))
        {
            return Task.FromResult<string?>(null);
        }

        if (entry.ExpiresAtUtc <= DateTime.UtcNow)
        {
            _sessions.TryRemove(sessionId, out _);
            return Task.FromResult<string?>(null);
        }

        return Task.FromResult<string?>(entry.AccessToken);
    }

    private sealed record GitHubSessionEntry(string AccessToken, string UserLogin, DateTime ExpiresAtUtc);
}