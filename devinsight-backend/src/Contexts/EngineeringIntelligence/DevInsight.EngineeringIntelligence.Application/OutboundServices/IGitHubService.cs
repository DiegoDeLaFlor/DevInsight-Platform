namespace DevInsight.EngineeringIntelligence.Application.OutboundServices;

public interface IGitHubService
{
    Task<string> CloneRepositoryAsync(string repositoryUrl, string? branch, string? accessToken, CancellationToken cancellationToken);
}
