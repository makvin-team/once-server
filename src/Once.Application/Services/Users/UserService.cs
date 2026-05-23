using Microsoft.EntityFrameworkCore;
using Once.Application.Services.Auth;
using Once.Application.Services.Users.Contracts;
using Once.Domain.Abstractions;
using Once.Domain.Entities;
using Once.Infrastructure.Persistence;

namespace Once.Application.Services.Users;

public class UserService(AppDbContext dbContext, IPasswordHasher passwordHasher) : IUserService
{
    public async Task<Result<PagedList<UserResponse>>> GetAllAsync(
        UserFilterRequest filter,
        CancellationToken ct = default)
    {
        var query = dbContext.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(u =>
                u.Username.Contains(filter.Search) ||
                u.FirstName.Contains(filter.Search) ||
                u.LastName.Contains(filter.Search));

        if (filter.Role.HasValue)
            query = query.Where(u => u.Role == filter.Role.Value);

        if (filter.IsActive.HasValue)
            query = query.Where(u => u.IsActive == filter.IsActive.Value);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderBy(u => u.Id)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .Select(u => MapToResponse(u))
            .ToListAsync(ct);

        return Result<PagedList<UserResponse>>.Success(
            new PagedList<UserResponse>(items, totalCount, filter.PageIndex, filter.PageSize));
    }

    public async Task<Result<UserResponse>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == id && !u.IsDeleted)
            .Select(u => MapToResponse(u))
            .SingleOrDefaultAsync(ct);

        if (user is null) return UserErrors.NotFound;
        return user;
    }

    public async Task<Result<UserResponse>> CreateAsync(
        CreateUserRequest request,
        CancellationToken ct = default)
    {
        var exists = await dbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Username == request.Username && !u.IsDeleted, ct);

        if (exists) return UserErrors.AlreadyExists;

        var user = new User
        {
            Username     = request.Username,
            PasswordHash = passwordHasher.Hash(request.Password),
            FirstName    = request.FirstName,
            LastName     = request.LastName,
            Role         = request.Role,
            IsActive     = true,
            PhoneNumber  = request.PhoneNumber
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(ct);

        return MapToResponse(user);
    }

    public async Task<Result<UserResponse>> UpdateAsync(
        UpdateUserRequest request,
        CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == request.Id && !u.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (user is null) return UserErrors.NotFound;

        user.FirstName   = request.FirstName;
        user.LastName    = request.LastName;
        user.Role        = request.Role;
        user.IsActive    = request.IsActive;
        user.PhoneNumber = request.PhoneNumber;

        await dbContext.SaveChangesAsync(ct);
        return MapToResponse(user);
    }

    public async Task<Result> DeleteAsync(long id, CancellationToken ct = default)
    {
        var user = await dbContext.Users
            .Where(u => u.Id == id && !u.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (user is null) return UserErrors.NotFound;

        user.IsDeleted = true;
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static UserResponse MapToResponse(User u) => new(
        u.Id,
        u.Username,
        u.FirstName,
        u.LastName,
        u.Role,
        u.IsActive,
        u.PhoneNumber,
        u.CreatedAt);
}
