namespace Once.Application.Helpers;

public static class EnvironmentHelper
{
    private const string EnvironmentKey = "ASPNETCORE_ENVIRONMENT";
    private const string DevelopmentEnvironmentKey = "Development";
    private const string StagingEnvironmentKey = "Staging";
    private const string ProductionEnvironmentKey = "Production";

    private static string Environment =>
        System.Environment.GetEnvironmentVariable(EnvironmentKey) ?? DevelopmentEnvironmentKey;

    public static bool IsDevelopment =>
        Environment.Equals(DevelopmentEnvironmentKey, StringComparison.OrdinalIgnoreCase);

    public static bool IsStaging =>
        Environment.Equals(StagingEnvironmentKey, StringComparison.OrdinalIgnoreCase);

    public static bool IsProduction =>
        Environment.Equals(ProductionEnvironmentKey, StringComparison.OrdinalIgnoreCase);
}