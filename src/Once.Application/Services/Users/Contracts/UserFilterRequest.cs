using Once.Domain.Abstractions;
using Once.Domain.Enums;

namespace Once.Application.Services.Users.Contracts;

public record UserFilterRequest : BaseFilter
{
    public string?   Search     { get; init; }
    public EnumRole? Role       { get; init; }
    public bool?     IsActive   { get; init; }
    public long?     BranchId   { get; init; }
    public long?     PositionId { get; init; }
}
