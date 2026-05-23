using Once.Application.Services.Branches.Contracts;
using Once.Domain.Abstractions;

namespace Once.Application.Services.Branches;

public interface IBranchService
{
    Task<Result<PagedList<BranchResponse>>> GetAllAsync(BranchFilterRequest filter, CancellationToken ct = default);
    Task<Result<List<BranchResponse>>>     GetAllLookupAsync(CancellationToken ct = default);
    Task<Result<BranchResponse>>           GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result>                           AddAsync(CreateBranchRequest request, CancellationToken ct = default);
    Task<Result>                           UpdateAsync(long id, UpdateBranchRequest request, CancellationToken ct = default);
    Task<Result>                           DeleteAsync(long id, CancellationToken ct = default);
}
