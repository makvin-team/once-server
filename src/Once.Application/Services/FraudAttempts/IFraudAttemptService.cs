using Once.Application.Services.FraudAttempts.Contracts;
using Once.Domain.Abstractions;

namespace Once.Application.Services.FraudAttempts;

public interface IFraudAttemptService
{
    Task<Result<List<FraudAttemptResponse>>> GetAllByUserAsync(long userId, CancellationToken ct = default);
    Task<Result<FraudStatsResponse>>         GetStatsByUserAsync(long userId, CancellationToken ct = default);
    Task<Result<FraudAttemptResponse>>       SubmitAsync(long userId, SubmitFraudAttemptRequest request, CancellationToken ct = default);
}
