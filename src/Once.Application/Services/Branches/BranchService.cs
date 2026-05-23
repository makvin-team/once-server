using Microsoft.EntityFrameworkCore;
using Once.Application.Services.Branches.Contracts;
using Once.Domain.Abstractions;
using Once.Domain.Entities;
using Once.Infrastructure.Persistence;

namespace Once.Application.Services.Branches;

public class BranchService(AppDbContext dbContext) : IBranchService
{
    public async Task<Result<PagedList<BranchResponse>>> GetAllAsync(
        BranchFilterRequest filter,
        CancellationToken ct = default)
    {
        var query = dbContext.Branches
            .AsNoTracking()
            .Where(b => !b.IsDeleted);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(b =>
                b.Name.Contains(filter.Search) ||
                b.Code.Contains(filter.Search));

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderBy(b => b.Id)
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .Select(b => new BranchResponse(
                b.Id,
                b.Name,
                b.Code,
                b.Address,
                b.CreatedAt,
                dbContext.Users.Count(u => u.BranchId == b.Id && !u.IsDeleted)))
            .ToListAsync(ct);

        return Result<PagedList<BranchResponse>>.Success(
            new PagedList<BranchResponse>(items, totalCount, filter.PageIndex, filter.PageSize));
    }

    public async Task<Result<List<BranchResponse>>> GetAllLookupAsync(CancellationToken ct = default)
    {
        var items = await dbContext.Branches
            .AsNoTracking()
            .Where(b => !b.IsDeleted)
            .OrderBy(b => b.Name)
            .Select(b => new BranchResponse(b.Id, b.Name, b.Code, b.Address, b.CreatedAt, 0))
            .ToListAsync(ct);

        return items;
    }

    public async Task<Result<BranchResponse>> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var branch = await dbContext.Branches
            .AsNoTracking()
            .Where(b => b.Id == id && !b.IsDeleted)
            .Select(b => new BranchResponse(
                b.Id,
                b.Name,
                b.Code,
                b.Address,
                b.CreatedAt,
                dbContext.Users.Count(u => u.BranchId == b.Id && !u.IsDeleted)))
            .SingleOrDefaultAsync(ct);

        if (branch is null) return BranchErrors.NotFound;
        return branch;
    }

    public async Task<Result> AddAsync(CreateBranchRequest request, CancellationToken ct = default)
    {
        var codeExists = await dbContext.Branches
            .AsNoTracking()
            .AnyAsync(b => b.Code == request.Code && !b.IsDeleted, ct);

        if (codeExists) return BranchErrors.AlreadyExists;

        var branch = new Branch
        {
            Name    = request.Name,
            Code    = request.Code,
            Address = request.Address
        };

        dbContext.Branches.Add(branch);
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(long id, UpdateBranchRequest request, CancellationToken ct = default)
    {
        var branch = await dbContext.Branches
            .Where(b => b.Id == id && !b.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (branch is null) return BranchErrors.NotFound;

        branch.Name    = request.Name;
        branch.Address = request.Address;

        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(long id, CancellationToken ct = default)
    {
        var branch = await dbContext.Branches
            .Where(b => b.Id == id && !b.IsDeleted)
            .SingleOrDefaultAsync(ct);

        if (branch is null) return BranchErrors.NotFound;

        branch.IsDeleted = true;
        await dbContext.SaveChangesAsync(ct);
        return Result.Success();
    }
}
