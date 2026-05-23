using Microsoft.EntityFrameworkCore;
using Once.Domain.Entities;
using Once.Domain.Enums;
using Once.Infrastructure.Persistence;

namespace Once.Infrastructure.Extensions;

public static class DbContextExtensions
{
    private static readonly (string Username, string Password, string FirstName, string LastName, EnumRole Role)[] SeedUsers =
    [
        ("admin", "Admin@123",  "Admin",   "User",    EnumRole.Admin),
        ("hr",    "Hr@123456",  "HR",      "Manager", EnumRole.Hr),
        ("user",  "User@1234",  "Regular", "User",    EnumRole.User),
    ];

    public static async Task SeedAsync(this AppDbContext dbContext)
    {
        var existingUsernames = await dbContext.Users
            .AsNoTracking()
            .Where(u => !u.IsDeleted)
            .Select(u => u.Username)
            .ToListAsync();

        var existingSet = existingUsernames.ToHashSet();

        var toInsert = SeedUsers
            .Where(s => !existingSet.Contains(s.Username))
            .Select(s => new User
            {
                Username     = s.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(s.Password, workFactor: 12),
                FirstName    = s.FirstName,
                LastName     = s.LastName,
                Role         = s.Role,
                IsActive     = true,
            })
            .ToList();

        if (toInsert.Count == 0)
            return;

        dbContext.Users.AddRange(toInsert);
        await dbContext.SaveChangesAsync();
    }
}