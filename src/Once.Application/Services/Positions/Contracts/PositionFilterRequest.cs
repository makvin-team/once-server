using Once.Domain.Abstractions;

namespace Once.Application.Services.Positions.Contracts;

public record PositionFilterRequest : BaseFilter
{
    public string? Search { get; init; }
}
