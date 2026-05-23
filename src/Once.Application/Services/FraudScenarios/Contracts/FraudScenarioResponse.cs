namespace Once.Application.Services.FraudScenarios.Contracts;

public class FraudScenarioResponse
{
    public long   Id               { get; set; }
    public string Title            { get; set; } = "";
    public string Description      { get; set; } = "";
    public string FraudType        { get; set; } = "";
    public string Difficulty       { get; set; } = "";
    public string RiskLevel        { get; set; } = "";
    public int    EstimatedMinutes { get; set; }
    public int    PassScore        { get; set; }
    public int    AverageScore     { get; set; }
    public int    AttemptsCount    { get; set; }
    public string UpdatedAt        { get; set; } = "";

    public List<string>           Skills         { get; set; } = new();
    public string                 Context        { get; set; } = "";
    public string                 LearnerRole    { get; set; } = "";
    public string                 Task           { get; set; } = "";
    public object                 Evidence       { get; set; } = new();
    public List<RedFlagOptionDto> RedFlagOptions { get; set; } = new();
    public List<DecisionOptionDto> DecisionOptions { get; set; } = new();
    public string                 Explanation    { get; set; } = "";
    public string                 Recommendation { get; set; } = "";

    public int?    PreviousBest   { get; set; }
    public string? InitialStatus  { get; set; }
}

public class RedFlagOptionDto
{
    public string Id      { get; set; } = "";
    public string Label   { get; set; } = "";
    public bool   Correct { get; set; }
}

public class DecisionOptionDto
{
    public string Id      { get; set; } = "";
    public string Label   { get; set; } = "";
    public bool   Correct { get; set; }
}
