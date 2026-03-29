using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using DevInsight.EngineeringIntelligence.Application.Contracts;
using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevInsight.EngineeringIntelligence.Infrastructure.Services;

public sealed class GitHubOAuthService : IGitHubOAuthService
{
    private static readonly ConcurrentDictionary<string, DateTime> PendingStates = new(StringComparer.Ordinal);
    private static readonly TimeSpan StateTtl = TimeSpan.FromMinutes(10);

    private readonly HttpClient _httpClient;
    private readonly GitHubOptions _options;
    private readonly ILogger<GitHubOAuthService> _logger;

    public GitHubOAuthService(
        HttpClient httpClient,
        IOptions<GitHubOptions> options,
        ILogger<GitHubOAuthService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task<GitHubOAuthStartResult> StartAuthorizationAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureConfigured();

        var state = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        PendingStates[state] = DateTime.UtcNow.Add(StateTtl);
        TrimExpiredStates();

        var authorizationUrl =
            "https://github.com/login/oauth/authorize" +
            $"?client_id={Uri.EscapeDataString(_options.ClientId)}" +
            $"&redirect_uri={Uri.EscapeDataString(_options.RedirectUri)}" +
            $"&scope={Uri.EscapeDataString("repo read:user")}" +
            $"&state={Uri.EscapeDataString(state)}";

        return Task.FromResult(new GitHubOAuthStartResult(authorizationUrl, state));
    }

    public async Task<GitHubOAuthCompletionResult> CompleteAuthorizationAsync(string code, string state, CancellationToken cancellationToken)
    {
        EnsureConfigured();

        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
        {
            throw new InvalidOperationException("OAuth callback payload is incomplete.");
        }

        ValidateAndConsumeState(state);

        using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "https://github.com/login/oauth/access_token")
        {
            Content = JsonContent.Create(new
            {
                client_id = _options.ClientId,
                client_secret = _options.ClientSecret,
                code,
                redirect_uri = _options.RedirectUri,
                state
            })
        };
        tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        tokenRequest.Headers.UserAgent.Add(new ProductInfoHeaderValue("DevInsight", "1.0"));

        using var tokenResponse = await _httpClient.SendAsync(tokenRequest, cancellationToken);
        if (!tokenResponse.IsSuccessStatusCode)
        {
            _logger.LogWarning("GitHub token exchange failed with status {StatusCode}.", tokenResponse.StatusCode);
            throw new InvalidOperationException("GitHub token exchange failed.");
        }

        var tokenPayload = await tokenResponse.Content.ReadFromJsonAsync<GitHubAccessTokenResponse>(cancellationToken);
        if (tokenPayload is null || string.IsNullOrWhiteSpace(tokenPayload.AccessToken))
        {
            throw new InvalidOperationException("GitHub token response is invalid.");
        }

        var userLogin = await FetchUserLoginAsync(tokenPayload.AccessToken, cancellationToken);
        return new GitHubOAuthCompletionResult(tokenPayload.AccessToken, userLogin);
    }

    public async Task<IReadOnlyCollection<GitHubRepositorySummary>> GetUserRepositoriesAsync(string accessToken, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new InvalidOperationException("Access token is required.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/repos?per_page=100&sort=updated");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("DevInsight", "1.0"));

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GitHub repositories endpoint failed with status {StatusCode}.", response.StatusCode);
            throw new InvalidOperationException("Failed to fetch repositories from GitHub.");
        }

        var repositories = await response.Content.ReadFromJsonAsync<IReadOnlyCollection<GitHubRepositoryDto>>(cancellationToken);
        return repositories?
            .Select(x => new GitHubRepositorySummary(
                x.Id,
                x.Name ?? string.Empty,
                x.FullName ?? string.Empty,
                x.DefaultBranch ?? "main",
                x.Private,
                x.HtmlUrl ?? string.Empty))
            .ToArray() ?? Array.Empty<GitHubRepositorySummary>();
    }

    private async Task<string> FetchUserLoginAsync(string accessToken, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("DevInsight", "1.0"));

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("GitHub user endpoint failed with status {StatusCode}.", response.StatusCode);
            throw new InvalidOperationException("Failed to fetch authenticated GitHub user.");
        }

        var user = await response.Content.ReadFromJsonAsync<GitHubUserDto>(cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.Login))
        {
            throw new InvalidOperationException("GitHub user payload is invalid.");
        }

        return user.Login;
    }

    private void EnsureConfigured()
    {
        if (string.IsNullOrWhiteSpace(_options.ClientId) ||
            string.IsNullOrWhiteSpace(_options.ClientSecret) ||
            string.IsNullOrWhiteSpace(_options.RedirectUri))
        {
            throw new InvalidOperationException("GitHub OAuth is not configured. Set GitHub ClientId, ClientSecret and RedirectUri.");
        }
    }

    private static void ValidateAndConsumeState(string state)
    {
        if (!PendingStates.TryRemove(state, out var expiresAtUtc))
        {
            throw new InvalidOperationException("OAuth state is invalid or already used.");
        }

        if (expiresAtUtc <= DateTime.UtcNow)
        {
            throw new InvalidOperationException("OAuth state has expired.");
        }
    }

    private static void TrimExpiredStates()
    {
        var now = DateTime.UtcNow;
        foreach (var item in PendingStates.Where(x => x.Value <= now).ToArray())
        {
            PendingStates.TryRemove(item.Key, out _);
        }
    }

    private sealed class GitHubAccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; } = string.Empty;

        [JsonPropertyName("scope")]
        public string Scope { get; init; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; } = string.Empty;
    }

    private sealed class GitHubUserDto
    {
        [JsonPropertyName("login")]
        public string Login { get; init; } = string.Empty;
    }

    private sealed class GitHubRepositoryDto
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("full_name")]
        public string? FullName { get; init; }

        [JsonPropertyName("default_branch")]
        public string? DefaultBranch { get; init; }

        [JsonPropertyName("private")]
        public bool Private { get; init; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; init; }
    }
}