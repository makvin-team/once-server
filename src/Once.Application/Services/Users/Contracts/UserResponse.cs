using Once.Domain.Enums;

namespace Once.Application.Services.Users.Contracts;

public record UserResponse(
    long     Id,
    string   Username,
    string   FirstName,
    string   LastName,
    EnumRole Role,
    bool     IsActive,
    string?  PhoneNumber,
    DateTime CreatedAt,
    long?    BranchId,
    string?  BranchName,
    long?    PositionId,
    string?  PositionName
);
