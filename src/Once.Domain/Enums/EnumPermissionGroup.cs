using System.ComponentModel.DataAnnotations;

namespace Once.Domain.Enums;
public enum EnumPermissionGroup
{
    [Display(Name = "Users")]
    Users = 1,

    [Display(Name = "Branches")]
    Branches = 2,

    [Display(Name = "Positions")]
    Positions = 3,
}