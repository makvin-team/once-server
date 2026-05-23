using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Once.Domain.Entities.Common;

namespace Once.Domain.Entities;

public class Position : AuditableModelBase<long>
{
    [MaxLength(255)]
    [Column("name")]
    public required string Name { get; set; }

    [MaxLength(50)]
    [Column("code")]
    public required string Code { get; set; }

    public List<User> Users { get; set; } = new();
}
