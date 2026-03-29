using DevInsight.EngineeringIntelligence.Domain.Commands;
using DevInsight.EngineeringIntelligence.Interfaces.Resources;

namespace DevInsight.EngineeringIntelligence.Interfaces.Assemblers;

public static class AnalyzeRepositoryCommandAssembler
{
    public static AnalyzeRepositoryCommand ToCommand(AnalyzeRepositoryRequestResource resource) =>
        new(resource.RepositoryName, resource.RepositoryUrl, resource.Branch);
}
