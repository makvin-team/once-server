namespace Once.Application.Services.FraudAttempts.Contracts;

public class SubmitFraudAttemptRequest
{
    public long         ScenarioId          { get; set; }
    public int          Score               { get; set; }
    public bool         Passed              { get; set; }
    public int          DetectedFlags       { get; set; }
    public int          MissedFlags         { get; set; }
    public string       SelectedDecisionId  { get; set; } = "";
    public List<string> SelectedFlagIds     { get; set; } = new();
}
