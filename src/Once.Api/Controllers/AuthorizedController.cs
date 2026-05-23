using Once.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Once.Domain.Exceptions;

namespace Once.Api.Controllers;

/// <inheritdoc />
[ApiController]
[Authorize]
public class AuthorizedController : ControllerBase
{
    private long? _userId;

    public long UserId
    {
        get
        {
            if (_userId.HasValue)
                return _userId.Value;

            var rawValue = HttpContext.User.FindFirstValue(CustomClaims.Id) ??
                           throw new UnauthorizedException($"Required claim not found");

            var value = Convert.ChangeType(rawValue, typeof(long));

            _userId = (long)(value ?? throw new UnauthorizedException($"Required claim not found"));

            return _userId.Value;
        }
    }
}