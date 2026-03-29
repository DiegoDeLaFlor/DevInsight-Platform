using DevInsight.EngineeringIntelligence.Application.CommandServices;
using DevInsight.EngineeringIntelligence.Application.QueryServices;
using DevInsight.EngineeringIntelligence.Domain.Queries;
using DevInsight.EngineeringIntelligence.Interfaces.Assemblers;
using DevInsight.EngineeringIntelligence.Interfaces.Resources;
using Microsoft.AspNetCore.Mvc;

namespace DevInsight.Api.Controllers;

[ApiController]
[Route("api/engineering-intelligence/repositories")]
public sealed class EngineeringIntelligenceController : ControllerBase
{
    private readonly IAnalyzeRepositoryCommandService _commandService;
    private readonly IAnalysisQueryService _queryService;
    private readonly ILogger<EngineeringIntelligenceController> _logger;

    public EngineeringIntelligenceController(
        IAnalyzeRepositoryCommandService commandService,
        IAnalysisQueryService queryService,
        ILogger<EngineeringIntelligenceController> logger)
    {
        _commandService = commandService;
        _queryService = queryService;
        _logger = logger;
    }

    [HttpPost("analyze")]
    [ProducesResponseType(typeof(AnalyzeRepositoryResponseResource), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AnalyzeRepository([FromBody] AnalyzeRepositoryRequestResource request, CancellationToken cancellationToken)
    {
        if (request is null)
        {
            return BadRequest(new { error = "Request body is required." });
        }

        var command = AnalyzeRepositoryCommandAssembler.ToCommand(request);
        var result = await _commandService.HandleAsync(command, cancellationToken);

        _logger.LogInformation("Analysis requested for repository {RepositoryId}", result.RepositoryId);

        return Accepted(new AnalyzeRepositoryResponseResource(result.RepositoryId, result.AnalysisId));
    }

    [HttpGet("{repositoryId:guid}/analysis/latest")]
    [ProducesResponseType(typeof(AnalysisResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestAnalysis([FromRoute] Guid repositoryId, CancellationToken cancellationToken)
    {
        var analysis = await _queryService.HandleAsync(new GetAnalysisByRepositoryIdQuery(repositoryId), cancellationToken);
        if (analysis is null)
        {
            return NotFound(new { error = "Analysis not found for repository." });
        }

        return Ok(AnalysisResourceAssembler.ToResource(analysis));
    }
}
