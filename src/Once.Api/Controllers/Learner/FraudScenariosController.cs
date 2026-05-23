using Microsoft.AspNetCore.Mvc;
using Once.Application.Services.FraudAttempts;
using Once.Application.Services.FraudAttempts.Contracts;
using Once.Application.Services.FraudScenarios;
using Once.Api.Extensions;

namespace Once.Api.Controllers.Learner;

[ApiController]
[Route("api/fraud")]
public class FraudScenariosController(
    IFraudScenarioService scenarioService,
    IFraudAttemptService  attemptService) : AuthorizedController
{
    /// <summary>Get all fraud scenarios with learner-specific status.</summary>
    [HttpGet("scenarios")]
    public async Task<IResult> GetAllAsync(CancellationToken ct = default)
    {
        var result = await scenarioService.GetAllAsync(UserId, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Get fraud scenario by id.</summary>
    [HttpGet("scenarios/{id:long}")]
    public async Task<IResult> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var result = await scenarioService.GetByIdAsync(id, UserId, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Get learner fraud attempts.</summary>
    [HttpGet("attempts")]
    public async Task<IResult> GetAttemptsAsync(CancellationToken ct = default)
    {
        var result = await attemptService.GetAllByUserAsync(UserId, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Get learner fraud stats.</summary>
    [HttpGet("stats")]
    public async Task<IResult> GetStatsAsync(CancellationToken ct = default)
    {
        var result = await attemptService.GetStatsByUserAsync(UserId, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Submit a fraud scenario attempt.</summary>
    [HttpPost("attempts")]
    public async Task<IResult> SubmitAttemptAsync(
        [FromBody] SubmitFraudAttemptRequest request,
        CancellationToken ct = default)
    {
        var result = await attemptService.SubmitAsync(UserId, request, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }
}
