using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Once.Domain.Abstractions;
using Once.Domain.Entities;

namespace Once.Infrastructure.Authentication;

public class JwtTokenGenerator(IOptions<JwtOptions> options) : IJwtTokenGenerator
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateAccessToken(User user)
    {
        var fullName = $"{user.FirstName} {user.LastName}".Trim();

        var claims = new List<Claim>
        {
            new(CustomClaims.Id,        user.Id.ToString()),
            new(CustomClaims.Role,      user.Role.ToString()),
            new(ClaimTypes.Role,        user.Role.ToString()),
            new(CustomClaims.Username,  user.Username),
            new(CustomClaims.FirstName, user.FirstName),
            new(CustomClaims.LastName,  user.LastName),
            new(CustomClaims.FullName,  fullName),
            new(ClaimTypes.Name,        fullName),
        };

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer:             _options.Issuer,
            audience:           _options.Audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(_options.Expiration),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
