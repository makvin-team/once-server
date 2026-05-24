using Once.Application.Services.Profile.Contracts;
using Once.Application.Services.Users.Contracts;
using Once.Domain.Abstractions;

namespace Once.Application.Services.Profile;

public interface IProfileService
{
    Task<Result<UserResponse>> GetProfileAsync(long userId, CancellationToken ct = default);
    Task<Result<UserResponse>> UpdateProfileAsync(long userId, UpdateProfileRequest request, CancellationToken ct = default);
    Task<Result>               ChangePasswordAsync(long userId, ChangePasswordRequest request, CancellationToken ct = default);
}
