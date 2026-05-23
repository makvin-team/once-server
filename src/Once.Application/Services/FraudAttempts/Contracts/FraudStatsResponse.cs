namespace Once.Application.Services.FraudAttempts.Contracts;

public class FraudStatsResponse
{
    public int TotalAttempts { get; set; }
    public int AverageScore  { get; set; }
    public int BestScore     { get; set; }
    public int PassedCount   { get; set; }
    public int FailedCount   { get; set; }
}
