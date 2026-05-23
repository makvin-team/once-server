using Microsoft.Extensions.DependencyInjection;
using Once.Application.Services.AiAssistant;
using Once.Application.Services.Auth;
using Once.Application.Services.Users;

namespace Once.Application;

public static class Dependencies
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAiAssistantService, AiAssistantService>();
        return services;
    }
}
