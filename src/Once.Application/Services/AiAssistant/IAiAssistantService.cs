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

    // ---- Citation document viewer passthroughs ----
    // Cited reference material (regulatory acts, uploaded files) is shared and
    // read-only, so these aren't ownership-guarded — any authenticated learner
    // may open a document the assistant cited. The caller disposes the response.

    Task<HttpResponseMessage> GetRegulatoryDocumentAsync(Guid documentId, CancellationToken ct);

    Task<HttpResponseMessage> GetRegulatoryDocumentSiblingsAsync(Guid documentId, CancellationToken ct);

    Task<HttpResponseMessage> GetRegulatoryDocumentContentAsync(Guid documentId, CancellationToken ct);

    Task<HttpResponseMessage> GetKnowledgeFileContentAsync(string filename, CancellationToken ct);
}
