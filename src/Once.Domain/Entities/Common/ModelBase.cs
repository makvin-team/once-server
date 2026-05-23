using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Once.Domain.Entities.Common;

public abstract class ModelBase<TId> where TId : struct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")] public TId Id { get; set; }

    /// <summary>
    /// Записни хисобдан чиқариш учун керак бўлган
    /// 1 - актив
    /// 0 - 
    /// </summary>
    [Column("is_deleted")] public bool IsDeleted { get; set; }
}
