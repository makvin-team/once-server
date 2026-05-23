using Once.Domain.Abstractions;

namespace Once.Application.Services.Auth;

public static class AuthErrors
{
    public static Error InvalidCredentials => Error.Validation("Auth.InvalidCredentials");
    public static Error UserInactive       => Error.Validation("Auth.UserInactive");
    public static Error InvalidToken       => Error.Validation("Auth.InvalidToken");
    public static Error TokenExpired       => Error.Validation("Auth.TokenExpired");
    public static Error TokenRevoked       => Error.Validation("Auth.TokenRevoked");
}
