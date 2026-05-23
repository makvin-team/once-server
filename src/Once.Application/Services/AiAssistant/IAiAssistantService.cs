using Once.Application.Services.AiAssistant.Contracts;
using Once.Domain.Abstractions;
using Once.Infrastructure.Brokers.AiBackend;

namespace Once.Application.Services.AiAssistant;

public interface IAiAssistantService
{
    /// <summary>Relays ai-backend SSE frames (each ends with a blank line) for the user.</summary>
    IAsyncEnumerable<string> StreamChatAsync(long userId, AiChatRequest request, CancellationToken ct);

    Task<Result<AiPaged<AiConversationDto>>> GetConversationsAsync(long userId, CancellationToken ct);

    Task<Result<AiPaged<AiMessageDto>>> GetMessagesAsync(long userId, Guid conversationId, CancellationToken ct);

    Task<Result> DeleteConversationAsync(long userId, Guid conversationId, CancellationToken ct);
}
