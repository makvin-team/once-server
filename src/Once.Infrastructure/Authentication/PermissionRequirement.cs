using Once.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Once.Infrastructure.Authentication;

public class PermissionRequirement(EnumPermission permission) : IAuthorizationRequirement
{
    public EnumPermission Permission { get; private set; } = permission;
}   