namespace Once.Infrastructure.Brokers.AiBackend;

public interface IAiBackendBroker
{
    /// <summary>Opens the SSE chat stream. Caller disposes the response and reads its content stream.</summary>
    Task<HttpResponseMessage> OpenChatStreamAsync(AiChatStreamRequest request, CancellationToken ct);

    Task<AiPaged<AiConversationDto>> GetConversationsAsync(string externalUserId, CancellationToken ct);

    Task<AiPaged<AiMessageDto>> GetMessagesAsync(Guid conversationId, CancellationToken ct);

    Task DeleteConversationAsync(Guid conversationId, CancellationToken ct);

    /// <summary>Uploads a document into the knowledge_base collection. Streams the content.</summary>
    Task<AiDocumentDto> UploadDocumentAsync(Stream content, string fileName, string contentType, CancellationToken ct);

    Task<AiPaged<AiDocumentDto>> GetDocumentsAsync(string? embeddingStatus, int limit, CancellationToken ct);

    Task<AiDocumentStatsDto> GetDocumentStatsAsync(CancellationToken ct);

    // ---- Citation document viewer passthroughs ----
    // Each returns the raw upstream response so the controller can copy the
    // status + content-type and stream the body straight through. The caller
    // disposes the response.

    /// <summary>Regulatory document metadata (JSON).</summary>
    Task<HttpResponseMessage> GetRegulatoryDocumentAsync(Guid documentId, CancellationToken ct);

    /// <summary>Sibling language variants of a regulatory document (JSON).</summary>
    Task<HttpResponseMessage> GetRegulatoryDocumentSiblingsAsync(Guid documentId, CancellationToken ct);

    /// <summary>Regulatory document body — HTML inline or PDF/DOCX as a file.</summary>
    Task<HttpResponseMessage> GetRegulatoryDocumentContentAsync(Guid documentId, CancellationToken ct);

    /// <summary>Uploaded knowledge-base file body (e.g. a cited PDF).</summary>
    Task<HttpResponseMessage> GetKnowledgeFileContentAsync(string filename, CancellationToken ct);
}
