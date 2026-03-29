using DevInsight.EngineeringIntelligence.Application.CommandServices;
using DevInsight.EngineeringIntelligence.Application.Pipelines;
using DevInsight.EngineeringIntelligence.Application.QueryServices;
using Microsoft.Extensions.DependencyInjection;

namespace DevInsight.EngineeringIntelligence.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddEngineeringIntelligenceApplication(this IServiceCollection services)
    {
        services.AddScoped<IAnalyzeRepositoryCommandService, AnalyzeRepositoryCommandService>();
        services.AddScoped<IAnalysisQueryService, AnalysisQueryService>();
        services.AddScoped<IAnalysisPipeline, AnalysisPipeline>();

        return services;
    }
}
