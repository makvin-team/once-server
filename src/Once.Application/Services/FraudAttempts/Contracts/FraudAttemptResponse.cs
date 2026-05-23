namespace Once.Application.Services.FraudAttempts.Contracts;

public class FraudAttemptResponse
{
    public long   Id             { get; set; }
    public long   ScenarioId     { get; set; }
    public string ScenarioTitle  { get; set; } = "";
    public string FraudType      { get; set; } = "";
    public int    Score          { get; set; }
    public bool   Passed         { get; set; }
    public int    DetectedFlags  { get; set; }
    public int    MissedFlags    { get; set; }
    public string AttemptedAt    { get; set; } = "";
}
