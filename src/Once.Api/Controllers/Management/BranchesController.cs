using Microsoft.AspNetCore.Mvc;
using Once.Api.Extensions;
using Once.Application.Services.Branches;
using Once.Application.Services.Branches.Contracts;
using Once.Domain.Enums;
using Once.Infrastructure.Authentication;

namespace Once.Api.Controllers.Management;

/// <summary>Manages branches.</summary>
[Route("api/branches")]
public class BranchesController(IBranchService branchService) : AuthorizedController
{
    /// <summary>Returns a paged list of branches.</summary>
    [HttpGet]
    [HasPermission(EnumPermission.ViewBranch)]
    public async Task<IResult> GetAllAsync([FromQuery] BranchFilterRequest filter, CancellationToken ct)
    {
        var result = await branchService.GetAllAsync(filter, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Returns all branches without pagination for dropdown use.</summary>
    [HttpGet("lookup")]
    [HasPermission(EnumPermission.ViewBranch)]
    public async Task<IResult> GetAllLookupAsync(CancellationToken ct)
    {
        var result = await branchService.GetAllLookupAsync(ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Returns a single branch by ID.</summary>
    [HttpGet("{id:long}")]
    [HasPermission(EnumPermission.ViewBranch)]
    public async Task<IResult> GetByIdAsync(long id, CancellationToken ct)
    {
        var result = await branchService.GetByIdAsync(id, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Creates a new branch.</summary>
    [HttpPost]
    [HasPermission(EnumPermission.CreateBranch)]
    public async Task<IResult> AddAsync([FromBody] CreateBranchRequest request, CancellationToken ct)
    {
        var result = await branchService.AddAsync(request, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }

    /// <summary>Updates an existing branch.</summary>
    [HttpPut("{id:long}")]
    [HasPermission(EnumPermission.EditBranch)]
    public async Task<IResult> UpdateAsync(long id, [FromBody] UpdateBranchRequest request, CancellationToken ct)
    {
        var result = await branchService.UpdateAsync(id, request, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }

    /// <summary>Soft-deletes a branch.</summary>
    [HttpDelete("{id:long}")]
    [HasPermission(EnumPermission.DeleteBranch)]
    public async Task<IResult> DeleteAsync(long id, CancellationToken ct)
    {
        var result = await branchService.DeleteAsync(id, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}
