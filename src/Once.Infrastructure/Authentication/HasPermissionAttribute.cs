using Once.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace Once.Infrastructure.Authentication;

public class HasPermissionAttribute(EnumPermission permission) :
    AuthorizeAttribute(permission.ToString());