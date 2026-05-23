namespace Once.Application.Services.AiAssistant.Contracts;

/// <summary>Chat message coming from once-client. ConversationId is null for a new chat.</summary>
public sealed record AiChatRequest
{
    public Guid?  ConversationId { get; init; }
    public string Message        { get; init; } = string.Empty;
}
