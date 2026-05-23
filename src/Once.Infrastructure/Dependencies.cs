using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Once.Domain.Abstractions;
using Once.Infrastructure.Authentication;
using Once.Infrastructure.Brokers.AiBackend;

namespace Once.Infrastructure;

public static class Dependencies
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        var aiOptions = configuration
            .GetSection(AiBackendOptions.SectionName)
            .Get<AiBackendOptions>() ?? new AiBackendOptions();

        services.AddHttpClient<IAiBackendBroker, AiBackendBroker>(client =>
        {
            client.BaseAddress = new Uri(aiOptions.BaseUrl);
            client.DefaultRequestHeaders.Add("X-API-Key", aiOptions.ApiKey);
            // ai-backend is reached through an ngrok tunnel whose free tier serves
            // an HTML browser-warning interstitial instead of proxying. This header
            // skips it so we get the real JSON response back.
            client.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        return services;
    }
}
