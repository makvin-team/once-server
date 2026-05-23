using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Once.Api.Extensions;
using Once.Application.Services.Auth;
using Once.Application.Services.Auth.Contracts;

namespace Once.Api.Controllers.Auth;

/// <summary>Handles authentication — login, token refresh, and logout.</summary>
[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>Authenticates a user with username and password. Returns access and refresh tokens.</summary>
    [HttpPost("login")]
    public async Task<IResult> LoginAsync([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await authService.LoginAsync(request, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Issues a new token pair given a valid, non-revoked refresh token.</summary>
    [HttpPost("refresh")]
    public async Task<IResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await authService.RefreshTokenAsync(request, ct);
        return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
    }

    /// <summary>Revokes the given refresh token, invalidating the session.</summary>
    [HttpPost("logout")]
    public async Task<IResult> LogoutAsync([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var result = await authService.LogoutAsync(request.RefreshToken, ct);
        return result.IsSuccess ? Results.Ok() : result.ToProblemDetails();
    }
}
