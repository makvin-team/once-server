using Once.Domain.Entities.Common;
using Once.Domain.Enums;

namespace Once.Domain.Entities;

public class User : AuditableModelBase<long>
{
    public required string Username     { get; set; }
    public required string PasswordHash { get; set; }
    public EnumRole        Role         { get; set; }
    public bool            IsActive     { get; set; } = true;
    public required string FirstName    { get; set; }
    public required string LastName     { get; set; }
    public string?         PhoneNumber  { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
