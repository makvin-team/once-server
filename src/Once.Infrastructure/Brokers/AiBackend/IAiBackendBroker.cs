namespace Once.Infrastructure.Brokers.AiBackend;

public interface IAiBackendBroker
{
    /// <summary>Opens the SSE chat stream. Caller disposes the response and reads its content stream.</summary>
    Task<HttpResponseMessage> OpenChatStreamAsync(AiChatStreamRequest request, CancellationToken ct);

    Task<AiPaged<AiConversationDto>> GetConversationsAsync(string externalUserId, CancellationToken ct);

    Task<AiPaged<AiMessageDto>> GetMessagesAsync(Guid conversationId, CancellationToken ct);

    Task DeleteConversationAsync(Guid conversationId, CancellationToken ct);
}
