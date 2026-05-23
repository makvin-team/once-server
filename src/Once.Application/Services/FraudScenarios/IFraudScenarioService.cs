using Once.Application.Services.FraudScenarios.Contracts;
using Once.Domain.Abstractions;

namespace Once.Application.Services.FraudScenarios;

public interface IFraudScenarioService
{
    Task<Result<List<FraudScenarioResponse>>> GetAllAsync(long? userId, CancellationToken ct = default);
    Task<Result<FraudScenarioResponse>>       GetByIdAsync(long id, long? userId, CancellationToken ct = default);
}
