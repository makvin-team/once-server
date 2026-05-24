using Once.Domain.Entities.Common;

namespace Once.Application.Services.FraudScenarios.Contracts;

public class FraudScenarioResponse
{
    public long               Id               { get; set; }
    public MultiLanguageField Title            { get; set; } = "";
    public MultiLanguageField Description      { get; set; } = "";
    public string             FraudType        { get; set; } = "";
    public string             Difficulty       { get; set; } = "";
    public string             RiskLevel        { get; set; } = "";
    public int                EstimatedMinutes { get; set; }
    public int                PassScore        { get; set; }
    public int                AverageScore     { get; set; }
    public int                AttemptsCount    { get; set; }
    public string             UpdatedAt        { get; set; } = "";
    public MultiLanguageField LearnerRole      { get; set; } = "";

    public int?    PreviousBest  { get; set; }
    public string? InitialStatus { get; set; }
    public string? PlayUrl       { get; set; }
}
