using Microsoft.EntityFrameworkCore;
using Once.Application.Services.Positions.Contracts;
using Once.Domain.Abstractions;
using Once.Domain.Entities;
using Once.Infrastructure.Persistence;

namespace Once.Application.Services.Positions;

public class PositionService(AppDbContext dbContext) : IPositionService
{
    public async Task<Result<PagedList<PositionResponse>>> GetAllAsync(
        PositionFilterRequest filter,
        CancellationToken ct = default)
    {
        var query = dbContext.Positions
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(p =>
                p.Name.Contains(filter.Search) ||
                p.Code.Contains(filter.Search));

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderBy(p => p.Id)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .Select(p => new PositionResponse(
                p.Id,
                p.Name,
                p.Code,
                p.CreatedAt,
                dbContext.Users.Count(u => u.PositionId == p.Id && !u.IsDeleted)))
            .ToListAsync(ct);

        return Result<PagedList<PositionResponse>>.Success(
            new PagedList<PositionResponse>(items, totalCount, filter.PageIndex, filter.PageSize));
    }

    public async Task<Result<List<PositionResponse>>> GetAllLookupAsync(CancellationToken ct = default)
    {
        var items = await dbContext.Positions
            .AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.Name)
            .Select(p => new PositionResponse(p.Id, p.Name, p.Code, p.CreatedAt, 0))
            .ToListAsync(ct);

        return items;
    }

    public async Task<Result<PositionResponse>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var position = await dbContext.Positions
            .AsNoTracking()
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new PositionResponse(
                p.Id,
                p.Name,
                p.Code,
                p.CreatedAt,
                dbContext.Users.Count(u => u.PositionId == p.Id && !u.IsDeleted)))
            .SingleOrDefaultAsync(ct);

        if (position is null) return PositionErrors.NotFound;
        return position;
    }

    public async Task<Result> AddAsync(CreatePositionRequest request, CancellationToken ct = default)
    {
        var codeExists = await dbContext.Positions
            .AsNoTracking()
            .AnyAsync(p => p.Code == request.Code && !p.IsDeleted, ct);

        if (codeExists) return PositionErrors.AlreadyExists;

        var position = new Position
        {
            Name = request.Name,
            Code = request.Code
        };

        dbContext.Positions.Add(position);
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(long id, UpdatePositionRequest request, CancellationToken ct = default)
    {
        var position = await dbContext.Positions
            .Where(p => p.Id == id && !p.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (position is null) return PositionErrors.NotFound;

        position.Name = request.Name;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(long id, CancellationToken ct = default)
    {
        var position = await dbContext.Positions
            .Where(p => p.Id == id && !p.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (position is null) return PositionErrors.NotFound;

        position.IsDeleted = true;
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
