namespace Once.Infrastructure.Brokers.AiBackend;

public sealed class AiBackendOptions
{
    public const string SectionName = "AiBackend";

    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey  { get; set; } = string.Empty;
}
