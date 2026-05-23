using Once.Domain.Abstractions;

namespace Once.Application.Services.Users;

public static class UserErrors
{
    public static Error NotFound      => Error.NotFound("User.NotFound");
    public static Error AlreadyExists => Error.Conflict("User.AlreadyExists");
}
