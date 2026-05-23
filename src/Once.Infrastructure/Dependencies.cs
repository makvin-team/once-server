using Microsoft.Extensions.DependencyInjection;
using Once.Domain.Abstractions;
using Once.Infrastructure.Authentication;

namespace Once.Infrastructure;

public static class Dependencies
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        return services;
    }
}
