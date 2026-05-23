using Once.Domain.Entities.Common;

namespace Once.Domain.Entities;

/// <summary>
/// Local ownership map: links an ai-backend conversation id to the once-server
/// user who owns it. Used to authorize history reads and deletes, since the
/// shared api-key makes ai-backend see every once user as the same upstream user.
/// </summary>
public class AiConversation : AuditableModelBase<long>
{
    public Guid ConversationId { get; set; }
    public long OwnerUserId    { get; set; }

    public User Owner { get; set; } = null!;
}
