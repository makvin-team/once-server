using Microsoft.AspNetCore.Mvc;
using Once.Api.Extensions;
using Once.Application.Services.Profile;
using Once.Application.Services.Profile.Contracts;

namespace Once.Api.Controllers.Profile;

/// <summary>Current user profile management.</summary>
[Route("api/profile")]
public class ProfileController(IProfileService profileService) : AuthorizedController
{
    /// <summary>Returns the current user's profile.</summary>
    [HttpGet]
    public async Task<IResult> GetProfileAsync(CancellationToken ct = default)
    {
        var result = await profileService.GetProfileAsync(UserId, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Updates the current user's first name, last name, and phone number.</summary>
    [HttpPut]
    public async Task<IResult> UpdateProfileAsync(
        [FromBody] UpdateProfileRequest request,
        CancellationToken ct = default)
    {
        var result = await profileService.UpdateProfileAsync(UserId, request, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Changes the current user's password.</summary>
    [HttpPut("password")]
    public async Task<IResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest request,
        CancellationToken ct = default)
    {
        var result = await profileService.ChangePasswordAsync(UserId, request, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}
