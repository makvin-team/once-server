using Once.Domain.Enums;

namespace Once.Application.Services.Users.Contracts;

public record CreateUserRequest(
    string   Username,
    string   Password,
    string   FirstName,
    string   LastName,
    EnumRole Role,
    string?  PhoneNumber
);
