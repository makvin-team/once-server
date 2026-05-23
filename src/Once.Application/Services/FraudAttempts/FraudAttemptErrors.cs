using Once.Domain.Abstractions;

namespace Once.Application.Services.FraudAttempts;

public static class FraudAttemptErrors
{
    public static readonly Error NotFound        = Error.NotFound("FraudAttempt.NotFound");
    public static readonly Error ScenarioInvalid = Error.Validation("FraudAttempt.ScenarioInvalid");
}
