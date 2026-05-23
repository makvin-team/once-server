using Hangfire.Dashboard;

namespace Once.Api.Filters;

/// <summary>
/// Custom Hangfire dashboard authorization filter
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    /// <inheritdoc/>
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Custom basic authentication
        string? authHeader = httpContext.Request.Headers["Authorization"];

        if (authHeader != null && authHeader.StartsWith("Basic "))
        {
            var encodedUsernamePassword = authHeader.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[1]?.Trim();
            var decodedUsernamePassword = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedUsernamePassword!));

            var username = decodedUsernamePassword.Split(':', 2)[0];
            var password = decodedUsernamePassword.Split(':', 2)[1];

            var config = httpContext.RequestServices.GetRequiredService<IConfiguration>();
            var dashboardSection = config.GetSection("Hangfire");
            var isAuthorized = username == dashboardSection["Username"] &&
                   password == dashboardSection["Password"];

            if (isAuthorized)
                return true;
        }

        httpContext.Response.Headers["WWW-Authenticate"] = "Basic";
        httpContext.Response.StatusCode = 401;

        return false;
    }
}
