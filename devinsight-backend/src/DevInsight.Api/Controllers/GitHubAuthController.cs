using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using DevInsight.EngineeringIntelligence.Interfaces.Resources;
using Microsoft.AspNetCore.Mvc;

namespace DevInsight.Api.Controllers;

[ApiController]
[Route("api/auth/github")]
public sealed class GitHubAuthController : ControllerBase
{
    private readonly IGitHubOAuthService _gitHubOAuthService;
    private readonly IGitHubSessionService _gitHubSessionService;
    private readonly ILogger<GitHubAuthController> _logger;

    public GitHubAuthController(
        IGitHubOAuthService gitHubOAuthService,
        IGitHubSessionService gitHubSessionService,
        ILogger<GitHubAuthController> logger)
    {
        _gitHubOAuthService = gitHubOAuthService;
        _gitHubSessionService = gitHubSessionService;
        _logger = logger;
    }

    [HttpGet("login-url")]
    [ProducesResponseType(typeof(GitHubLoginUrlResource), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoginUrl(CancellationToken cancellationToken)
    {
        var result = await _gitHubOAuthService.StartAuthorizationAsync(cancellationToken);
        return Ok(new GitHubLoginUrlResource(result.AuthorizationUrl, result.State));
    }

    [HttpPost("callback")]
    [ProducesResponseType(typeof(GitHubSessionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteOAuthCallback(
        [FromBody] GitHubOAuthCallbackRequestResource request,
        CancellationToken cancellationToken)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.State))
        {
            return BadRequest(new { error = "OAuth callback requires code and state." });
        }

        var completion = await _gitHubOAuthService.CompleteAuthorizationAsync(request.Code, request.State, cancellationToken);
        var session = await _gitHubSessionService.CreateSessionAsync(completion.AccessToken, completion.UserLogin, cancellationToken);

        _logger.LogInformation("GitHub OAuth completed for user {UserLogin}.", session.UserLogin);

        return Ok(new GitHubSessionResource(session.SessionId, session.ExpiresAtUtc, session.UserLogin));
    }

    [HttpGet("repositories")]
    [ProducesResponseType(typeof(IReadOnlyCollection<GitHubRepositoryResource>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetRepositories([FromQuery] string sessionId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return Unauthorized(new { error = "Session is required." });
        }

        var accessToken = await _gitHubSessionService.GetAccessTokenAsync(sessionId, cancellationToken);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Unauthorized(new { error = "Session is invalid or expired." });
        }

        var repositories = await _gitHubOAuthService.GetUserRepositoriesAsync(accessToken, cancellationToken);
        var response = repositories
            .Select(x => new GitHubRepositoryResource(
                x.Id,
                x.Name,
                x.FullName,
                x.DefaultBranch,
                x.IsPrivate,
                x.HtmlUrl))
            .ToArray();

        return Ok(response);
    }
}