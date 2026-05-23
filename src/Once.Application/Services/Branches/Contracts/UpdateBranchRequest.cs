namespace Once.Application.Services.Branches.Contracts;

public record UpdateBranchRequest(
    string  Name,
    string? Address
);
