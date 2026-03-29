using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using DevInsight.EngineeringIntelligence.Domain.Repositories;
using DevInsight.EngineeringIntelligence.Infrastructure.Persistence;
using DevInsight.EngineeringIntelligence.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsight.EngineeringIntelligence.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEngineeringIntelligenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRepositoryRepository, InMemoryRepositoryRepository>();
        services.AddSingleton<IAnalysisRepository, InMemoryAnalysisRepository>();

        services.Configure<AiEngineOptions>(configuration.GetSection(AiEngineOptions.SectionName));

        services.AddHttpClient<IAIInsightService, AiEngineInsightService>();
        services.AddScoped<IGitHubService, FileSystemGitHubService>();
        services.AddScoped<ICodeAnalyzerService, RoslynCodeAnalyzerService>();

        return services;
    }
}
