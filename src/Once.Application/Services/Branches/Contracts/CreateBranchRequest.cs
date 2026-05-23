namespace Once.Application.Services.Branches.Contracts;

public record CreateBranchRequest(
    string  Name,
    string  Code,
    string? Address
);
