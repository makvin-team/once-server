using Microsoft.AspNetCore.Mvc;
using Once.Api.Extensions;
using Once.Application.Services.Users;
using Once.Application.Services.Users.Contracts;
using Once.Domain.Abstractions;
using Once.Domain.Enums;
using Once.Infrastructure.Authentication;

namespace Once.Api.Controllers.Management;

/// <summary>Manages system users.</summary>
[Route("api/users")]
public class UsersController(IUserService userService) : AuthorizedController
{
    /// <summary>Returns a paged list of users.</summary>
    [HttpGet]
    [HasPermission(EnumPermission.ViewUser)]
    public async Task<IResult> GetAllAsync([FromQuery] UserFilterRequest filter, CancellationToken ct)
    {
        var result = await userService.GetAllAsync(filter, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Returns a single user by ID.</summary>
    [HttpGet("{id:long}")]
    [HasPermission(EnumPermission.ViewUser)]
    public async Task<IResult> GetByIdAsync(long id, CancellationToken ct)
    {
        var result = await userService.GetByIdAsync(id, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Creates a new user.</summary>
    [HttpPost]
    [HasPermission(EnumPermission.CreateUser)]
    public async Task<IResult> CreateAsync([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var result = await userService.CreateAsync(request, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Updates an existing user.</summary>
    [HttpPut("{id:long}")]
    [HasPermission(EnumPermission.EditUser)]
    public async Task<IResult> UpdateAsync(long id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var result = await userService.UpdateAsync(request with { Id = id }, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Soft-deletes a user.</summary>
    [HttpDelete("{id:long}")]
    [HasPermission(EnumPermission.DeleteUser)]
    public async Task<IResult> DeleteAsync(long id, CancellationToken ct)
    {
        var result = await userService.DeleteAsync(id, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}
