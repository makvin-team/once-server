using Once.Application.Services.Users.Contracts;
using Once.Domain.Abstractions;

namespace Once.Application.Services.Users;

public interface IUserService
{
    Task<Result<PagedList<UserResponse>>> GetAllAsync(UserFilterRequest filter, CancellationToken ct = default);
    Task<Result<UserResponse>>           GetByIdAsync(long id, CancellationToken ct = default);
    Task<Result<UserResponse>>           CreateAsync(CreateUserRequest request, CancellationToken ct = default);
    Task<Result<UserResponse>>           UpdateAsync(UpdateUserRequest request, CancellationToken ct = default);
    Task<Result>                         DeleteAsync(long id, CancellationToken ct = default);
}
