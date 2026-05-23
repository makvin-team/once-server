using Hangfire;
using Once.Api;
using Once.Api.Filters;
using Once.Application;
using Once.Application.Jobs;
using Once.Infrastructure;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder
    .ConfigureKestrel()
    .ConfigureHostConfigurations();

builder.Services
    .ConfigureHangfire(builder.Configuration)
    .ConfigureJobs()
    .AddInfrastructureServices()
    .AddApplicationServices()
    .ConfigureServices()
    .ConfigureAuthentication()
    .ConfigureLocalization()
    .ConfigureDbContext(builder.Configuration)
    .ConfigureSwagger()
    .ConfigureControllers()
    .ConfigureCors()
    .ConfigureGlobalExceptionHandler();

// Add health checks for Docker container monitoring
builder.Services.AddHealthChecks();

var app = builder.Build();

// if (!app.Environment.IsProduction())
{
    app.UseSwagger((Action<SwaggerOptions>)(options =>
    {
        options.PreSerializeFilters.Add((Action<OpenApiDocument, HttpRequest>)((swagger, req) => swagger.Servers = new List<OpenApiServer>()
                {
                    new OpenApiServer()
                    {
                        Description = app.Environment.EnvironmentName + " server",
                        Url = "/"
                    }
                }));
        
    }));

    app.UseSwaggerUI(options =>
    {
        options.ConfigObject.AdditionalItems.Add("persistAuthorization", true);
        options.InjectStylesheet("/swagger-ui/custom.css");
        options.DocExpansion(DocExpansion.None);
    });
}

//app.UseMiddleware<RequestResponseLoggingMiddleware>();

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors();
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Map health check endpoint for Docker health monitoring
app.MapHealthChecks("/health");

//app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
app.UseForwardedHeaders();
await app.ApplyMigrationsAsync();

var options = new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    PrefixPath = null
};
app.UseHangfireDashboard("/hangfire", options);

JobsRegistrar.RegisterRecurringJobs();

await app.RunAsync();

// Required for WebApplicationFactory in integration tests
namespace Once.Api
{
    public partial class Program;
}