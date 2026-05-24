using Microsoft.EntityFrameworkCore;
using Once.Application.Services.Auth;
using Once.Application.Services.Profile.Contracts;
using Once.Application.Services.Users.Contracts;
using Once.Domain.Abstractions;
using Once.Infrastructure.Persistence;

namespace Once.Application.Services.Profile;

public class ProfileService(AppDbContext dbContext, IPasswordHasher passwordHasher) : IProfileService
{
    public async Task<Result<UserResponse>> GetProfileAsync(long userId, CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId && !u.IsDeleted)
            .Select(u => new UserResponse(
                u.Id,
                u.Username,
                u.FirstName,
                u.LastName,
                u.Role,
                u.IsActive,
                u.PhoneNumber,
                u.CreatedAt,
                u.BranchId,
                u.Branch != null ? u.Branch.Name : null,
                u.PositionId,
                u.Position != null ? u.Position.Name : null))
            .SingleOrDefaultAsync(ct);

        return user is null ? ProfileErrors.NotFound : user;
    }

    public async Task<Result<UserResponse>> UpdateProfileAsync(
        long userId,
        UpdateProfileRequest request,
        CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == userId && !u.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (user is null) return ProfileErrors.NotFound;

        user.FirstName   = request.FirstName;
        user.LastName    = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        await dbContext.SaveChangesAsync(ct);

        return await GetProfileAsync(userId, ct);
    }

    public async Task<Result> ChangePasswordAsync(
        long userId,
        ChangePasswordRequest request,
        CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == userId && !u.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (user is null) return ProfileErrors.NotFound;

        if (!passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            return ProfileErrors.WrongPassword;

        if (passwordHasher.Verify(request.NewPassword, user.PasswordHash))
            return ProfileErrors.SamePassword;

        user.PasswordHash = passwordHasher.Hash(request.NewPassword);
        await dbContext.SaveChangesAsync(ct);

        return Result.Success();
    }
}
