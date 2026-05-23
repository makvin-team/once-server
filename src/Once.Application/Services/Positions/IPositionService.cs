using Once.Application.Services.Positions.Contracts;
using Once.Domain.Abstractions;

namespace Once.Application.Services.Positions;

public interface IPositionService
{
    Task<Result<PagedList<PositionResponse>>> GetAllAsync(PositionFilterRequest filter, CancellationToken ct = default);
    Task<Result<List<PositionResponse>>>      GetAllLookupAsync(CancellationToken ct = default);
    Task<Result<PositionResponse>>            GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result>                              AddAsync(CreatePositionRequest request, CancellationToken ct = default);
    Task<Result>                              UpdateAsync(long id, UpdatePositionRequest request, CancellationToken ct = default);
    Task<Result>                              DeleteAsync(long id, CancellationToken ct = default);
}
