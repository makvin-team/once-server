using Once.Domain.Abstractions;

namespace Once.Application.Services.Positions;

public static class PositionErrors
{
    public static readonly Error NotFound      = Error.NotFound("Position.NotFound");
    public static readonly Error AlreadyExists = Error.Conflict("Position.AlreadyExists");
}
