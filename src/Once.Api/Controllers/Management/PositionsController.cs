using Microsoft.AspNetCore.Mvc;
using Once.Api.Extensions;
using Once.Application.Services.Positions;
using Once.Application.Services.Positions.Contracts;
using Once.Domain.Enums;
using Once.Infrastructure.Authentication;

namespace Once.Api.Controllers.Management;

/// <summary>Manages positions.</summary>
[Route("api/positions")]
public class PositionsController(IPositionService positionService) : AuthorizedController
{
    /// <summary>Returns a paged list of positions.</summary>
    [HttpGet]
    [HasPermission(EnumPermission.ViewPosition)]
    public async Task<IResult> GetAllAsync([FromQuery] PositionFilterRequest filter, CancellationToken ct)
    {
        var result = await positionService.GetAllAsync(filter, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Returns all positions without pagination for dropdown use.</summary>
    [HttpGet("lookup")]
    [HasPermission(EnumPermission.ViewPosition)]
    public async Task<IResult> GetAllLookupAsync(CancellationToken ct)
    {
        var result = await positionService.GetAllLookupAsync(ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Returns a single position by ID.</summary>
    [HttpGet("{id:long}")]
    [HasPermission(EnumPermission.ViewPosition)]
    public async Task<IResult> GetByIdAsync(long id, CancellationToken ct)
    {
        var result = await positionService.GetByIdAsync(id, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Creates a new position.</summary>
    [HttpPost]
    [HasPermission(EnumPermission.CreatePosition)]
    public async Task<IResult> AddAsync([FromBody] CreatePositionRequest request, CancellationToken ct)
    {
        var result = await positionService.AddAsync(request, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }

    /// <summary>Updates an existing position.</summary>
    [HttpPut("{id:long}")]
    [HasPermission(EnumPermission.EditPosition)]
    public async Task<IResult> UpdateAsync(long id, [FromBody] UpdatePositionRequest request, CancellationToken ct)
    {
        var result = await positionService.UpdateAsync(id, request, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }

    /// <summary>Soft-deletes a position.</summary>
    [HttpDelete("{id:long}")]
    [HasPermission(EnumPermission.DeletePosition)]
    public async Task<IResult> DeleteAsync(long id, CancellationToken ct)
    {
        var result = await positionService.DeleteAsync(id, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}
