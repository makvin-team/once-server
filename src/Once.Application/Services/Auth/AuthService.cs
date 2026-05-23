using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Once.Application.Services.Auth.Contracts;
using Once.Domain.Abstractions;
using Once.Domain.Entities;
using Once.Infrastructure.Authentication;
using Once.Infrastructure.Persistence;

namespace Once.Application.Services.Auth;

public class AuthService(
    AppDbContext dbContext,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Username == request.Username && !u.IsDeleted, ct);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            return AuthErrors.InvalidCredentials;

        if (!user.IsActive)
            return AuthErrors.UserInactive;

        return await IssueTokenPairAsync(user, ct);
    }

    public async Task<Result<LoginResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken ct = default)
    {
        var token = await dbContext.RefreshTokens
            .Include(rt => rt.User)
            .SingleOrDefaultAsync(rt => rt.Token == request.RefreshToken && !rt.IsDeleted, ct);

        if (token is null)         return AuthErrors.InvalidToken;
        if (token.IsRevoked)       return AuthErrors.TokenRevoked;
        if (token.ExpiresAt < DateTime.UtcNow) return AuthErrors.TokenExpired;
        if (!token.User.IsActive)  return AuthErrors.UserInactive;

        token.IsRevoked = true;
        await dbContext.SaveChangesAsync(ct);

        return await IssueTokenPairAsync(token.User, ct);
    }

    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        var token = await dbContext.RefreshTokens
            .SingleOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsDeleted, ct);

        if (token is null) return AuthErrors.InvalidToken;

        token.IsRevoked = true;
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<Result<LoginResponse>> IssueTokenPairAsync(User user, CancellationToken ct)
    {
        var accessToken       = jwtTokenGenerator.GenerateAccessToken(user);
        var refreshTokenValue = jwtTokenGenerator.GenerateRefreshToken();
        var now               = DateTime.UtcNow;
        var refreshExpiresAt  = now.AddMinutes(_jwt.RefreshTokenExpiration);

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            Token     = refreshTokenValue,
            UserId    = user.Id,
            ExpiresAt = refreshExpiresAt,
            IsRevoked = false
        });

        await dbContext.SaveChangesAsync(ct);

        return new LoginResponse(
            accessToken,
            refreshTokenValue,
            now.AddMinutes(_jwt.Expiration),
            refreshExpiresAt);
    }
}
