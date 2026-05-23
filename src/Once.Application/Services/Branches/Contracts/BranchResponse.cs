namespace Once.Application.Services.Branches.Contracts;

public record BranchResponse(
    long     Id,
    string   Name,
    string   Code,
    string?  Address,
    DateTime CreatedAt,
    int      UsersCount
);
