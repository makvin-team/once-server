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
}
