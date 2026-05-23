using System.ComponentModel.DataAnnotations.Schema;

namespace Once.Domain.Entities.Common;

public abstract class ReferenceModelBase<T> : ModelBase<T> where T : struct
{
    [Column("group_name", TypeName = "jsonb")]
    public MultiLanguageField? GroupName { get; set; }

    [Column("is_system_defined")] public bool IsSystemDefined { get; set; }
}