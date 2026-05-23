using Once.Domain.Enums;

namespace Once.Application.Services.Users.Contracts;

public record UpdateUserRequest(
    long     Id,
    string   FirstName,
    string   LastName,
    EnumRole Role,
    bool     IsActive,
    string?  PhoneNumber
);
