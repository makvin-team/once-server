using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Once.Infrastructure.Brokers.AiBackend;

public sealed class AiBackendBroker(HttpClient httpClient) : IAiBackendBroker
{
    internal static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
    };

    private const string KnowledgeCollection = "knowledge_base";

    public async Task<HttpResponseMessage> OpenChatStreamAsync(
        AiChatStreamRequest request, CancellationToken ct)
    {
        var message = new HttpRequestMessage(HttpMethod.Post, "chat/stream")
        {
            Content = JsonContent.Create(request, options: Json),
        };
        return await httpClient.SendAsync(
            message, HttpCompletionOption.ResponseHeadersRead, ct);
    }

    public async Task<AiPaged<AiConversationDto>> GetConversationsAsync(
        string externalUserId, CancellationToken ct)
    {
        var url = $"conversations?external_user_id={Uri.EscapeDataString(externalUserId)}&limit=100";
        return await httpClient.GetFromJsonAsync<AiPaged<AiConversationDto>>(url, Json, ct)
               ?? new AiPaged<AiConversationDto>();
    }

    public async Task<AiPaged<AiMessageDto>> GetMessagesAsync(
        Guid conversationId, CancellationToken ct)
    {
        var url = $"messages?conversation_id={conversationId}&limit=100";
        return await httpClient.GetFromJsonAsync<AiPaged<AiMessageDto>>(url, Json, ct)
               ?? new AiPaged<AiMessageDto>();
    }

    public async Task DeleteConversationAsync(Guid conversationId, CancellationToken ct)
    {
        var response = await httpClient.DeleteAsync($"conversations/{conversationId}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task<AiDocumentDto> UploadDocumentAsync(
        Stream content, string fileName, string contentType, CancellationToken ct)
    {
        using var form = new MultipartFormDataContent();
        var fileContent = new StreamContent(content);
        if (!string.IsNullOrWhiteSpace(contentType))
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        form.Add(fileContent, "file", fileName);
        form.Add(new StringContent(KnowledgeCollection), "collection_name");

        var response = await httpClient.PostAsync("documents/upload-file", form, ct);
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<AiDocumentDto>(Json, ct);
        return dto ?? throw new HttpRequestException("Empty document response from ai-backend");
    }

    public async Task<AiPaged<AiDocumentDto>> GetDocumentsAsync(
        string? embeddingStatus, int limit, CancellationToken ct)
    {
        var url = $"documents?limit={limit}";
        if (!string.IsNullOrWhiteSpace(embeddingStatus))
            url += $"&embedding_status={Uri.EscapeDataString(embeddingStatus)}";
        return await httpClient.GetFromJsonAsync<AiPaged<AiDocumentDto>>(url, Json, ct)
               ?? new AiPaged<AiDocumentDto>();
    }

    public async Task<AiDocumentStatsDto> GetDocumentStatsAsync(CancellationToken ct)
        => await httpClient.GetFromJsonAsync<AiDocumentStatsDto>("documents/stats", Json, ct)
           ?? new AiDocumentStatsDto();

    public Task<HttpResponseMessage> GetRegulatoryDocumentAsync(Guid documentId, CancellationToken ct)
        => GetPassthroughAsync($"regulatory/documents/{documentId}", ct);

    public Task<HttpResponseMessage> GetRegulatoryDocumentSiblingsAsync(Guid documentId, CancellationToken ct)
        => GetPassthroughAsync($"regulatory/documents/{documentId}/siblings", ct);

    public Task<HttpResponseMessage> GetRegulatoryDocumentContentAsync(Guid documentId, CancellationToken ct)
        => GetPassthroughAsync($"regulatory/documents/{documentId}/content", ct);

    public Task<HttpResponseMessage> GetKnowledgeFileContentAsync(string filename, CancellationToken ct)
        => GetPassthroughAsync($"documents/content/{Uri.EscapeDataString(filename)}", ct);

    // ResponseHeadersRead so document bodies stream rather than buffer in memory.
    private Task<HttpResponseMessage> GetPassthroughAsync(string url, CancellationToken ct)
    {
        var message = new HttpRequestMessage(HttpMethod.Get, url);
        return httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, ct);
    }
}
