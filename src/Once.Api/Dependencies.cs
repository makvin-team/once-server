using Once.Application;
using Once.Infrastructure.Authentication;
using Once.Infrastructure.Extensions;
using Once.Infrastructure.Extensions.Seed;
using Once.Infrastructure.Persistence;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;
using Once.Api.Converters;
using Once.Api.Filters;
using Once.Api.Middlewares;

namespace Once.Api;

/// <inheritdoc />
public static class Dependencies
{
    public static WebApplicationBuilder ConfigureKestrel(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = null; });
        builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = long.MaxValue; });
        builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 968_435_456; });
        builder.WebHost.UseKestrel();
        builder.Services.Configure<ForwardedHeadersOptions>(o =>
        {
            o.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto |
                ForwardedHeaders.XForwardedHost;

            // Разреши доверенных прокси (пример — локальный Nginx)
            o.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
            o.ForwardLimit = 2;
        });
        return builder;
    }

    public static WebApplicationBuilder ConfigureHostConfigurations(this WebApplicationBuilder builder)
    {
        _ = builder.Configuration.AddJsonFile(
            Path.Join(AppContext.BaseDirectory,
                $"appsettings.{builder.Environment.EnvironmentName}.json"),
            optional: false);
        _ = builder.Configuration.AddJsonFile(
            Path.Join(AppContext.BaseDirectory,
                $"appsettings.json"),
            optional: false);
        builder.Configuration.AddEnvironmentVariables();

        return builder;
    }

    public static IServiceCollection ConfigureHangfire(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHangfire(x =>
        {
            x.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(
                    o => o.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnectionString")),
                    new PostgreSqlStorageOptions
                    {
                        SchemaName = "background",
                        PrepareSchemaIfNecessary = true
                    });
        });
        services.AddHangfireServer();

        return services;
    }

    public static IServiceCollection ConfigureJobs(
        this IServiceCollection services)
    {
        return services;
    }

    public static IServiceCollection ConfigureServices(
        this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddScoped<IHttpContextAccessorService, HttpContextAccessorService>();

        return services;
    }

    public static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services)
    {
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }

    public static IServiceCollection ConfigureLocalization(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        return services;
    }

    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(configuration
                .GetConnectionString("DefaultConnectionString")).EnableDynamicJson();

        services.AddDbContextPool<AppDbContext>(optionsBuilder =>
        {
            optionsBuilder
                .UseNpgsql(dataSourceBuilder.Build())
                .UseSnakeCaseNamingConvention();
        });

        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1",
                new OpenApiInfo()
                {
                    Title = "Once Api",
                    Version = "v1"
                });

            var xmlFile = $"{Assembly.GetEntryAssembly()!.GetName().Name}.xml";
            var xmlPath = Path.Join(AppContext.BaseDirectory, xmlFile);

            // Application XML
            var applicationAssembly = typeof(ApplicationAssemblyReference).Assembly;
            var appXml = $"{applicationAssembly.GetName().Name}.xml";
            var appXmlPath = Path.Combine(AppContext.BaseDirectory, appXml);

            options.CustomSchemaIds(type => type.FullName);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                options.IncludeXmlComments(appXmlPath);
            }
            options.OperationFilter<MlfHeaderFilter>();
            options.OperationFilter<PermissionFilter>();
            options.SchemaFilter<EnumSchemaFilter>();
            options.OperationFilter<EnumOperationFilter>();

            var securityScheme = new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Cookie and Header based Authentication",
                Name = "Jwt",
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                Reference = new OpenApiReference()
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            options.AddSecurityDefinition("Bearer", securityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    securityScheme, new List<string>()
                    {
                        "Bearer"
                    }
                }
            });
        });

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
        services.AddCookiePolicy(options => { options.Secure = CookieSecurePolicy.Always; });

        return services;
    }

    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
            options.AppendTrailingSlash = true;
        });

        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            var accessor = services.BuildServiceProvider().GetRequiredService<IHttpContextAccessor>();
            options.SerializerOptions.Converters.Add(new MultiLanguageFieldConverter(accessor));
        });

        services.AddControllers(options =>
        {
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            options.Filters.Add<ModelValidationFilter>();
        }).AddJsonOptions(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
            options.JsonSerializerOptions.Converters.Add(new MultiLanguageFieldConverter(httpContextAccessor));
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });
        
        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policyBuilder =>
                policyBuilder
                    //.AllowCredentials()
                    //.WithOrigins(builder.Configuration.GetSection("Origins").Get<string[]?>() ?? ["localhost"])
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
            );
        });

        return services;
    }

    public static IServiceCollection ConfigureGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandlerMiddleware>();
        services.AddProblemDetails();

        return services;
    }

    public static async Task ApplyMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Check and apply pending migrations
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

        if (pendingMigrations.Any())
        {
            Console.WriteLine("Applying pending migrations...");
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Migrations applied successfully.");
        }
        else
        {
            Console.WriteLine("No pending migrations found.");
        }

        await dbContext.SeedAsync();
        await dbContext.SeedFraudScenariosAsync();
    }
}
