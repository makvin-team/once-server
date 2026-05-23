using Once.Domain.Abstractions;

namespace Once.Application.Services.Branches.Contracts;

public record BranchFilterRequest : BaseFilter
{
    public string? Search { get; init; }
}
