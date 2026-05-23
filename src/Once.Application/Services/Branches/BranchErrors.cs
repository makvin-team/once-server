using Once.Domain.Abstractions;

namespace Once.Application.Services.Branches;

public static class BranchErrors
{
    public static readonly Error NotFound    = Error.NotFound("Branch.NotFound");
    public static readonly Error AlreadyExists = Error.Conflict("Branch.AlreadyExists");
}
