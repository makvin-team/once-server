using Microsoft.Extensions.DependencyInjection;
using Once.Application.Services.Auth;
using Once.Application.Services.FraudAttempts;
using Once.Application.Services.FraudScenarios;
using Once.Application.Services.Users;

namespace Once.Application;

public static class Dependencies
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFraudScenarioService, FraudScenarioService>();
        services.AddScoped<IFraudAttemptService, FraudAttemptService>();
        return services;
    }
}
