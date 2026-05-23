namespace Once.Application.Services.Auth.Contracts;

public record LoginResponse(
    string   AccessToken,
    string   RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt
);
