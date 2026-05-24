using Once.Domain.Abstractions;

namespace Once.Application.Services.Profile;

public static class ProfileErrors
{
    public static readonly Error NotFound      = Error.NotFound("Profile.NotFound");
    public static readonly Error WrongPassword = Error.Failure("Profile.WrongPassword");
    public static readonly Error SamePassword  = Error.Conflict("Profile.SamePassword");
}
