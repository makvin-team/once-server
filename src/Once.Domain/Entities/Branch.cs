using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Once.Domain.Entities.Common;

namespace Once.Domain.Entities;

public class Branch : AuditableModelBase<long>
{
    [MaxLength(255)]
    [Column("name")]
    public required string Name { get; set; }

    [MaxLength(50)]
    [Column("code")]
    public required string Code { get; set; }

    [MaxLength(500)]
    [Column("address")]
    public string? Address { get; set; }

    public List<User> Users { get; set; } = new();
}
