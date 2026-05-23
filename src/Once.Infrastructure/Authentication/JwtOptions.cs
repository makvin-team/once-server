namespace Once.Infrastructure.Authentication;

public class JwtOptions
{
    public string Issuer { get; init; } = default!;
    public string Audience { get; init; } = default!;
    public string SecretKey { get; set; } = default!;
    public int Expiration { get; set; }
    public int RefreshTokenExpiration { get; set; }
}