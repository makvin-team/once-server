using System.ComponentModel;

namespace Once.Domain.Enums;
/// <summary>
/// Enum representing the permissions in the system. 
/// 
/// Permissions are formatted as [Action][Resource] (e.g., ViewUser, CreateRole). 
/// - Action represents the operation (View, Create, Edit, Delete).
/// - Resource represents the entity or area (User, Role).
/// 
/// This enum is used for role-based access control (RBAC) to manage access to various
/// system resources and actions. Each permission corresponds to a specific action 
/// a user can perform on a resource.
/// </summary>

public enum EnumPermission
{
    // Users
    [Category(nameof(EnumPermissionGroup.Users))]
    CreateUser = 1001,
    ViewUser,
    EditUser,
    DeleteUser,

    // Branches
    [Category(nameof(EnumPermissionGroup.Branches))]
    CreateBranch = 2001,
    ViewBranch,
    EditBranch,
    DeleteBranch,

    // Positions
    [Category(nameof(EnumPermissionGroup.Positions))]
    CreatePosition = 3001,
    ViewPosition,
    EditPosition,
    DeletePosition,
}
