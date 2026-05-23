using Once.Domain.Entities;

namespace Once.Domain.Abstractions;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
