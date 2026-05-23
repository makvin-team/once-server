using Once.Domain.Entities.Common;

namespace Once.Domain.Entities;

public class RefreshToken : ModelBase<long>
{
    public required string Token     { get; set; }
    public DateTime        ExpiresAt { get; set; }
    public bool            IsRevoked { get; set; }
    public long            UserId    { get; set; }

    public User User { get; set; } = default!;
}
